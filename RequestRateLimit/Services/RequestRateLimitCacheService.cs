
using GlobalTimers.Services;

namespace RequestRateLimit.Services;

public class RequestRateLimitCacheService : IRequestRateLimitCacheService
{
    private static int LimitTypeLength { get; }
        = Enum.GetValues(typeof(RequestRateLimitType)).Length;
    private static int UnitCount { get; }
        = Enum.GetValues(typeof(RequestRateLimitPerTimeUnit)).Length;

    private readonly IReadOnlyList<ConcurrentDictionary<string, RequestRateLimitCacheContainter>> _caches;
    private readonly IReadOnlyList<object> _lock_waitingExpiredQueues;
    private readonly IReadOnlyList<ConcurrentQueue<(RequestRateLimitType type, string key, TimeSpan ExpiredTime)>> _waitingExpiredQueues;

    private readonly IRequestRateLimitStatusCacheService _requestRateLimitStatusCacheService;
    private readonly IGlobalTimerService _globalTimerService;

    public RequestRateLimitCacheService(
        IRequestRateLimitStatusCacheService requestRateLimitStatusCacheService,
        IGlobalTimerService globalTimerService)
    {
        var caches = new ConcurrentDictionary<string, RequestRateLimitCacheContainter>[LimitTypeLength];
        var lock_waitingExpiredQueues = new object[LimitTypeLength];
        var waitingExpiredQueues = new ConcurrentQueue<(RequestRateLimitType type, string key, TimeSpan ExpiredTime)>[LimitTypeLength];
        for (int i = 0; i < LimitTypeLength; i++)
        {
            caches[i] = new();
            lock_waitingExpiredQueues[i] = new();
        }
        for (int i = 0; i < UnitCount; i++)
        {
            waitingExpiredQueues[i] = new();
        }

        _caches = caches;
        _lock_waitingExpiredQueues = lock_waitingExpiredQueues;
        _waitingExpiredQueues = waitingExpiredQueues;

        _requestRateLimitStatusCacheService = requestRateLimitStatusCacheService;
        this._globalTimerService = globalTimerService;
    }

    private int GetCacheIndex(RequestRateLimitType limitType)
    {
        return (int)limitType;
    }

    private int GetWaitingExpiredQueuesIndex(RequestRateLimitPerTimeUnit unit)
    {
        return (int)unit;
    }

    private bool Valid(RequestRateLimitType limitType,
        string key, RequestRateLimitPerTimeUnit unit, int limit)
    {
        RemoveExpired(unit);

        var cache = _caches[GetCacheIndex(limitType)];

        if (!cache.ContainsKey(key))
            cache.TryAdd(key, new(key, _globalTimerService));
        var container = cache[key];
        var valid = container.Valid(unit, limit, out var expiredTime);

        var waitingExpiredQueue = _waitingExpiredQueues[GetWaitingExpiredQueuesIndex(unit)];
        waitingExpiredQueue.Enqueue((limitType, key, expiredTime));

        AddUpdatingToWaitingSendingStatusContainers(limitType, container);

        return valid;
    }

    private void RemoveExpired(RequestRateLimitPerTimeUnit unit)
    {
        var waitingExpiredQueuesIndex = GetWaitingExpiredQueuesIndex(unit);
        var lock_waitingExpiredQueue = _lock_waitingExpiredQueues[waitingExpiredQueuesIndex];
        var waitingExpiredQueue = _waitingExpiredQueues[waitingExpiredQueuesIndex];
        while (waitingExpiredQueue.Count > 0 &&
            waitingExpiredQueue.TryPeek(out var expiredInfo) &&
            expiredInfo.ExpiredTime < _globalTimerService.NowTimeSpan())
        {
            var got = false;
            lock (lock_waitingExpiredQueue)
            {
                if (waitingExpiredQueue.Count > 0 &&
                    waitingExpiredQueue.TryPeek(out expiredInfo) &&
                    expiredInfo.ExpiredTime < _globalTimerService.NowTimeSpan() &&
                    waitingExpiredQueue.TryDequeue(out expiredInfo))
                    got = true;
            }
            if (got)
            {
                var cache = _caches[GetCacheIndex(expiredInfo.type)];
                if (cache.TryGetValue(expiredInfo.key, out var container) &&
                    container.ExpiredTime < _globalTimerService.NowTimeSpan() &&
                    cache.TryRemove(expiredInfo.key, out container) &&
                    container != null)
                {
                    AddRemovingToWaitingSendingStatusContainers(expiredInfo.type, container);
                }
                else if (container != null)
                {
                    container.RemoveExpiredItem(unit);
                    AddUpdatingToWaitingSendingStatusContainers(expiredInfo.type, container);
                }
            }
        }
    }

