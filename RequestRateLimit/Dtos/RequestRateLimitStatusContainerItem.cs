namespace RequestRateLimit.Dtos;


public class RequestRateLimitStatusContainerItem
{
    public RequestRateLimitStatusPerTimeUnit perTimeUnit { get; private set; }
    public int limitTimes { get; private set; }
    public int capacity { get; private set; }

    public RequestRateLimitStatusContainerItem(RequestRateLimitStatusPerTimeUnit perTimeUnit, int limitTimes, int capacity)
    {
        this.perTimeUnit = perTimeUnit;
        this.limitTimes = limitTimes;
        this.capacity = capacity;
    }
}