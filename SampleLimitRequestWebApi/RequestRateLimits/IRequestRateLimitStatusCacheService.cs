using SampleLimitRequestWebApi.RequestRateLimits.Dtos;

namespace SampleLimitRequestWebApi.RequestRateLimits;

public interface IRequestRateLimitStatusCacheService
{
    void SendContainer(RequestRateLimitStatusContainerActionType actionType, RequestRateLimitStatusContainer container);
    void UpdateStatuses();
    string? GetStatusJson();
}