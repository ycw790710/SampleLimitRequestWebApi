namespace RequestRateLimit.Services;

public class RequestRateLimitStatusCacheService : IRequestRateLimitStatusCacheService
{
    private readonly ConcurrentQueue<(RequestRateLimitStatusContainerActionType ActionType, RequestRateLimitStatusContainer Container)> _waitingStatusContainers;

    private readonly object _lockStatusContainerStore;
    private readonly IReadOnlyDictionary<RequestRateLimitStatusContainerType, Dictionary<string, RequestRateLimitStatusContainer>> _statusContainerStore;

    private string? _statusJson;
    private byte[] _statusJsonBytes;

    private byte[] _statusInfoJsonBytes;

    public RequestRateLimitStatusCacheService()
    {
        _waitingStatusContainers = new();

        _lockStatusContainerStore = new();
        _statusContainerStore = GetStatusContainerStore();

        _statusJson = null;
        _statusJsonBytes = Array.Empty<byte>();
        _statusInfoJsonBytes = Array.Empty<byte>();
        SetStatusJsonBytes();
    }

    private void SetStatusJsonBytes()
    {
        var requestRateLimitStatusInfo =
            new RequestRateLimitStatusInfo(GetContainerTypeInfos(),
            GetPerTimeUnitInfos());
        var statusInfoJson = JsonSerializer.Serialize(requestRateLimitStatusInfo);
        _statusInfoJsonBytes = statusInfoJson != null ? Encoding.UTF8.GetBytes(statusInfoJson) : Array.Empty<byte>();
    }

    private IReadOnlyList<RequestRateLimitStatusContainerTypeInfo> GetContainerTypeInfos()
    {
        List<RequestRateLimitStatusContainerTypeInfo> list = new()
        {
            new(RequestRateLimitStatusContainerType.GlobalController, "Global Controller速率限制", "包含Controller的全部Action的全部Requests"),
            new(RequestRateLimitStatusContainerType.GlobalAction, "Global Action速率限制", "包含Action的全部Requests"),
            new(RequestRateLimitStatusContainerType.Ip, "Ip速率限制", ""),
            new(RequestRateLimitStatusContainerType.User, "User速率限制", "")
        };
        return list;
    }

    private IReadOnlyDictionary<int, RequestRateLimitStatusPerTimeUnitInfo> GetPerTimeUnitInfos()
    {
        Dictionary<int, RequestRateLimitStatusPerTimeUnitInfo> dict = new();
        foreach (var perTimeUnit in Enum.GetValues<RequestRateLimitStatusPerTimeUnit>())
            dict.Add((int)perTimeUnit, new RequestRateLimitStatusPerTimeUnitInfo(perTimeUnit, perTimeUnit.ToString()));
        return dict;
    }

    private Dictionary<RequestRateLimitStatusContainerType, Dictionary<string, RequestRateLimitStatusContainer>> GetStatusContainerStore()
    {
        var statuses = new Dictionary<RequestRateLimitStatusContainerType, Dictionary<string, RequestRateLimitStatusContainer>>();
        foreach (var statusContainerType in Enum.GetValues<RequestRateLimitStatusContainerType>())
            statuses.Add(statusContainerType, new());
        return statuses;
    }

    public void SendContainer(RequestRateLimitStatusContainerActionType actionType, RequestRateLimitStatusContainer container)
    {
        _waitingStatusContainers.Enqueue((actionType, container));
    }

    public void UpdateStatuses()
    {
        var updatedTime = DateTime.UtcNow;

        lock (_lockStatusContainerStore)
        {
            while (_waitingStatusContainers.TryPeek(out var info) && info.Container.updatedTime <= updatedTime &&
                _waitingStatusContainers.TryDequeue(out info))
            {
                var preContainerUpdatedTime = DateTime.MinValue;
                if (_statusContainerStore[info.Container.type].ContainsKey(info.Container.key) &&
                    _statusContainerStore[info.Container.type][info.Container.key] != null)
                    preContainerUpdatedTime = _statusContainerStore[info.Container.type][info.Container.key].updatedTime;
                if (info.ActionType == RequestRateLimitStatusContainerActionType.Update &&
                    info.Container.updatedTime > preContainerUpdatedTime)
                    _statusContainerStore[info.Container.type][info.Container.key] = info.Container;
                if (info.ActionType == RequestRateLimitStatusContainerActionType.Remove &&
                    info.Container.updatedTime > preContainerUpdatedTime)
                    _statusContainerStore[info.Container.type].Remove(info.Container.key);
            }

            var refContainerTypesItemContainers =
                new Dictionary<int, IReadOnlyCollection<RequestRateLimitStatusContainer>>();
            foreach (var statusCache in _statusContainerStore)
                refContainerTypesItemContainers[(int)statusCache.Key] = statusCache.Value.Values;
            var requestRateLimitStatus = new RequestRateLimitStatus(
                updatedTime, refContainerTypesItemContainers);

            var statusJson = JsonSerializer.Serialize(requestRateLimitStatus);
            _statusJson = statusJson;
            _statusJsonBytes = statusJson != null ? Encoding.UTF8.GetBytes(statusJson) : Array.Empty<byte>();
        }
    }

    public string? GetStatusJson()
    {
        return _statusJson;
    }

    public byte[] GetStatusJsonBytes()
    {
        return _statusJsonBytes;
    }

    public byte[] GetStatusInfoJsonBytes()
    {
        return _statusInfoJsonBytes;
    }
}