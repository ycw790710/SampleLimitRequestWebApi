namespace RequestRateLimit.Dtos;


public enum RequestRateLimitStatusContainerType
{
    GlobalController,
    GlobalAction,
    Ip,
    User,
}