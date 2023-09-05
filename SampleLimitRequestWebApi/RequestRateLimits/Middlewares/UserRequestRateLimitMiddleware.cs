using Microsoft.AspNetCore.Mvc.Controllers;
using RequestRateLimit.Services;

namespace SampleLimitRequestWebApi.RequestRateLimits.Middlewares;

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

        ControllerActionDescriptor? controllerActionDescriptor =
            context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();

        var cancellationToken = context.RequestAborted;


        int? userId = null;
        if (int.TryParse(context.User.Identity?.Name, out var parsedUserId))
            userId = parsedUserId;

        var isUserRequestOverLimit = false;
        if (controllerActionDescriptor != null)
        {
            var methodInfo = controllerActionDescriptor.MethodInfo;
            var typeInfo = controllerActionDescriptor.ControllerTypeInfo;
            var controllerName = controllerActionDescriptor.ControllerName;
            var actionName = controllerActionDescriptor.ActionName;

            isUserRequestOverLimit =
            _requestRateLimitService.IsUserRequestOverLimit(methodInfo,
                typeInfo, controllerName, actionName, userId, cancellationToken);
        }

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