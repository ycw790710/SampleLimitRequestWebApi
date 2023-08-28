using Microsoft.AspNetCore.Mvc.Controllers;
using SampleLimitRequestWebApi.CodeExtensions;
using SampleLimitRequestWebApi.RequestRateLimits.Components;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;

namespace SampleLimitRequestWebApi.RequestRateLimits;

public class RequestRateLimitCacheService : IRequestRateLimitCacheService
{
    private const int TypeLength = 4;
    private const int GlobalControllerIndex = 0;
    private const int GlobalActionIndex = 1;
    private const int IpIndex = 2;
    private const int UserIndex = 3;
    private static int RequestRateLimitPerTimeUnitCount { get; }
        = Enum.GetNames(typeof(RequestRateLimitPerTimeUnit)).Length;

    private readonly IReadOnlyList<ConcurrentDictionary<string, RequestRateLimitCacheContainter>> _caches;
    private readonly IReadOnlyList<object> _lock_waitingExpiredQueues;
    private readonly IReadOnlyList<ConcurrentQueue<(string key, TimeSpan ExpiredTime)>> _waitingExpiredQueues;

    public RequestRateLimitCacheService()
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
    }

    private bool Valid(int cacheIndex, string key, RequestRateLimitPerTimeUnit perTimeUnit, int limitTimes)
    {
        RemoveExpired(cacheIndex, key);

        var cache = _caches[cacheIndex];
        if (!cache.ContainsKey(key))
            cache.TryAdd(key, new(RequestRateLimitPerTimeUnitCount));
        var container = cache[key];
        return container.Valid(perTimeUnit, limitTimes);
    }

    private void RemoveExpired(int cacheIndex, string key)
    {
        var cache = _caches[cacheIndex];
        var lock_waitingExpiredQueue = _lock_waitingExpiredQueues[cacheIndex];
        var waitingExpiredQueue = _waitingExpiredQueues[cacheIndex];
        while (waitingExpiredQueue.Count > 0 &&
            waitingExpiredQueue.TryPeek(out var container) &&
            container.ExpiredTime < GlobalTimer.NowTimeSpan())
        {
            var got = false;
            lock (lock_waitingExpiredQueue)
            {
                if (waitingExpiredQueue.Count > 0 &&
                    waitingExpiredQueue.TryPeek(out container) &&
                    container.ExpiredTime < GlobalTimer.NowTimeSpan() &&
                    waitingExpiredQueue.TryDequeue(out container))
                    got = true;
            }
            if (got)
            {
                cache.TryRemove(key, out var rm);
            }
        }
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
            valid &= Valid(GlobalActionIndex, key, perTimeUnit, limitTimes);
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
            valid &= Valid(GlobalControllerIndex, key, perTimeUnit, limitTimes);
        }

        var ipRequestRateLimits = DistinctByMinLimitTimes(
            typeInfo
                .GetCustomAttributes(typeof(IpRequestRateLimitAttribute), true)
                .Select(n => (IpRequestRateLimitAttribute)n)
                .Concat(
                methodInfo
                .GetCustomAttributes(typeof(IpRequestRateLimitAttribute), true)
                .Select(n => (IpRequestRateLimitAttribute)n)
                )
                );
        foreach (var ipRequestRateLimit in ipRequestRateLimits)
        {
            var key = ipRequestRateLimit.GetKey(remoteIpAddress);
            var perTimeUnit = ipRequestRateLimit.PerTimeUnit;
            var limitTimes = ipRequestRateLimit.LimitTimes;
            valid &= Valid(IpIndex, key, perTimeUnit, limitTimes);
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

        var userRequestRateLimits = DistinctByMinLimitTimes(typeInfo
                .GetCustomAttributes(typeof(UserRequestRateLimitAttribute), true)
                .Select(n => (UserRequestRateLimitAttribute)n)
                .Concat(methodInfo
                .GetCustomAttributes(typeof(UserRequestRateLimitAttribute), true)
                .Select(n => (UserRequestRateLimitAttribute)n)
                )
                );
        foreach (var userRequestRateLimit in userRequestRateLimits)
        {
            var key = userRequestRateLimit.GetKey(userId);
            var perTimeUnit = userRequestRateLimit.PerTimeUnit;
            var limitTimes = userRequestRateLimit.LimitTimes;
            valid &= Valid(UserIndex, key, perTimeUnit, limitTimes);
        }

        return valid;
    }

    class RequestRateLimitCacheContainter
    {
        private readonly IReadOnlyList<object> _lockItems;
        private readonly RequestRateLimitCacheContainterItem[] _items;

        private readonly object _lockWaitingExpiredItems;
        private readonly ConcurrentQueue<(RequestRateLimitPerTimeUnit key, TimeSpan expiredTime)> _waiting_expiredItems;

        public TimeSpan ExpiredTime { get; private set; }

        public RequestRateLimitCacheContainter(int RequestRateLimitPerTimeUnitCount)
        {
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

            ExpiredTime = GlobalTimer.AddMillisecondsForNowTimeSpan(10 * 1000);
        }

        public bool Valid(RequestRateLimitPerTimeUnit perTimeUnit, int limitTimes)
        {
            RemoveExpiredItem();

            var valid = true;
            TimeSpan expiredTime = TimeSpan.Zero;
            var idx = (int)perTimeUnit;
            var lockItem = _lockItems[idx];
            lock (lockItem)
            {
                if (_items[idx] == null)
                    _items[idx] = new(limitTimes, perTimeUnit);
                expiredTime = _items[idx].Enter();
                valid = _items[idx].Valid();
            }
            _waiting_expiredItems.Enqueue((perTimeUnit, expiredTime));
            if (expiredTime > ExpiredTime)
                ExpiredTime = expiredTime;

            return valid;
        }

        private void RemoveExpiredItem()
        {
            while (_waiting_expiredItems.Count > 0 &&
                _waiting_expiredItems.TryPeek(out var item) &&
                item.expiredTime < GlobalTimer.NowTimeSpan())
            {
                var got = false;
                lock (_lockWaitingExpiredItems)
                {
                    if (_waiting_expiredItems.Count > 0 &&
                        _waiting_expiredItems.TryPeek(out item) &&
                        item.expiredTime < GlobalTimer.NowTimeSpan() &&
                        _waiting_expiredItems.TryDequeue(out item))
                        got = true;
                }
                if (got)
                {
                    var idx = (int)item.key;
                    var lockItem = _lockItems[idx];
                    lock (lockItem)
                    {
                        if (_items[idx] != null)
                            _items[idx].Leave();
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
            Debug.WriteLine($"RequestRateLimitPerTimeUnit: {PerTimeUnit}");
            Debug.WriteLine($"Capacity/LimitTimes: {Capacity}/{LimitTimes}");
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