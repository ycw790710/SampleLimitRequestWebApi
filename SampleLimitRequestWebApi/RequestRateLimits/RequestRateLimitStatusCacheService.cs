using SampleLimitRequestWebApi.RequestRateLimits.Dtos;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace SampleLimitRequestWebApi.RequestRateLimits;

public class RequestRateLimitStatusCacheService : IRequestRateLimitStatusCacheService
{
    private readonly ConcurrentQueue<(RequestRateLimitStatusContainerActionType ActionType, RequestRateLimitStatusContainer Container)> _waitingStatusContainers;

    private IReadOnlyList<RequestRateLimitStatusContainerTypeInfo> _containerTypeInfos;
    private IReadOnlyList<RequestRateLimitStatusPerTimeUnitInfo> _perTimeUnitInfos;
    private readonly object _lockStatusContainerStore;
    private readonly IReadOnlyDictionary<RequestRateLimitStatusContainerType, Dictionary<string, RequestRateLimitStatusContainer>> _statusContainerStore;

    private string? _statusJson;

    public RequestRateLimitStatusCacheService()
    {
        _waitingStatusContainers = new();

        _containerTypeInfos = GetContainerTypeInfos();
        _perTimeUnitInfos = GetPerTimeUnitInfos();
        _lockStatusContainerStore = new();
        _statusContainerStore = GetStatusContainerStore();

        _statusJson = null;
    }

    private IReadOnlyList<RequestRateLimitStatusContainerTypeInfo> GetContainerTypeInfos()
    {
        List<RequestRateLimitStatusContainerTypeInfo> list = new()
        {
            new(RequestRateLimitStatusContainerType.GlobalController, "Global Controller速率限制", "向Controller的所有Actions的所有Request的速率限制"),
            new(RequestRateLimitStatusContainerType.GlobalAction, "Global Action速率限制", "向Action的所有Request的速率限制"),
            new(RequestRateLimitStatusContainerType.Ip, "Ip速率限制", ""),
            new(RequestRateLimitStatusContainerType.User, "User速率限制", "")
        };
        return list;
    }

    private IReadOnlyList<RequestRateLimitStatusPerTimeUnitInfo> GetPerTimeUnitInfos()
    {
        List<RequestRateLimitStatusPerTimeUnitInfo> list = new();
        foreach (var perTimeUnit in Enum.GetValues<RequestRateLimitStatusPerTimeUnit>())
            list.Add(new RequestRateLimitStatusPerTimeUnitInfo(perTimeUnit, perTimeUnit.ToString()));
        return list;
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
        if (actionType == RequestRateLimitStatusContainerActionType.Remove)
            Debug.WriteLine("SendContainer Remove");
        _waitingStatusContainers.Enqueue((actionType, container));
    }

    public void UpdateStatuses()
    {
        var updatedTime = DateTime.UtcNow;

        lock (_lockStatusContainerStore)
        {
            while (_waitingStatusContainers.TryPeek(out var info) && info.Container.UpdatedTime <= updatedTime &&
                _waitingStatusContainers.TryDequeue(out info))
            {
                if (info.ActionType == RequestRateLimitStatusContainerActionType.Update)
                    _statusContainerStore[info.Container.Type][info.Container.Key] = info.Container;
                if (info.ActionType == RequestRateLimitStatusContainerActionType.Remove)
                    _statusContainerStore[info.Container.Type].Remove(info.Container.Key);
            }

            var refContainerTypesItemContainers =
                new Dictionary<RequestRateLimitStatusContainerType, IReadOnlyCollection<RequestRateLimitStatusContainer>>();
            foreach (var statusCache in _statusContainerStore)
                refContainerTypesItemContainers[statusCache.Key] = statusCache.Value.Values;
            var requestRateLimitStatus = new RequestRateLimitStatus(_containerTypeInfos,
                _perTimeUnitInfos, updatedTime, refContainerTypesItemContainers);

            var statusJson = JsonSerializer.Serialize(requestRateLimitStatus);
            _statusJson = statusJson;
        }
    }

    public string? GetStatusJson()
    {
        return _statusJson;
    }

}