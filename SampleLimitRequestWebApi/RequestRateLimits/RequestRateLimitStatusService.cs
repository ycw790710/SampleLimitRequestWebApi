using SampleLimitRequestWebApi.RequestRateLimits.Dtos;

namespace SampleLimitRequestWebApi.RequestRateLimits;

public class RequestRateLimitStatusService : IRequestRateLimitStatusService
{
    private readonly IRequestRateLimitStatusCacheService _requestRateLimitStatusCacheService;
    private readonly IRequestRateLimitCacheService _requestRateLimitCacheService;

    public RequestRateLimitStatusService(IRequestRateLimitStatusCacheService requestRateLimitStatusCacheService,
        IRequestRateLimitCacheService requestRateLimitCacheService)
    {
        _requestRateLimitStatusCacheService = requestRateLimitStatusCacheService;
        _requestRateLimitCacheService = requestRateLimitCacheService;
    }

    public void UpdateStatuses()
    {
        _requestRateLimitCacheService.RemoveExpired();
        _requestRateLimitStatusCacheService.UpdateStatuses();
    }

    public RequestRateLimitStatus? GetStatus()
    {
        return _requestRateLimitStatusCacheService.GetStatus();
    }

    public string? GetStatusJson()
    {
        return _requestRateLimitStatusCacheService.GetStatusJson();
    }
}