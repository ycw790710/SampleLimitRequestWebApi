namespace RequestRateLimit.Components;

public class UserRequestRateLimitPerTimeUnit
{
    public static UserRequestRateLimitPerTimeUnit Seconds = new(0, 5, RequestRateLimitPerTimeUnit.Seconds);
    public static UserRequestRateLimitPerTimeUnit Minutes = new(1, 40, RequestRateLimitPerTimeUnit.Minutes);
    public static UserRequestRateLimitPerTimeUnit Hours = new(2, 1600, RequestRateLimitPerTimeUnit.Hours);

    public int Id { get; }
    public int MinTimes { get; }
    public int MaxTimes { get; }
    public RequestRateLimitPerTimeUnit PerTimeUnit { get; }

    public UserRequestRateLimitPerTimeUnit(int id, int maxTimes, RequestRateLimitPerTimeUnit perTimeUnit)
    {
        Id = id;
        MinTimes = 1;
        MaxTimes = maxTimes;
        PerTimeUnit = perTimeUnit;
    }

    public static UserRequestRateLimitPerTimeUnit Convert(RequestRateLimitPerTimeUnit requestRateLimitPerTimeUnit)
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