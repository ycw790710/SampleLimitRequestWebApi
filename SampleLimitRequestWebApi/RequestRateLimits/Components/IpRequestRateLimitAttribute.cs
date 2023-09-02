using System.Net;

namespace SampleLimitRequestWebApi.RequestRateLimits.Components;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class IpRequestRateLimitAttribute : Attribute, IRequestRateLimitAttribute
{
    public int LimitTimes { get; }

    public RequestRateLimitPerTimeUnit PerTimeUnit { get; }

    public IpRequestRateLimitAttribute(int limitTimes, RequestRateLimitPerTimeUnit inputPerTimeUnit)
    {
        var perTimeUnit = IpRequestRateLimitPerTimeUnit.Convert(inputPerTimeUnit);
        if (limitTimes < perTimeUnit.MinTimes || limitTimes > perTimeUnit.MaxTimes)
            throw new ArgumentException(
                $"Invalid {nameof(limitTimes)}, min {perTimeUnit.MinTimes}, max {perTimeUnit.MaxTimes}");

        LimitTimes = limitTimes;
        PerTimeUnit = inputPerTimeUnit;
    }

    public string GetKey(string controllerName, string actionName, IPAddress iPAddress)
    {
        return $"[{iPAddress}] [{controllerName}/{actionName}]";
    }
}
