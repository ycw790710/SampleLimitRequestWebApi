namespace RequestRateLimit.Components;

public interface IRequestRateLimitAttribute
{
    int Limit { get; }
    RequestRateLimitPerTimeUnit Unit { get; }
}