using Microsoft.AspNetCore.Mvc.Controllers;
using SampleLimitRequestWebApi.CodeExtensions;
using SampleLimitRequestWebApi.RequestRateLimits.Components;
using SampleLimitRequestWebApi.RequestRateLimits.Dtos;
using System.Collections.Concurrent;
using System.Net;

namespace SampleLimitRequestWebApi.RequestRateLimits;

public class RequestRateLimitCacheService : IRequestRateLimitCacheService
{
    private static int TypeLength { get; }
        = Enum.GetValues(typeof(RequestRateLimitType)).Length;
    private static int RequestRateLimitPerTimeUnitCount { get; }
        = Enum.GetValues(typeof(RequestRateLimitPerTimeUnit)).Length;

    private readonly IReadOnlyList<ConcurrentDictionary<string, RequestRateLimitCacheContainter>> _caches;
    private readonly IReadOnlyList<object> _lock_waitingExpiredQueues;
    private readonly IReadOnlyList<ConcurrentQueue<(string key, TimeSpan ExpiredTime)>> _waitingExpiredQueues;

    private readonly IRequestRateLimitStatusCacheService _requestRateLimitStatusCacheService;

    public RequestRateLimitCacheService(
        IRequestRateLimitStatusCacheService requestRateLimitStatusCacheService)
    {
        var caches = new ConcurrentDictionary<string, RequestRateLimitCacheContainter>[TypeLength];
        var lock_waitingExpiredQueues = new object[TypeLength];
        var waitingExpiredQueues = new ConcurrentQueue<(string key, TimeSpan ExpiredTime)>[TypeLength];
        for (int i = 0; i < TypeLength; i++)
        {
            caches[i] = new();
            lock_waitingExpiredQueues[i] = new();
            waitingExpiredQueues[i] = new();
        }

        _caches = caches;
        _lock_waitingExpiredQueues = lock_waitingExpiredQueues;
        _waitingExpiredQueues = waitingExpiredQueues;

        _requestRateLimitStatusCacheService = requestRateLimitStatusCacheService;
    }

    private int GetCacheIndex(RequestRateLimitType requestRateLimitType)
    {
        return (int)requestRateLimitType;
    }

    private bool Valid(RequestRateLimitType requestRateLimitType,
        string key, RequestRateLimitPerTimeUnit perTimeUnit, int limitTimes)
    {
        RemoveExpired(requestRateLimitType);

        var cacheIndex = GetCacheIndex(requestRateLimitType);

        var cache = _caches[cacheIndex];
        if (!cache.ContainsKey(key))
            cache.TryAdd(key, new(key, RequestRateLimitPerTimeUnitCount));
        var container = cache[key];
        var valid = container.Valid(perTimeUnit, limitTimes);

        var waitingExpiredQueue = _waitingExpiredQueues[cacheIndex];
        waitingExpiredQueue.Enqueue((key, container.ExpiredTime));

        AddUpdatingToWaitingSendingStatusContainers(requestRateLimitType, container);

        return valid;
    }

    private void RemoveExpired(RequestRateLimitType requestRateLimitType)
    {
        var cacheIndex = GetCacheIndex(requestRateLimitType);

        var cache = _caches[cacheIndex];
        var lock_waitingExpiredQueue = _lock_waitingExpiredQueues[cacheIndex];
        var waitingExpiredQueue = _waitingExpiredQueues[cacheIndex];
        while (waitingExpiredQueue.Count > 0 &&
            waitingExpiredQueue.TryPeek(out var expiredInfo) &&
            expiredInfo.ExpiredTime < GlobalTimer.NowTimeSpan())
        {
            var got = false;
            lock (lock_waitingExpiredQueue)
            {
                if (waitingExpiredQueue.Count > 0 &&
                    waitingExpiredQueue.TryPeek(out expiredInfo) &&
                    expiredInfo.ExpiredTime < GlobalTimer.NowTimeSpan() &&
                    waitingExpiredQueue.TryDequeue(out expiredInfo))
                    got = true;
            }
            if (got)
            {
                if (cache.TryGetValue(expiredInfo.key, out var rm) &&
                    rm.ExpiredTime < GlobalTimer.NowTimeSpan() &&
                    cache.TryRemove(expiredInfo.key, out rm) &&
                    rm != null)
                {
                    AddRemovingToWaitingSendingStatusContainers(requestRateLimitType, rm);
                }
            }
        }
    }

