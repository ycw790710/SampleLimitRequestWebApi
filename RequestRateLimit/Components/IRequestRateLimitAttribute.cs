namespace RequestRateLimit.Components;

public interface IRequestRateLimitAttribute
{
    int LimitTimes { get; }
    RequestRateLimitPerTimeUnit PerTimeUnit { get; }
}