namespace RequestRateLimit.Services;

public interface IRequestRateLimitStatusService
{
    void UpdateStatuses();
    string? GetStatusJson();
    byte[] GetStatusJsonBytes();
    byte[] GetStatusInfoJsonBytes();
}