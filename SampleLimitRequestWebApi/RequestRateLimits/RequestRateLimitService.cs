using Microsoft.AspNetCore.Mvc.Controllers;
using System.Net;

namespace SampleLimitRequestWebApi.RequestRateLimits;

public class RequestRateLimitService : IRequestRateLimitService
{
    private readonly IRequestRateLimitCacheService _requestRateLimitCacheService;

    public RequestRateLimitService(IRequestRateLimitCacheService requestRateLimitCacheService)
    {
        _requestRateLimitCacheService = requestRateLimitCacheService;
    }

    public bool IsRequestOverLimit(ControllerActionDescriptor? controllerActionDescriptor, IPAddress? remoteIpAddress,
        CancellationToken cancellationToken = default)
    {
        if (controllerActionDescriptor == null)
            return false;
        if (remoteIpAddress == null)
            return true;

        return !_requestRateLimitCacheService.Valid(controllerActionDescriptor, remoteIpAddress);
    }

    public bool IsUserRequestOverLimit(ControllerActionDescriptor? controllerActionDescriptor, long? userId,
        CancellationToken cancellationToken = default)
    {
        if (controllerActionDescriptor == null)
            return false;
        if (userId == null)
            return false;

        return !_requestRateLimitCacheService.ValidUser(controllerActionDescriptor, userId.Value);
    }

}