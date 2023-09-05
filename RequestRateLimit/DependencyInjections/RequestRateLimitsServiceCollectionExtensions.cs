using Microsoft.Extensions.DependencyInjection;

namespace RequestRateLimit.DependencyInjections;

public static class RequestRateLimitsServiceCollectionExtensions
{
    public static IServiceCollection AddRequestRateLimits(this IServiceCollection services)
    {
        services.AddSingleton<IRequestRateLimitStatusCacheService, RequestRateLimitStatusCacheService>();
        services.AddSingleton<IRequestRateLimitCacheService, RequestRateLimitCacheService>();
        services.AddSingleton<IRequestRateLimitStatusService, RequestRateLimitStatusService>();
        services.AddScoped<IRequestRateLimitService, RequestRateLimitService>();
        return services;
    }
}