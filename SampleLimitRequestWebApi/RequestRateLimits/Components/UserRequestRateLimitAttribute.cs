namespace SampleLimitRequestWebApi.RequestRateLimits.Components;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class UserRequestRateLimitAttribute : Attribute, IRequestRateLimitAttribute
{
    public int LimitTimes { get; }

    public RequestRateLimitPerTimeUnit PerTimeUnit { get; }

    public UserRequestRateLimitAttribute(int limitTimes, RequestRateLimitPerTimeUnit inputPerTimeUnit)
    {
        var perTimeUnit = UserRequestRateLimitPerTimeUnit.Convert(inputPerTimeUnit);
        if (limitTimes < perTimeUnit.MinTimes || limitTimes > perTimeUnit.MaxTimes)
            throw new ArgumentException(
                $"Invalid {nameof(limitTimes)}, min {perTimeUnit.MinTimes}, max {perTimeUnit.MaxTimes}");

        LimitTimes = limitTimes;
        PerTimeUnit = inputPerTimeUnit;
    }

    public string GetKey(long userId)
    {
        return userId.ToString();
    }
}