    private void AddUpdatingToWaitingSendingStatusContainers(RequestRateLimitType requestRateLimitType,
        RequestRateLimitCacheContainter container)
    {
        var requestRateLimitStatusContainerItems = new List<RequestRateLimitStatusContainerItem>();
        var requestRateLimitStatusContainer = new RequestRateLimitStatusContainer(container.Key,
            Convert(requestRateLimitType),
            requestRateLimitStatusContainerItems);
        foreach (var item in container.Items)
        {
            if (item == null)
                continue;
            var statusContainerItem = new RequestRateLimitStatusContainerItem(
                Convert(item.PerTimeUnit), item.LimitTimes, item.Capacity);
            requestRateLimitStatusContainerItems.Add(statusContainerItem);
        }

        _requestRateLimitStatusCacheService.SendContainer(
            RequestRateLimitStatusContainerActionType.Update, requestRateLimitStatusContainer);
    }

    private void AddRemovingToWaitingSendingStatusContainers(RequestRateLimitType requestRateLimitType,
        RequestRateLimitCacheContainter container)
    {
        var requestRateLimitStatusContainer = new RequestRateLimitStatusContainer(container.Key,
                                    Convert(requestRateLimitType));

        _requestRateLimitStatusCacheService.SendContainer(
            RequestRateLimitStatusContainerActionType.Remove, requestRateLimitStatusContainer);
    }

    private RequestRateLimitStatusContainerType Convert(RequestRateLimitType requestRateLimitType)
    {
        switch (requestRateLimitType)
        {
            case RequestRateLimitType.GlobalController:
                return RequestRateLimitStatusContainerType.GlobalController;
            case RequestRateLimitType.GlobalAction:
                return RequestRateLimitStatusContainerType.GlobalAction;
            case RequestRateLimitType.Ip:
                return RequestRateLimitStatusContainerType.Ip;
            case RequestRateLimitType.User:
                return RequestRateLimitStatusContainerType.User;
            default:
                throw new Exception("Unset Mapping");
        }
    }
    private RequestRateLimitStatusPerTimeUnit Convert(RequestRateLimitPerTimeUnit perTimeUnit)
    {
        switch (perTimeUnit)
        {
            case RequestRateLimitPerTimeUnit.Seconds:
                return RequestRateLimitStatusPerTimeUnit.Seconds;
            case RequestRateLimitPerTimeUnit.Minutes:
                return RequestRateLimitStatusPerTimeUnit.Minutes;
            case RequestRateLimitPerTimeUnit.Hours:
                return RequestRateLimitStatusPerTimeUnit.Hours;
            default:
                throw new Exception("Unset Mapping");
        }
    }

    public bool Valid(ControllerActionDescriptor controllerActionDescriptor, IPAddress remoteIpAddress)
    {
        var valid = true;
        var methodInfo = controllerActionDescriptor.MethodInfo;
        var typeInfo = controllerActionDescriptor.ControllerTypeInfo;
        var controllerName = controllerActionDescriptor.ControllerName;
        var actionName = controllerActionDescriptor.ActionName;

        var globalActionRequestRateLimits =
            DistinctByMinLimitTimes(methodInfo
            .GetCustomAttributes(typeof(GlobalRequestRateLimitAttribute), true)
            .Select(n => (GlobalRequestRateLimitAttribute)n));
        foreach (var globalActionRequestRateLimit in globalActionRequestRateLimits)
        {
            var key = globalActionRequestRateLimit.GetKey(controllerName, actionName);
            var perTimeUnit = globalActionRequestRateLimit.PerTimeUnit;
            var limitTimes = globalActionRequestRateLimit.LimitTimes;
            valid &= Valid(RequestRateLimitType.GlobalAction, key, perTimeUnit, limitTimes);
        }

        var globalControllerRequestRateLimits =
            DistinctByMinLimitTimes(typeInfo
            .GetCustomAttributes(typeof(GlobalRequestRateLimitAttribute), true)
            .Select(n => (GlobalRequestRateLimitAttribute)n));
        foreach (var globalControllerRequestRateLimit in globalControllerRequestRateLimits)
        {
            var key = globalControllerRequestRateLimit.GetKey(controllerName, "");
            var perTimeUnit = globalControllerRequestRateLimit.PerTimeUnit;
            var limitTimes = globalControllerRequestRateLimit.LimitTimes;
            valid &= Valid(RequestRateLimitType.GlobalController, key, perTimeUnit, limitTimes);
        }

        var ipActionRequestRateLimits =
            DistinctByMinLimitTimes(methodInfo
            .GetCustomAttributes(typeof(IpRequestRateLimitAttribute), true)
            .Select(n => (IpRequestRateLimitAttribute)n));
        foreach (var ipActionRequestRateLimit in ipActionRequestRateLimits)
        {
            var key = ipActionRequestRateLimit.GetKey(controllerName, actionName, remoteIpAddress);
            var perTimeUnit = ipActionRequestRateLimit.PerTimeUnit;
            var limitTimes = ipActionRequestRateLimit.LimitTimes;
            valid &= Valid(RequestRateLimitType.Ip, key, perTimeUnit, limitTimes);
        }

        var ipControllerRequestRateLimits =
            DistinctByMinLimitTimes(typeInfo
            .GetCustomAttributes(typeof(IpRequestRateLimitAttribute), true)
            .Select(n => (IpRequestRateLimitAttribute)n));
        foreach (var ipControllerRequestRateLimit in ipControllerRequestRateLimits)
        {
            var key = ipControllerRequestRateLimit.GetKey(controllerName, "", remoteIpAddress);
            var perTimeUnit = ipControllerRequestRateLimit.PerTimeUnit;
            var limitTimes = ipControllerRequestRateLimit.LimitTimes;
            valid &= Valid(RequestRateLimitType.Ip, key, perTimeUnit, limitTimes);
        }

        return valid;
    }

