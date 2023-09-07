using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitActions;

public class LimitAction3sController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitAction3sController(ILogger<LimitAction3sController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}