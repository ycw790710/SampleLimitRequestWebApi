namespace RequestRateLimit.Components;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class UserRequestRateLimitAttribute : RequestRateLimitAttribute
{

    public UserRequestRateLimitAttribute(int limitTimes, RequestRateLimitPerTimeUnit inputPerTimeUnit)
        : base(limitTimes, inputPerTimeUnit)
    {
    }

    public string GetKey(string controllerName, string actionName, long userId)
    {
        return $"[{userId}] [{controllerName}/{actionName}]";
    }

    protected override (int MinTimes, int MaxTimes) GetExpectedTimes(RequestRateLimitPerTimeUnit inputPerTimeUnit)
    {
        switch (inputPerTimeUnit)
        {
            case RequestRateLimitPerTimeUnit.Seconds:
                return (1, 5);
            case RequestRateLimitPerTimeUnit.Minutes:
                return (1, 40);
            case RequestRateLimitPerTimeUnit.Hours:
                return (1, 1600);
            default:
                throw new Exception("Miss Type");
        }
    }
}
