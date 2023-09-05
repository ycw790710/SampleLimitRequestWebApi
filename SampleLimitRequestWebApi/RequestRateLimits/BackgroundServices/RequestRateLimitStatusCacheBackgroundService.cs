using RequestRateLimit.Services;

namespace SampleLimitRequestWebApi.RequestRateLimits.BackgroundServices;

public class RequestRateLimitStatusCacheBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public RequestRateLimitStatusCacheBackgroundService(IServiceProvider serviceProvider,
        ILogger<RequestRateLimitStatusCacheBackgroundService> logger)
    {
        _serviceProvider = serviceProvider
            ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();

            var requestRateLimitStatusService =
                scope.ServiceProvider.GetRequiredService<IRequestRateLimitStatusService>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    requestRateLimitStatusService.UpdateStatuses();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "執行'更新[Request速率限制]的狀態Cache'失敗");
                }

                await Task.Delay(100, cancellationToken);
            }
        }
    }
}