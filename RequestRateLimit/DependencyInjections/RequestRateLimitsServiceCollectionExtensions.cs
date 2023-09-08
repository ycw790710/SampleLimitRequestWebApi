using Microsoft.Extensions.DependencyInjection;
using RequestRateLimit.BackgroundServices;
using GlobalTimers.DependencyInjections;

namespace RequestRateLimit.DependencyInjections;

public static class RequestRateLimitsServiceCollectionExtensions
{
    public static IServiceCollection AddRequestRateLimits(this IServiceCollection services)
    {
        services.AddGlobalTimers();
        services.AddSingleton<IRequestRateLimitStatusCacheService, RequestRateLimitStatusCacheService>();
        services.AddSingleton<IRequestRateLimitCacheService, RequestRateLimitCacheService>();
        services.AddSingleton<IRequestRateLimitStatusService, RequestRateLimitStatusService>();
        services.AddScoped<IRequestRateLimitService, RequestRateLimitService>();
        services.AddHostedService<RequestRateLimitStatusCacheBackgroundService>();
        return services;
    }
}