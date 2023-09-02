
namespace SampleLimitRequestWebApi.RequestRateLimits.Dtos;

public enum RequestRateLimitStatusContainerType
{
    GlobalController,
    GlobalAction,
    Ip,
    User,
}