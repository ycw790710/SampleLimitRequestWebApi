using Microsoft.AspNetCore.Mvc;
using RequestRateLimit.Dtos;
using RequestRateLimit.Services;

namespace SampleLimitRequestWebApi.Controllers;

public class RequestRateLimitStatusController : DefaultControllerBase
{
    private readonly IRequestRateLimitStatusService _requestRateLimitStatusService;

    public RequestRateLimitStatusController(IRequestRateLimitStatusService requestRateLimitStatusService)
    {
        _requestRateLimitStatusService = requestRateLimitStatusService;
    }

    [HttpPost]
    [GlobalRequestRateLimit(20000, RequestRateLimitPerTimeUnit.Seconds)]
    public ActionResult<RequestRateLimitStatus> GetStatus()
    {
        //var json = _requestRateLimitStatusService.GetStatusJson();
        //return new ContentResult
        //{
        //    Content = json,
        //    ContentType = "application/json",
        //    StatusCode = 200
        //};

        var jsonBytes = _requestRateLimitStatusService.GetStatusJsonBytes();
        return File(jsonBytes, "application/json");

        //var jsonBytes = _requestRateLimitStatusService.GetStatusJsonBytes();
        //var content = new ByteArrayContent(jsonBytes);
        //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        //HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.OK)
        //{
        //    Content = content
        //};
        //return new HttpResponseMessageResult(responseMsg);
    }

    [HttpPost]
    [GlobalRequestRateLimit(20000, RequestRateLimitPerTimeUnit.Seconds)]
    public ActionResult<RequestRateLimitStatusInfo> GetStatusInfo()
    {
        var jsonBytes = _requestRateLimitStatusService.GetStatusInfoJsonBytes();
        return File(jsonBytes, "application/json");
    }

}