    private void AddUpdatingToWaitingSendingStatusContainers(RequestRateLimitType limitType,
        RequestRateLimitCacheContainter container)
    {
        var requestRateLimitStatusContainerItems = new List<RequestRateLimitStatusContainerItem>();
        var requestRateLimitStatusContainer = new RequestRateLimitStatusContainer(container.Key,
            Convert(limitType),
            requestRateLimitStatusContainerItems);
        foreach (var item in container.Items)
        {
            if (item == null)
                continue;
            var statusContainerItem = new RequestRateLimitStatusContainerItem(
                Convert(item.Unit), item.Limit, item.Amount);
            requestRateLimitStatusContainerItems.Add(statusContainerItem);
        }

        _requestRateLimitStatusCacheService.SendContainer(
            RequestRateLimitStatusContainerActionType.Update, requestRateLimitStatusContainer);
    }

    private void AddRemovingToWaitingSendingStatusContainers(RequestRateLimitType limitType,
        RequestRateLimitCacheContainter container)
    {
        var requestRateLimitStatusContainer = new RequestRateLimitStatusContainer(container.Key,
                                    Convert(limitType));

        _requestRateLimitStatusCacheService.SendContainer(
            RequestRateLimitStatusContainerActionType.Remove, requestRateLimitStatusContainer);
    }

