namespace RequestRateLimit.Services;

public class RequestRateLimitService : IRequestRateLimitService
{
    private readonly IRequestRateLimitCacheService _requestRateLimitCacheService;

    public RequestRateLimitService(IRequestRateLimitCacheService requestRateLimitCacheService)
    {
        _requestRateLimitCacheService = requestRateLimitCacheService;
    }

    public bool IsRequestOverLimit(string httpMethod, MethodInfo methodInfo, TypeInfo typeInfo,
        string controllerName, string actionName, IPAddress? remoteIpAddress,
        CancellationToken cancellationToken = default)
    {
        if (remoteIpAddress == null)
            return true;

        return !_requestRateLimitCacheService.Valid(httpMethod, methodInfo, typeInfo, controllerName, actionName, remoteIpAddress);
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