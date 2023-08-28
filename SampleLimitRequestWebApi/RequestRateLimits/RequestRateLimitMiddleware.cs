using Microsoft.AspNetCore.Mvc.Controllers;
using System.Diagnostics;

namespace SampleLimitRequestWebApi.RequestRateLimits;

public class RequestRateLimitMiddleware
{
    private readonly RequestDelegate _next;

    public RequestRateLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IRequestRateLimitService _requestRateLimitService)
    {
        var requestAborted = context.RequestAborted;
        if (requestAborted.IsCancellationRequested)
            return;
        Debug.WriteLine("RequestRateLimitMiddleware");

        ControllerActionDescriptor? controllerActionDescriptor =
            context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();

        var isRequestOverLimit =
            _requestRateLimitService.IsRequestOverLimit(controllerActionDescriptor, context.Connection.RemoteIpAddress,
            requestAborted);

        if (requestAborted.IsCancellationRequested)
            return;

        if (isRequestOverLimit)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too many requests. Please try again later.", requestAborted);
            return;
        }

        await _next(context);
    }

}