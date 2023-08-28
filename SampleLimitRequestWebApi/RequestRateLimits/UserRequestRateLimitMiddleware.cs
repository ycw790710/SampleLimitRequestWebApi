using Microsoft.AspNetCore.Mvc.Controllers;
using System.Diagnostics;

namespace SampleLimitRequestWebApi.RequestRateLimits;

public class UserRequestRateLimitMiddleware
{
    private readonly RequestDelegate _next;

    public UserRequestRateLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IRequestRateLimitService _requestRateLimitService)
    {
        var requestAborted = context.RequestAborted;
        if (requestAborted.IsCancellationRequested)
            return;
        Debug.WriteLine("UserRequestRateLimitMiddleware");

        ControllerActionDescriptor? controllerActionDescriptor =
            context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();

        var cancellationToken = context.RequestAborted;

        int? userId = 1;//TODO: const user id

        var isUserRequestOverLimit =
            _requestRateLimitService.IsUserRequestOverLimit(controllerActionDescriptor, userId, cancellationToken);

        if (requestAborted.IsCancellationRequested)
            return;

        if (isUserRequestOverLimit)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
            return;
        }

        await _next(context);
    }

}