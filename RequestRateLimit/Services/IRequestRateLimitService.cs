namespace RequestRateLimit.Services;

public interface IRequestRateLimitService
{
    bool IsRequestOverLimit(MethodInfo methodInfo, TypeInfo typeInfo,
        string controllerName, string actionName, IPAddress? remoteIpAddress,
        CancellationToken cancellationToken = default);
    bool IsUserRequestOverLimit(MethodInfo methodInfo, TypeInfo typeInfo,
        string controllerName, string actionName, long? userId,
        CancellationToken cancellationToken = default);
}