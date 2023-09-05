namespace RequestRateLimit.Services;

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

    public string? GetStatusJson()
    {
        return _requestRateLimitStatusCacheService.GetStatusJson();
    }

    public byte[] GetStatusJsonBytes()
    {
        return _requestRateLimitStatusCacheService.GetStatusJsonBytes();
    }
    public byte[] GetStatusInfoJsonBytes()
    {
        return _requestRateLimitStatusCacheService.GetStatusInfoJsonBytes();
    }
}