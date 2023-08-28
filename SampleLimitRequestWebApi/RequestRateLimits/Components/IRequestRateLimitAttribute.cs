namespace SampleLimitRequestWebApi.RequestRateLimits.Components;

public interface IRequestRateLimitAttribute
{
    int LimitTimes { get; }
    RequestRateLimitPerTimeUnit PerTimeUnit { get; }
}