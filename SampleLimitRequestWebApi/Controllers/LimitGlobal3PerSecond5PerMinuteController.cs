using Microsoft.AspNetCore.Mvc;
using SampleLimitRequestWebApi.RequestRateLimits.Components;

namespace SampleLimitRequestWebApi.Controllers;

[GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
[GlobalRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
public class LimitGlobal3PerSecond5PerMinuteController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitGlobal3PerSecond5PerMinuteController(ILogger<LimitGlobal3PerSecond5PerMinuteController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get_Normal([FromQuery] string? data)
    {
        return Ok();
    }

}