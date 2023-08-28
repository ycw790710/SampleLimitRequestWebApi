namespace SampleLimitRequestWebApi.RequestRateLimits.Components;

public class GlobalRequestRateLimitPerTimeUnit
{
    public static GlobalRequestRateLimitPerTimeUnit Seconds = new(0, 5000, RequestRateLimitPerTimeUnit.Seconds);
    public static GlobalRequestRateLimitPerTimeUnit Minutes = new(1, 40000, RequestRateLimitPerTimeUnit.Minutes);
    public static GlobalRequestRateLimitPerTimeUnit Hours = new(2, 1600000, RequestRateLimitPerTimeUnit.Hours);

    public int Id { get; }
    public int MinTimes { get; }
    public int MaxTimes { get; }
    public RequestRateLimitPerTimeUnit PerTimeUnit { get; }

    public GlobalRequestRateLimitPerTimeUnit(int id, int maxTimes, RequestRateLimitPerTimeUnit perTimeUnit)
    {
        Id = id;
        MinTimes = 1;
        MaxTimes = maxTimes;
        PerTimeUnit = perTimeUnit;
    }

    public static GlobalRequestRateLimitPerTimeUnit Convert(RequestRateLimitPerTimeUnit requestRateLimitPerTimeUnit)
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