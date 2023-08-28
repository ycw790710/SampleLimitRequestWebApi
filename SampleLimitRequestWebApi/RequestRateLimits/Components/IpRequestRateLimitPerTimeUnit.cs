namespace SampleLimitRequestWebApi.RequestRateLimits.Components;

public class IpRequestRateLimitPerTimeUnit
{
    public static IpRequestRateLimitPerTimeUnit Seconds = new(0, 500, RequestRateLimitPerTimeUnit.Seconds);
    public static IpRequestRateLimitPerTimeUnit Minutes = new(1, 4000, RequestRateLimitPerTimeUnit.Minutes);
    public static IpRequestRateLimitPerTimeUnit Hours = new(2, 160000, RequestRateLimitPerTimeUnit.Hours);

    public int Id { get; }
    public int MinTimes { get; }
    public int MaxTimes { get; }
    public RequestRateLimitPerTimeUnit PerTimeUnit { get; }

    public IpRequestRateLimitPerTimeUnit(int id, int maxTimes, RequestRateLimitPerTimeUnit perTimeUnit)
    {
        Id = id;
        MinTimes = 1;
        MaxTimes = maxTimes;
        PerTimeUnit = perTimeUnit;
    }

    public static IpRequestRateLimitPerTimeUnit Convert(RequestRateLimitPerTimeUnit requestRateLimitPerTimeUnit)
    {
        if (requestRateLimitPerTimeUnit == RequestRateLimitPerTimeUnit.Seconds)
            return Seconds;
        if (requestRateLimitPerTimeUnit == RequestRateLimitPerTimeUnit.Minutes)
            return Minutes;
        if (requestRateLimitPerTimeUnit == RequestRateLimitPerTimeUnit.Hours)
            return Hours;
        throw new Exception("Miss Type");
    }
}