using SampleLimitRequestWebApi.RequestRateLimits.Dtos;

namespace SampleLimitRequestWebApi.RequestRateLimits;

public interface IRequestRateLimitStatusService
{
    RequestRateLimitStatus? GetStatus();
    void UpdateStatuses();
    string? GetStatusJson();
}