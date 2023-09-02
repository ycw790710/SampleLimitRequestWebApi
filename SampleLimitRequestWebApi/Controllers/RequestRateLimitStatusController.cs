using Microsoft.AspNetCore.Mvc;
using SampleLimitRequestWebApi.RequestRateLimits;
using SampleLimitRequestWebApi.RequestRateLimits.Dtos;

namespace SampleLimitRequestWebApi.Controllers;

public class RequestRateLimitStatusController : DefaultControllerBase
{
    private readonly IRequestRateLimitStatusService _requestRateLimitStatusService;

    public RequestRateLimitStatusController(IRequestRateLimitStatusService requestRateLimitStatusService)
    {
        _requestRateLimitStatusService = requestRateLimitStatusService;
    }

    [HttpPost]
    public ActionResult<RequestRateLimitStatus> GetStatus()
    {
        var json = _requestRateLimitStatusService.GetStatusJson();
        return new ContentResult
        {
            Content = json,
            ContentType = "application/json",
            StatusCode = 200
        };
    }

}