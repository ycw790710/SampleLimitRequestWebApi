namespace RequestRateLimit.Components;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class UserRequestRateLimitAttribute : RequestRateLimitAttribute
{

    public UserRequestRateLimitAttribute(int limitTimes, RequestRateLimitPerTimeUnit inputPerTimeUnit)
        : base(limitTimes, inputPerTimeUnit)
    {
    }

    public string GetKey(string httpMethod, string controllerName, string actionName, long userId)
    {
        return $"[{userId}] {GetPathKey(httpMethod, controllerName, actionName)}";
    }

    protected override (int MinTimes, int MaxTimes) GetExpectedTimes(RequestRateLimitPerTimeUnit inputPerTimeUnit)
    {
        switch (inputPerTimeUnit)
        {
            case RequestRateLimitPerTimeUnit.Seconds:
                return (1, 10);
            case RequestRateLimitPerTimeUnit.Minutes:
                return (1, 600);
            case RequestRateLimitPerTimeUnit.Hours:
                return (1, 36000);
            default:
                throw new Exception("Miss Type");
        }
    }
}
