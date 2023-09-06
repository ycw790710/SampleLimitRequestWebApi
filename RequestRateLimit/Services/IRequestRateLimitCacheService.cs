namespace RequestRateLimit.Services;

public interface IRequestRateLimitCacheService
{
    bool Valid(string httpMethod, MethodInfo methodInfo, TypeInfo typeInfo,
        string controllerName, string actionName, IPAddress remoteIpAddress);
    bool ValidUser(string httpMethod, MethodInfo methodInfo, TypeInfo typeInfo,
        string controllerName, string actionName, long userId);
    void RemoveExpired();
}