namespace RequestRateLimit.Dtos;

public class RequestRateLimitStatusContainer
{
    public string key { get; }
    public DateTime updatedTime { get; }
    public RequestRateLimitStatusContainerType type { get; }
    public IReadOnlyList<RequestRateLimitStatusContainerItem>? items { get; }

    public RequestRateLimitStatusContainer(string key,
        DateTime updatedTime, RequestRateLimitStatusContainerType type,
        IReadOnlyList<RequestRateLimitStatusContainerItem>? items)
    {
        this.key = key;
        this.updatedTime = updatedTime;
        this.type = type;
        this.items = items;
    }

    internal RequestRateLimitStatusContainer(string key, RequestRateLimitStatusContainerType type,
        IReadOnlyList<RequestRateLimitStatusContainerItem> items)
    {
        this.key = key;
        this.type = type;
        this.updatedTime = DateTime.UtcNow;
        this.items = items;
    }

    internal RequestRateLimitStatusContainer(string key, RequestRateLimitStatusContainerType type)
    {
        this.key = key;
        this.type = type;
        this.updatedTime = DateTime.UtcNow;
        this.items = null;
    }
}