    private RequestRateLimitStatusContainerType Convert(RequestRateLimitType limitType)
    {
        switch (limitType)
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
    private RequestRateLimitStatusPerTimeUnit Convert(RequestRateLimitPerTimeUnit unit)
    {
        switch (unit)
        {
            case RequestRateLimitPerTimeUnit.Seconds:
                return RequestRateLimitStatusPerTimeUnit.Second;
            case RequestRateLimitPerTimeUnit.Minutes:
                return RequestRateLimitStatusPerTimeUnit.Minute;
            case RequestRateLimitPerTimeUnit.Hours:
                return RequestRateLimitStatusPerTimeUnit.Hour;
            default:
                throw new Exception("Unset Mapping");
        }
    }

    public bool Valid(string httpMethod, MethodInfo actionInfo, TypeInfo controllerInfo,
        string controllerName, string actionName, string remoteIpAddress)
    {
        var valid = true;

        var globalActionRequestRateLimits =
            GetRequestRateLimitAttributes<GlobalRequestRateLimitAttribute>(actionInfo);
        foreach (var globalActionRequestRateLimit in globalActionRequestRateLimits)
        {
            var key = globalActionRequestRateLimit.GetKey(httpMethod, controllerName, actionName);
            var unit = globalActionRequestRateLimit.Unit;
            var limit = globalActionRequestRateLimit.Limit;
            valid &= Valid(RequestRateLimitType.GlobalAction, key, unit, limit);
        }

        var globalControllerRequestRateLimits =
            GetRequestRateLimitAttributes<GlobalRequestRateLimitAttribute>(controllerInfo);
        foreach (var globalControllerRequestRateLimit in globalControllerRequestRateLimits)
        {
            var key = globalControllerRequestRateLimit.GetKey(httpMethod, controllerName, "");
            var unit = globalControllerRequestRateLimit.Unit;
            var limit = globalControllerRequestRateLimit.Limit;
            valid &= Valid(RequestRateLimitType.GlobalController, key, unit, limit);
        }

        var ipActionRequestRateLimits =
            GetRequestRateLimitAttributes<IpRequestRateLimitAttribute>(actionInfo);
        foreach (var ipActionRequestRateLimit in ipActionRequestRateLimits)
        {
            var key = ipActionRequestRateLimit.GetKey(httpMethod, controllerName, actionName, remoteIpAddress);
            var unit = ipActionRequestRateLimit.Unit;
            var limit = ipActionRequestRateLimit.Limit;
            valid &= Valid(RequestRateLimitType.Ip, key, unit, limit);
        }

        var ipControllerRequestRateLimits =
            GetRequestRateLimitAttributes<IpRequestRateLimitAttribute>(controllerInfo);
        foreach (var ipControllerRequestRateLimit in ipControllerRequestRateLimits)
        {
            var key = ipControllerRequestRateLimit.GetKey(httpMethod, controllerName, "", remoteIpAddress);
            var unit = ipControllerRequestRateLimit.Unit;
            var limit = ipControllerRequestRateLimit.Limit;
            valid &= Valid(RequestRateLimitType.Ip, key, unit, limit);
        }

        return valid;
    }

    public bool ValidUser(string httpMethod, MethodInfo actionInfo, TypeInfo controllerInfo,
        string controllerName, string actionName, long userId)
    {
        var valid = true;
        var userActionRequestRateLimits =
            GetRequestRateLimitAttributes<UserRequestRateLimitAttribute>(actionInfo);
        foreach (var userActionRequestRateLimit in userActionRequestRateLimits)
        {
            var key = userActionRequestRateLimit.GetKey(httpMethod, controllerName, actionName, userId);
            var unit = userActionRequestRateLimit.Unit;
            var limit = userActionRequestRateLimit.Limit;
            valid &= Valid(RequestRateLimitType.User, key, unit, limit);
        }

        var userControllerRequestRateLimits =
            GetRequestRateLimitAttributes<UserRequestRateLimitAttribute>(controllerInfo);
        foreach (var userControllerRequestRateLimit in userControllerRequestRateLimits)
        {
            var key = userControllerRequestRateLimit.GetKey(httpMethod, controllerName, "", userId);
            var unit = userControllerRequestRateLimit.Unit;
            var limit = userControllerRequestRateLimit.Limit;
            valid &= Valid(RequestRateLimitType.User, key, unit, limit);
        }

        return valid;
    }

    private IList<T> GetRequestRateLimitAttributes<T>(MemberInfo memberInfo)
        where T : RequestRateLimitAttribute
    {
        return DistinctByMinLimitTimes(memberInfo
                    .GetCustomAttributes(typeof(T), true)
                    .Select(n => (T)n));
    }

    private IList<T> DistinctByMinLimitTimes<T>(IEnumerable<T> items)
        where T : IRequestRateLimitAttribute
    {
        T[] arr = new T[UnitCount];
        foreach (var item in items)
        {
            var idx = (int)item.Unit;
            if (arr[idx] == null || arr[idx].Limit > item.Limit)
                arr[idx] = item;
        }
        return arr.Where(n => n != null).ToArray();
    }

    public void RemoveExpired()
    {
        foreach (var unit in Enum.GetValues<RequestRateLimitPerTimeUnit>())
        {
            RemoveExpired(unit);
        }
    }

    class RequestRateLimitCacheContainter
    {
        private readonly IReadOnlyList<object> _lockItems;
        private readonly RequestRateLimitCacheContainterItem[] _items;
        public IReadOnlyList<RequestRateLimitCacheContainterItem> Items => _items;

        private readonly IReadOnlyList<object> _lockWaitingExpiredItems_queues;
        private readonly IReadOnlyList<ConcurrentQueue<TimeSpan>> _waiting_expiredItems_queues;
        private readonly IGlobalTimerService _globalTimerService;

        public string Key { get; private set; }
        public TimeSpan ExpiredTime { get; private set; }

        public RequestRateLimitCacheContainter(string key,
        IGlobalTimerService globalTimerService)
        {
            Key = key;
            this._globalTimerService = globalTimerService;
            var itemsLength = UnitCount;
            var lockItems = new object[itemsLength];
            var lockWaitingExpiredItems_queues = new object[itemsLength];
            var waiting_expiredItems_queues = new List<ConcurrentQueue<TimeSpan>>();
            for (int i = 0; i < itemsLength; i++)
            {
                lockItems[i] = new();
                lockWaitingExpiredItems_queues[i] = new();
                waiting_expiredItems_queues.Add(new());
            }
            _lockItems = lockItems;
            _items = new RequestRateLimitCacheContainterItem[itemsLength];

            _lockWaitingExpiredItems_queues = lockWaitingExpiredItems_queues;
            _waiting_expiredItems_queues = waiting_expiredItems_queues;

            ExpiredTime = _globalTimerService.NowTimeSpan();
        }

        public bool Valid(RequestRateLimitPerTimeUnit unit, int newLimit, out TimeSpan expiredTime)
        {
            RemoveExpiredItem();

            var valid = true;
            expiredTime = TimeSpan.Zero;
            var idx = (int)unit;
            var lockItem = _lockItems[idx];
            lock (lockItem)
            {
                if (_items[idx] == null)
                {
                    _items[idx] = new(newLimit, unit, _globalTimerService);
                }
                expiredTime = _items[idx].Enter();
                valid = _items[idx].Valid();
            }
            _waiting_expiredItems_queues[(int)unit].Enqueue(expiredTime);
            if (expiredTime > ExpiredTime)
                ExpiredTime = expiredTime;

            return valid;
        }

        public void RemoveExpiredItem(RequestRateLimitPerTimeUnit unit)
        {
            var idx = (int)unit;
            var waiting_expiredItems_queue = _waiting_expiredItems_queues[idx];
            if (waiting_expiredItems_queue == null)
                return;
            while (waiting_expiredItems_queue.Count > 0 &&
                waiting_expiredItems_queue.TryPeek(out var expiredTime) &&
                expiredTime < _globalTimerService.NowTimeSpan())
            {
                var got = false;
                lock (_lockWaitingExpiredItems_queues[idx])
                {
                    if (waiting_expiredItems_queue.Count > 0 &&
                        waiting_expiredItems_queue.TryPeek(out expiredTime) &&
                        expiredTime < _globalTimerService.NowTimeSpan() &&
                        waiting_expiredItems_queue.TryDequeue(out expiredTime))
                        got = true;
                }
                if (got)
                {
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

        public void RemoveExpiredItem()
        {
            foreach (var unit in Enum.GetValues<RequestRateLimitPerTimeUnit>())
            {
                RemoveExpiredItem(unit);
            }
        }
    }

    class RequestRateLimitCacheContainterItem
    {
        private readonly IGlobalTimerService _globalTimerService;

        public int Limit { get; private set; }
        public int Amount { get; private set; }
        public RequestRateLimitPerTimeUnit Unit { get; private set; }
        public TimeSpan ExpiredTime { get; private set; }

        public RequestRateLimitCacheContainterItem(int limit, RequestRateLimitPerTimeUnit unit,
        IGlobalTimerService globalTimerService)
        {
            Limit = limit;
            Amount = 0;
            Unit = unit;
            this._globalTimerService = globalTimerService;
            ExpiredTime = GetExpiredTime(unit);
        }

        public TimeSpan Enter()
        {
            Amount++;
            var expiredTime = GetExpiredTime(Unit);
            ExpiredTime = expiredTime;
            return expiredTime;
        }

        public bool Valid()
        {
            return Amount <= Limit;
        }

        public void Leave()
        {
            Amount--;
        }

        private TimeSpan GetExpiredTime(RequestRateLimitPerTimeUnit unit)
        {
            var expiredMilliseconds = 0;
            if (unit == RequestRateLimitPerTimeUnit.Seconds)
                expiredMilliseconds = 1000;
            else if (unit == RequestRateLimitPerTimeUnit.Minutes)
                expiredMilliseconds = 60 * 1000;
            else if (unit == RequestRateLimitPerTimeUnit.Hours)
                expiredMilliseconds = 3600 * 1000;
            else
                throw new Exception("Miss mapping enum");
            return _globalTimerService.AddMillisecondsForNowTimeSpan(expiredMilliseconds);
        }
    }
}