using SampleLimitRequestWebApi.RequestRateLimits.Dtos;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SampleLimitRequestWebApi.RequestRateLimits;

public class RequestRateLimitStatusCacheService : IRequestRateLimitStatusCacheService
{
    private readonly ConcurrentQueue<(RequestRateLimitStatusContainerActionType ActionType, RequestRateLimitStatusContainer Container)> _waitingStatusContainers;

    private IReadOnlyList<RequestRateLimitStatusContainerTypeInfo> _containerTypeInfos;
    private IReadOnlyList<RequestRateLimitStatusPerTimeUnitInfo> _perTimeUnitInfos;
    private readonly IReadOnlyDictionary<RequestRateLimitStatusContainerType, Dictionary<string, RequestRateLimitStatusContainer>> _statusContainerStore;


    private RequestRateLimitStatus? _status;
    private string? _statusJson;

    public RequestRateLimitStatusCacheService()
    {
        _waitingStatusContainers = new();

        _containerTypeInfos = GetContainerTypeInfos();
        _perTimeUnitInfos = GetPerTimeUnitInfos();
        _statusContainerStore = GetStatusContainerStore();

        _status = null;
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

        while (_waitingStatusContainers.TryPeek(out var info) && info.Container.UpdatedTime <= updatedTime &&
            _waitingStatusContainers.TryDequeue(out info))
        {
            if (info.ActionType == RequestRateLimitStatusContainerActionType.Update)
                _statusContainerStore[info.Container.Type][info.Container.Key] = info.Container;
            if (info.ActionType == RequestRateLimitStatusContainerActionType.Remove)
                _statusContainerStore[info.Container.Type].Remove(info.Container.Key);
        }

        var cloneContainerTypesItemContainers =
            new Dictionary<RequestRateLimitStatusContainerType, IReadOnlyList<RequestRateLimitStatusContainer>>();
        foreach (var statusCache in _statusContainerStore)
        {
            List<RequestRateLimitStatusContainer> requestRateLimitStatusContainers = new();
            cloneContainerTypesItemContainers[statusCache.Key] = requestRateLimitStatusContainers;
            foreach (var kvp in statusCache.Value)
            {
                RequestRateLimitStatusContainer container = kvp.Value;
                if (container == null || container.Items == null)
                    continue;

                List<RequestRateLimitStatusContainerItem> cloneItems = new();
                foreach (var item in container.Items)
                {
                    if (item == null)
                        continue;

                    RequestRateLimitStatusContainerItem cloneItem = new(
                        item.PerTimeUnit, item.LimitTimes, item.Capacity);
                    cloneItems.Add(cloneItem);
                }
                RequestRateLimitStatusContainer cloneContainer = new(
                    container.Key, container.Type, cloneItems, container.UpdatedTime);
                requestRateLimitStatusContainers.Add(cloneContainer);
            }
        }

        RequestRateLimitStatus requestRateLimitStatus = new(_containerTypeInfos,
            _perTimeUnitInfos, updatedTime, cloneContainerTypesItemContainers);
        _status = requestRateLimitStatus;

        _statusJson = JsonSerializer.Serialize(requestRateLimitStatus);
    }

    public RequestRateLimitStatus? GetStatus()
    {
        return _status;
    }

    public string? GetStatusJson()
    {
        return _statusJson;
    }

}