    public bool ValidUser(ControllerActionDescriptor controllerActionDescriptor, long userId)
    {
        var valid = true;
        var methodInfo = controllerActionDescriptor.MethodInfo;
        var typeInfo = controllerActionDescriptor.ControllerTypeInfo;
        var controllerName = controllerActionDescriptor.ControllerName;
        var actionName = controllerActionDescriptor.ActionName;

        var userActionRequestRateLimits =
            DistinctByMinLimitTimes(methodInfo
            .GetCustomAttributes(typeof(UserRequestRateLimitAttribute), true)
            .Select(n => (UserRequestRateLimitAttribute)n));
        foreach (var userActionRequestRateLimit in userActionRequestRateLimits)
        {
            var key = userActionRequestRateLimit.GetKey(controllerName, actionName, userId);
            var perTimeUnit = userActionRequestRateLimit.PerTimeUnit;
            var limitTimes = userActionRequestRateLimit.LimitTimes;
            valid &= Valid(RequestRateLimitType.User, key, perTimeUnit, limitTimes);
        }

        var userControllerRequestRateLimits =
            DistinctByMinLimitTimes(typeInfo
            .GetCustomAttributes(typeof(UserRequestRateLimitAttribute), true)
            .Select(n => (UserRequestRateLimitAttribute)n));
        foreach (var userControllerRequestRateLimit in userControllerRequestRateLimits)
        {
            var key = userControllerRequestRateLimit.GetKey(controllerName, "", userId);
            var perTimeUnit = userControllerRequestRateLimit.PerTimeUnit;
            var limitTimes = userControllerRequestRateLimit.LimitTimes;
            valid &= Valid(RequestRateLimitType.User, key, perTimeUnit, limitTimes);
        }

        return valid;
    }

    private IList<T> DistinctByMinLimitTimes<T>(IEnumerable<T> items)
        where T : IRequestRateLimitAttribute
    {
        T[] arr = new T[RequestRateLimitPerTimeUnitCount];
        foreach (var item in items)
        {
            var idx = (int)item.PerTimeUnit;
            if (arr[idx] == null || arr[idx].LimitTimes > item.LimitTimes)
                arr[idx] = item;
        }
        return arr.Where(n => n != null).ToArray();
    }

    public void RemoveExpired()
    {
        foreach (var requestRateLimitType in Enum.GetValues<RequestRateLimitType>())
        {
            RemoveExpired(requestRateLimitType);
        }
    }

    class RequestRateLimitCacheContainter
    {
        private readonly IReadOnlyList<object> _lockItems;
        private readonly RequestRateLimitCacheContainterItem[] _items;
        public IReadOnlyList<RequestRateLimitCacheContainterItem> Items => _items;

