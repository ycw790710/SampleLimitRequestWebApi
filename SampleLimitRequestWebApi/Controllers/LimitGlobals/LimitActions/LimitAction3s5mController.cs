using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitActions;

public class LimitAction3s5mController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitAction3s5mController(ILogger<LimitAction3s5mController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    [GlobalRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}