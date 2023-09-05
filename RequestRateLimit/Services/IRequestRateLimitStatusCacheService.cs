namespace RequestRateLimit.Services;

public interface IRequestRateLimitStatusCacheService
{
    void SendContainer(RequestRateLimitStatusContainerActionType actionType, RequestRateLimitStatusContainer container);
    void UpdateStatuses();
    string? GetStatusJson();
    byte[] GetStatusJsonBytes();
    byte[] GetStatusInfoJsonBytes();
}