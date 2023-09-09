namespace RequestRateLimit.Services;

public interface IRequestRateLimitCacheService
{
    bool Valid(string httpMethod, MethodInfo actionInfo, TypeInfo controllerInfo,
        string controllerName, string actionName, string remoteIpAddress);
    bool ValidUser(string httpMethod, MethodInfo actionInfo, TypeInfo controllerInfo,
        string controllerName, string actionName, long userId);
    void RemoveExpired();
}