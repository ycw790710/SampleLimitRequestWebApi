namespace RequestRateLimit.Components;

public abstract class RequestRateLimitAttribute : Attribute, IRequestRateLimitAttribute
{
    public int Limit { get; }
    public RequestRateLimitPerTimeUnit Unit { get; }

    public RequestRateLimitAttribute(int limit, RequestRateLimitPerTimeUnit unit)
    {
        var expectedTimes = GetExpectedTimes(unit);
        if (limit < expectedTimes.MinTimes || limit > expectedTimes.MaxTimes)
            throw new ArgumentException(
                $"Invalid {nameof(limit)}, min {expectedTimes.MinTimes}, max {expectedTimes.MaxTimes}");

        Limit = limit;
        Unit = unit;
    }

    protected string GetPathKey(string httpMethod, string controllerName, string actionName)
    {
        return $"[{httpMethod}] [{controllerName}/{actionName}]";
    }

    protected abstract (int MinTimes, int MaxTimes) GetExpectedTimes(RequestRateLimitPerTimeUnit unit);
}