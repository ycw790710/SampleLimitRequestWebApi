namespace RequestRateLimit.Components;

public abstract class RequestRateLimitAttribute : Attribute, IRequestRateLimitAttribute
{
    public int LimitTimes { get; }
    public RequestRateLimitPerTimeUnit PerTimeUnit { get; }

    public RequestRateLimitAttribute(int limitTimes, RequestRateLimitPerTimeUnit inputPerTimeUnit)
    {
        var expectedTimes = GetExpectedTimes(inputPerTimeUnit);
        if (limitTimes < expectedTimes.MinTimes || limitTimes > expectedTimes.MaxTimes)
            throw new ArgumentException(
                $"Invalid {nameof(limitTimes)}, min {expectedTimes.MinTimes}, max {expectedTimes.MaxTimes}");

        LimitTimes = limitTimes;
        PerTimeUnit = inputPerTimeUnit;
    }

    protected abstract (int MinTimes, int MaxTimes) GetExpectedTimes(RequestRateLimitPerTimeUnit inputPerTimeUnit);
}