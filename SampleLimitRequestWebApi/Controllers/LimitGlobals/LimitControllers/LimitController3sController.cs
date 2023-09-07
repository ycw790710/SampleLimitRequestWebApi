using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitControllers;

[GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
public class LimitController3sController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitController3sController(ILogger<LimitController3sController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}