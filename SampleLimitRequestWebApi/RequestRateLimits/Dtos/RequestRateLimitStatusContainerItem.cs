namespace SampleLimitRequestWebApi.RequestRateLimits.Dtos;

public class RequestRateLimitStatusContainerItem
{
    public RequestRateLimitStatusPerTimeUnit PerTimeUnit { get; private set; }
    public int LimitTimes { get; private set; }
    public int Capacity { get; private set; }

    public RequestRateLimitStatusContainerItem(RequestRateLimitStatusPerTimeUnit perTimeUnit, int limitTimes, int capacity)
    {
        PerTimeUnit = perTimeUnit;
        LimitTimes = limitTimes;
        Capacity = capacity;
    }
}