namespace RequestRateLimit.Components;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class IpRequestRateLimitAttribute : RequestRateLimitAttribute
{
    public IpRequestRateLimitAttribute(int limitTimes, RequestRateLimitPerTimeUnit inputPerTimeUnit)
        : base(limitTimes, inputPerTimeUnit)
    {
    }

    public string GetKey(string controllerName, string actionName, IPAddress iPAddress)
    {
        return $"[{iPAddress}] [{controllerName}/{actionName}]";
    }

    protected override (int MinTimes, int MaxTimes) GetExpectedTimes(RequestRateLimitPerTimeUnit inputPerTimeUnit)
    {
        switch (inputPerTimeUnit)
        {
            case RequestRateLimitPerTimeUnit.Seconds:
                return (1, 500);
            case RequestRateLimitPerTimeUnit.Minutes:
                return (1, 4000);
            case RequestRateLimitPerTimeUnit.Hours:
                return (1, 160000);
            default:
                throw new Exception("Miss Type");
        }
    }
}
