namespace RequestRateLimit.Services;

public class RequestRateLimitService : IRequestRateLimitService
{
    private readonly IRequestRateLimitCacheService _requestRateLimitCacheService;

    public RequestRateLimitService(IRequestRateLimitCacheService requestRateLimitCacheService)
    {
        _requestRateLimitCacheService = requestRateLimitCacheService;
    }

    public bool IsRequestOverLimit(string httpMethod, MethodInfo actionInfo, TypeInfo controllerInfo,
        string controllerName, string actionName, string? remoteIpAddress,
        CancellationToken cancellationToken = default)
    {
        if (remoteIpAddress == null)
            return true;

        return !_requestRateLimitCacheService.Valid(httpMethod, actionInfo, controllerInfo, controllerName, actionName, remoteIpAddress);
    }

    public bool IsUserRequestOverLimit(string httpMethod, MethodInfo methodInfo, TypeInfo typeInfo,
        string controllerName, string actionName, long? userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == null)
            return false;

        return !_requestRateLimitCacheService.ValidUser(httpMethod, methodInfo, typeInfo, controllerName, actionName, userId.Value);
    }

}