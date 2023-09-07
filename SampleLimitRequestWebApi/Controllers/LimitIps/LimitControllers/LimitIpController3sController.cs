using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitControllers;

[IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
public class LimitIpController3sController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitIpController3sController(ILogger<LimitIpController3sController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}