        private readonly object _lockWaitingExpiredItems;
        private readonly ConcurrentQueue<(RequestRateLimitPerTimeUnit key, TimeSpan expiredTime)> _waiting_expiredItems;

        public string Key { get; private set; }
        public TimeSpan ExpiredTime { get; private set; }

        public RequestRateLimitCacheContainter(string key, int RequestRateLimitPerTimeUnitCount)
        {
            Key = key;

            var itemsLength = RequestRateLimitPerTimeUnitCount;
            var lockItems = new object[itemsLength];
            for (int i = 0; i < itemsLength; i++)
            {
                lockItems[i] = new();
            }
            _lockItems = lockItems;
            _items = new RequestRateLimitCacheContainterItem[itemsLength];

            _lockWaitingExpiredItems = new();
            _waiting_expiredItems = new();

            ExpiredTime = GlobalTimer.NowTimeSpan();
        }

        public bool Valid(RequestRateLimitPerTimeUnit perTimeUnit, int newLimitTimes)
        {
            RemoveExpiredItem();

            var capacity = 0;
            var limitTimes = 0;

            var valid = true;
            TimeSpan expiredTime = TimeSpan.Zero;
            var idx = (int)perTimeUnit;
            var lockItem = _lockItems[idx];
            lock (lockItem)
            {
                if (_items[idx] == null)
                {
                    _items[idx] = new(newLimitTimes, perTimeUnit);
                }
                expiredTime = _items[idx].Enter();
                valid = _items[idx].Valid();

                capacity = _items[idx].Capacity;
                limitTimes = _items[idx].LimitTimes;
            }
            _waiting_expiredItems.Enqueue((perTimeUnit, expiredTime));
            if (expiredTime > ExpiredTime)
                ExpiredTime = expiredTime;

            return valid;
        }

        private void RemoveExpiredItem()
        {
            while (_waiting_expiredItems.Count > 0 &&
                _waiting_expiredItems.TryPeek(out var expiredInfo) &&
                expiredInfo.expiredTime < GlobalTimer.NowTimeSpan())
            {
                var got = false;
                lock (_lockWaitingExpiredItems)
                {
                    if (_waiting_expiredItems.Count > 0 &&
                        _waiting_expiredItems.TryPeek(out expiredInfo) &&
                        expiredInfo.expiredTime < GlobalTimer.NowTimeSpan() &&
                        _waiting_expiredItems.TryDequeue(out expiredInfo))
                        got = true;
                }
                if (got)
                {
                    var idx = (int)expiredInfo.key;
                    var lockItem = _lockItems[idx];
                    lock (lockItem)
                    {
                        var target = _items[idx];
                        if (target != null)
                        {
                            target.Leave();
                        }
                    }

                }
            }
        }
    }

    class RequestRateLimitCacheContainterItem
    {
        public int LimitTimes { get; private set; }
        public int Capacity { get; private set; }
        public RequestRateLimitPerTimeUnit PerTimeUnit { get; private set; }
        public TimeSpan ExpiredTime { get; private set; }

        public RequestRateLimitCacheContainterItem(int limitTimes, RequestRateLimitPerTimeUnit perTimeUnit)
        {
            LimitTimes = limitTimes;
            Capacity = 0;
            PerTimeUnit = perTimeUnit;
            ExpiredTime = GetExpiredTime(perTimeUnit);
        }

        public TimeSpan Enter()
        {
            Capacity++;
            var expiredTime = GetExpiredTime(PerTimeUnit);
            ExpiredTime = expiredTime;
            return expiredTime;
        }

        public bool Valid()
        {
            return Capacity <= LimitTimes;
        }

        public void Leave()
        {
            Capacity--;
        }

        private TimeSpan GetExpiredTime(RequestRateLimitPerTimeUnit perTimeUnit)
        {
            var expiredMilliseconds = 0;
            if (perTimeUnit == RequestRateLimitPerTimeUnit.Seconds)
                expiredMilliseconds = 1000;
            else if (perTimeUnit == RequestRateLimitPerTimeUnit.Minutes)
                expiredMilliseconds = 60 * 1000;
            else if (perTimeUnit == RequestRateLimitPerTimeUnit.Hours)
                expiredMilliseconds = 3600 * 1000;
            else
                throw new Exception("Miss mapping enum");
            return GlobalTimer.AddMillisecondsForNowTimeSpan(expiredMilliseconds);
        }
    }
}