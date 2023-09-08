using GlobalTimers.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GlobalTimers.DependencyInjections;

public static class GlobalTimerServiceCollectionExtensions
{
    public static IServiceCollection AddGlobalTimers(this IServiceCollection services)
    {
        services.AddSingleton<IGlobalTimerService, GlobalTimerService>();
        return services;
    }
}