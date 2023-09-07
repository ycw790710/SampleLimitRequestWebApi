using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace RequestRateLimit.Middlewares;

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
            var httpMethod = context.Request.Method;
            var methodInfo = controllerActionDescriptor.MethodInfo;
            var typeInfo = controllerActionDescriptor.ControllerTypeInfo;
            var controllerName = controllerActionDescriptor.ControllerName;
            var actionName = controllerActionDescriptor.ActionName;

            isUserRequestOverLimit =
            _requestRateLimitService.IsUserRequestOverLimit(httpMethod, methodInfo,
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

public static class UserRequestRateLimitMiddlewareExtensions
{
    /// <summary>
    /// 在驗證使用者(UseAuthentication and UseAuthorization)之後使用
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseUserRequestRateLimit(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserRequestRateLimitMiddleware>();
    }
}