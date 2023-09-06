
namespace RequestRateLimit.Services;

public class RequestRateLimitCacheService : IRequestRateLimitCacheService
{
    private static int TypeLength { get; }
        = Enum.GetValues(typeof(RequestRateLimitType)).Length;
    private static int RequestRateLimitPerTimeUnitCount { get; }
        = Enum.GetValues(typeof(RequestRateLimitPerTimeUnit)).Length;

    private readonly IReadOnlyList<ConcurrentDictionary<string, RequestRateLimitCacheContainter>> _caches;
    private readonly IReadOnlyList<object> _lock_waitingExpiredQueues;
    private readonly IReadOnlyList<ConcurrentQueue<(RequestRateLimitType type, string key, TimeSpan ExpiredTime)>> _waitingExpiredQueues;

    private readonly IRequestRateLimitStatusCacheService _requestRateLimitStatusCacheService;

    public RequestRateLimitCacheService(
        IRequestRateLimitStatusCacheService requestRateLimitStatusCacheService)
    {
        var caches = new ConcurrentDictionary<string, RequestRateLimitCacheContainter>[TypeLength];
        var lock_waitingExpiredQueues = new object[TypeLength];
        var waitingExpiredQueues = new ConcurrentQueue<(RequestRateLimitType type, string key, TimeSpan ExpiredTime)>[TypeLength];
        for (int i = 0; i < TypeLength; i++)
        {
            caches[i] = new();
            lock_waitingExpiredQueues[i] = new();
        }
        for (int i = 0; i < RequestRateLimitPerTimeUnitCount; i++)
        {
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

    private int GetWaitingExpiredQueuesIndex(RequestRateLimitPerTimeUnit requestRateLimitPerTimeUnit)
    {
        return (int)requestRateLimitPerTimeUnit;
    }

    private bool Valid(RequestRateLimitType requestRateLimitType,
        string key, RequestRateLimitPerTimeUnit perTimeUnit, int limitTimes)
    {
        RemoveExpired(perTimeUnit);

        var cache = _caches[GetCacheIndex(requestRateLimitType)];

        if (!cache.ContainsKey(key))
            cache.TryAdd(key, new(key));
        var container = cache[key];
        var valid = container.Valid(perTimeUnit, limitTimes, out var expiredTime);

        var waitingExpiredQueue = _waitingExpiredQueues[GetWaitingExpiredQueuesIndex(perTimeUnit)];
        waitingExpiredQueue.Enqueue((requestRateLimitType, key, expiredTime));

        AddUpdatingToWaitingSendingStatusContainers(requestRateLimitType, container);

        return valid;
    }

    private void RemoveExpired(RequestRateLimitPerTimeUnit perTimeUnit)
    {
        var waitingExpiredQueuesIndex = GetWaitingExpiredQueuesIndex(perTimeUnit);
        var lock_waitingExpiredQueue = _lock_waitingExpiredQueues[waitingExpiredQueuesIndex];
        var waitingExpiredQueue = _waitingExpiredQueues[waitingExpiredQueuesIndex];
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
                var cache = _caches[GetCacheIndex(expiredInfo.type)];
                if (cache.TryGetValue(expiredInfo.key, out var container) &&
                    container.ExpiredTime < GlobalTimer.NowTimeSpan() &&
                    cache.TryRemove(expiredInfo.key, out container) &&
                    container != null)
                {
                    AddRemovingToWaitingSendingStatusContainers(expiredInfo.type, container);
                }
                else if (container != null)
                {
                    container.RemoveExpiredItem(perTimeUnit);
                    AddUpdatingToWaitingSendingStatusContainers(expiredInfo.type, container);
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

    public bool Valid(string httpMethod, MethodInfo methodInfo, TypeInfo typeInfo,
        string controllerName, string actionName, IPAddress remoteIpAddress)
    {
        var valid = true;

        var globalActionRequestRateLimits =
            DistinctByMinLimitTimes(methodInfo
            .GetCustomAttributes(typeof(GlobalRequestRateLimitAttribute), true)
            .Select(n => (GlobalRequestRateLimitAttribute)n));
        foreach (var globalActionRequestRateLimit in globalActionRequestRateLimits)
        {
            var key = globalActionRequestRateLimit.GetKey(httpMethod, controllerName, actionName);
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
            var key = globalControllerRequestRateLimit.GetKey(httpMethod, controllerName, "");
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
            var key = ipActionRequestRateLimit.GetKey(httpMethod, controllerName, actionName, remoteIpAddress);
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
            var key = ipControllerRequestRateLimit.GetKey(httpMethod, controllerName, "", remoteIpAddress);
            var perTimeUnit = ipControllerRequestRateLimit.PerTimeUnit;
            var limitTimes = ipControllerRequestRateLimit.LimitTimes;
            valid &= Valid(RequestRateLimitType.Ip, key, perTimeUnit, limitTimes);
        }

        return valid;
    }

    public bool ValidUser(string httpMethod, MethodInfo methodInfo, TypeInfo typeInfo,
        string controllerName, string actionName, long userId)
    {
        var valid = true;

        var userActionRequestRateLimits =
            DistinctByMinLimitTimes(methodInfo
            .GetCustomAttributes(typeof(UserRequestRateLimitAttribute), true)
            .Select(n => (UserRequestRateLimitAttribute)n));
        foreach (var userActionRequestRateLimit in userActionRequestRateLimits)
        {
            var key = userActionRequestRateLimit.GetKey(httpMethod, controllerName, actionName, userId);
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
            var key = userControllerRequestRateLimit.GetKey(httpMethod, controllerName, "", userId);
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
        foreach (var perTimeUnit in Enum.GetValues<RequestRateLimitPerTimeUnit>())
        {
            RemoveExpired(perTimeUnit);
        }
    }

    class RequestRateLimitCacheContainter
    {
        private readonly IReadOnlyList<object> _lockItems;
        private readonly RequestRateLimitCacheContainterItem[] _items;
        public IReadOnlyList<RequestRateLimitCacheContainterItem> Items => _items;

        private readonly IReadOnlyList<object> _lockWaitingExpiredItems_queues;
        private readonly IReadOnlyList<ConcurrentQueue<TimeSpan>> _waiting_expiredItems_queues;

        public string Key { get; private set; }
        public TimeSpan ExpiredTime { get; private set; }

        public RequestRateLimitCacheContainter(string key)
        {
            Key = key;

            var itemsLength = RequestRateLimitPerTimeUnitCount;
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

            ExpiredTime = GlobalTimer.NowTimeSpan();
        }

        public bool Valid(RequestRateLimitPerTimeUnit perTimeUnit, int newLimitTimes, out TimeSpan expiredTime)
        {
            RemoveExpiredItem();

            var valid = true;
            expiredTime = TimeSpan.Zero;
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
            }
            _waiting_expiredItems_queues[(int)perTimeUnit].Enqueue(expiredTime);
            if (expiredTime > ExpiredTime)
                ExpiredTime = expiredTime;

            return valid;
        }

        public void RemoveExpiredItem(RequestRateLimitPerTimeUnit perTimeUnit)
        {
            var idx = (int)perTimeUnit;
            var waiting_expiredItems_queue = _waiting_expiredItems_queues[idx];
            if (waiting_expiredItems_queue == null)
                return;
            while (waiting_expiredItems_queue.Count > 0 &&
                waiting_expiredItems_queue.TryPeek(out var expiredTime) &&
                expiredTime < GlobalTimer.NowTimeSpan())
            {
                var got = false;
                lock (_lockWaitingExpiredItems_queues[idx])
                {
                    if (waiting_expiredItems_queue.Count > 0 &&
                        waiting_expiredItems_queue.TryPeek(out expiredTime) &&
                        expiredTime < GlobalTimer.NowTimeSpan() &&
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
            foreach (var perTimeUnit in Enum.GetValues<RequestRateLimitPerTimeUnit>())
            {
                RemoveExpiredItem(perTimeUnit);
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