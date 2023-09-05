namespace RequestRateLimit.Components;

public enum RequestRateLimitType
{
    // as array index
    GlobalController = 0,
    GlobalAction = 1,
    Ip = 2,
    User = 3,
}