namespace SampleLimitRequestWebApi.RequestRateLimits;

public interface IRequestRateLimitStatusService
{
    void UpdateStatuses();
    string? GetStatusJson();
}