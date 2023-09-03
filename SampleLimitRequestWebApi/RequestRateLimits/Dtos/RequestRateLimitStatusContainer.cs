namespace SampleLimitRequestWebApi.RequestRateLimits.Dtos;

public class RequestRateLimitStatusContainer
{
    public string key { get; }
    public DateTime updatedTime { get; }
    public RequestRateLimitStatusContainerType type { get; }
    public IReadOnlyList<RequestRateLimitStatusContainerItem>? items { get; }

    public RequestRateLimitStatusContainer(string key, RequestRateLimitStatusContainerType type,
        IReadOnlyList<RequestRateLimitStatusContainerItem>? items = null,
        DateTime? updatedTime = null)
    {
        this.key = key;
        this.updatedTime = updatedTime.HasValue ? updatedTime.Value : DateTime.UtcNow;
        this.type = type;
        this.items = items;
    }
}