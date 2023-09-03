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
        //var json = _requestRateLimitStatusService.GetStatusJson();
        //return new ContentResult
        //{
        //    Content = json,
        //    ContentType = "application/json",
        //    StatusCode = 200
        //};

        var jsonBytes = _requestRateLimitStatusService.GetStatusJsonBytes();
        return File(jsonBytes, "application/json");

        //var content = new ByteArrayContent(jsonBytes);
        //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        //HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.OK)
        //{
        //    Content = content
        //};
        //return new HttpResponseMessageResult(responseMsg);
    }

}