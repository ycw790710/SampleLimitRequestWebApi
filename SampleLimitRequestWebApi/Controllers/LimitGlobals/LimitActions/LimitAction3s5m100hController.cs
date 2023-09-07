using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitActions;

public class LimitAction3s5m100hController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitAction3s5m100hController(ILogger<LimitAction3s5m100hController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    [GlobalRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
    [GlobalRequestRateLimit(100, RequestRateLimitPerTimeUnit.Hours)]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}