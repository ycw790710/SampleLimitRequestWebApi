using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace RequestRateLimit.Middlewares;

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

        bool isRequestOverLimit = false;
        ControllerActionDescriptor? controllerActionDescriptor =
            context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (controllerActionDescriptor != null)
        {
            var httpMethod = context.Request.Method;
            var methodInfo = controllerActionDescriptor.MethodInfo;
            var typeInfo = controllerActionDescriptor.ControllerTypeInfo;
            var controllerName = controllerActionDescriptor.ControllerName;
            var actionName = controllerActionDescriptor.ActionName;

            isRequestOverLimit =
                _requestRateLimitService.IsRequestOverLimit(httpMethod, methodInfo,
                typeInfo, controllerName, actionName, context.Connection.RemoteIpAddress,
                requestAborted);
        }

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

public static class RequestRateLimitMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestRateLimit(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestRateLimitMiddleware>();
    }
}