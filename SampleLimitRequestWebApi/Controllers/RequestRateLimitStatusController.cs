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
        var status = _requestRateLimitStatusService.GetStatus();
        return Ok(status);
    }

}