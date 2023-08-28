using Microsoft.AspNetCore.Mvc.Controllers;
using System.Net;

namespace SampleLimitRequestWebApi.RequestRateLimits;

public interface IRequestRateLimitService
{
    bool IsRequestOverLimit(ControllerActionDescriptor? controllerActionDescriptor, IPAddress? remoteIpAddress,
        CancellationToken cancellationToken = default);
    bool IsUserRequestOverLimit(ControllerActionDescriptor? controllerActionDescriptor, long? userId,
        CancellationToken cancellationToken = default);
}