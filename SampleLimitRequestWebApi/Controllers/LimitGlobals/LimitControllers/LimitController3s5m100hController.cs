using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitControllers;

[GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
[GlobalRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
[GlobalRequestRateLimit(100, RequestRateLimitPerTimeUnit.Hours)]
public class LimitController3s5m100hController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitController3s5m100hController(ILogger<LimitController3s5m100hController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}