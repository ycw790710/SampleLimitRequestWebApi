namespace RequestRateLimit.Services;

public interface IRequestRateLimitService
{
    bool IsRequestOverLimit(string httpMethod, MethodInfo actionInfo, TypeInfo controllerInfo,
        string controllerName, string actionName, string? remoteIpAddress,
        CancellationToken cancellationToken = default);
    bool IsUserRequestOverLimit(string httpMethod, MethodInfo methodInfo, TypeInfo typeInfo,
        string controllerName, string actionName, long? userId,
        CancellationToken cancellationToken = default);
}