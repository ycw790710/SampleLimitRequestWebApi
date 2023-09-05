﻿namespace RequestRateLimit.Components;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class GlobalRequestRateLimitAttribute : RequestRateLimitAttribute
{
    public GlobalRequestRateLimitAttribute(int limitTimes, RequestRateLimitPerTimeUnit inputPerTimeUnit)
        : base(limitTimes, inputPerTimeUnit)
    {
    }

    public string GetKey(string controllerName, string actionName)
    {
        return $"[{controllerName}/{actionName}]";
    }

    protected override (int MinTimes, int MaxTimes) GetExpectedTimes(RequestRateLimitPerTimeUnit inputPerTimeUnit)
    {
        switch (inputPerTimeUnit)
        {
            case RequestRateLimitPerTimeUnit.Seconds:
                return (1, 5000);
            case RequestRateLimitPerTimeUnit.Minutes:
                return (1, 40000);
            case RequestRateLimitPerTimeUnit.Hours:
                return (1, 1600000);
            default:
                throw new Exception("Miss Type");
        }
    }
}