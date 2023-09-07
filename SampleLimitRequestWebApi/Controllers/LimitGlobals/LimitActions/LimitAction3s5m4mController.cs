using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitActions;

public class LimitAction3s5m4mController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitAction3s5m4mController(ILogger<LimitAction3s5m4mController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    [GlobalRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
    [GlobalRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}