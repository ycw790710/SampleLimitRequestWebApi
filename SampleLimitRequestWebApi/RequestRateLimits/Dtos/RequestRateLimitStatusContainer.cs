namespace SampleLimitRequestWebApi.RequestRateLimits.Dtos;

public class RequestRateLimitStatusContainer
{
    public string Key { get; }
    public DateTime UpdatedTime { get; }
    public RequestRateLimitStatusContainerType Type { get; }
    public IReadOnlyList<RequestRateLimitStatusContainerItem>? Items { get; }

    public RequestRateLimitStatusContainer(string key, RequestRateLimitStatusContainerType type,
        IReadOnlyList<RequestRateLimitStatusContainerItem>? items = null,
        DateTime? updatedTime = null)
    {
        Key = key;
        UpdatedTime = updatedTime.HasValue ? updatedTime.Value : DateTime.UtcNow;
        Type = type;
        Items = items;
    }
}