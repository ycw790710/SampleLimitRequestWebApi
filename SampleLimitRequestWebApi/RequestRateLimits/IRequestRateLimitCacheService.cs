using Microsoft.AspNetCore.Mvc.Controllers;
using System.Net;

namespace SampleLimitRequestWebApi.RequestRateLimits;

public interface IRequestRateLimitCacheService
{
    bool Valid(ControllerActionDescriptor controllerActionDescriptor, IPAddress remoteIpAddress);
    bool ValidUser(ControllerActionDescriptor controllerActionDescriptor, long userId);
}