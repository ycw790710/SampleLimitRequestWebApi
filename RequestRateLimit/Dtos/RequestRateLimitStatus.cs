namespace RequestRateLimit.Dtos;

public class RequestRateLimitStatus
{
    public DateTime updatedTime { get; private set; }
    public IReadOnlyDictionary<int, IReadOnlyCollection<RequestRateLimitStatusContainer>> containerTypesContainers { get; private set; }

    public RequestRateLimitStatus(DateTime updatedTime,
        IReadOnlyDictionary<int, IReadOnlyCollection<RequestRateLimitStatusContainer>> containerTypesContainers)
    {
        this.updatedTime = updatedTime;
        this.containerTypesContainers = containerTypesContainers;
    }

}