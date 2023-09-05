namespace RequestRateLimit.Services;

public interface IRequestRateLimitCacheService
{
    bool Valid(MethodInfo methodInfo, TypeInfo typeInfo,
        string controllerName, string actionName, IPAddress remoteIpAddress);
    bool ValidUser(MethodInfo methodInfo, TypeInfo typeInfo,
        string controllerName, string actionName, long userId);
    void RemoveExpired();
}