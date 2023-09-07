namespace RequestRateLimit.Dtos;

public class RequestRateLimitStatus
{
    public DateTime updatedTime { get; private set; }
    public IReadOnlyDictionary<int, IReadOnlyCollection<RequestRateLimitStatusContainer>> typesContainers { get; private set; }

    public RequestRateLimitStatus(DateTime updatedTime,
        IReadOnlyDictionary<int, IReadOnlyCollection<RequestRateLimitStatusContainer>> typesContainers)
    {
        this.updatedTime = updatedTime;
        this.typesContainers = typesContainers;
    }

}