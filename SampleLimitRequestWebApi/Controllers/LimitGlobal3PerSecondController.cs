using Microsoft.AspNetCore.Mvc;
using SampleLimitRequestWebApi.RequestRateLimits.Components;

namespace SampleLimitRequestWebApi.Controllers;

[GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
public class LimitGlobal3PerSecondController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitGlobal3PerSecondController(ILogger<LimitGlobal3PerSecondController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get_Normal([FromQuery] string? data)
    {
        return Ok();
    }

}