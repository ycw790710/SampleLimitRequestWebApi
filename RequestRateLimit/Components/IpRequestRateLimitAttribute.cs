namespace RequestRateLimit.Components;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class IpRequestRateLimitAttribute : RequestRateLimitAttribute
{
    public IpRequestRateLimitAttribute(int limitTimes, RequestRateLimitPerTimeUnit inputPerTimeUnit)
        : base(limitTimes, inputPerTimeUnit)
    {
    }

    public string GetKey(string httpMethod, string controllerName, string actionName, IPAddress iPAddress)
    {
        return $"[{iPAddress}] [{httpMethod}] [{controllerName}/{actionName}]";
    }

    protected override (int MinTimes, int MaxTimes) GetExpectedTimes(RequestRateLimitPerTimeUnit inputPerTimeUnit)
    {
        switch (inputPerTimeUnit)
        {
            case RequestRateLimitPerTimeUnit.Seconds:
                return (1, 2000);
            case RequestRateLimitPerTimeUnit.Minutes:
                return (1, 120000);
            case RequestRateLimitPerTimeUnit.Hours:
                return (1, 7200000);
            default:
                throw new Exception("Miss Type");
        }
    }
}
