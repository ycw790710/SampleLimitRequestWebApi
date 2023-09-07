using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitControllers;

[IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
[IpRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
[IpRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
public class LimitIpController3s5m4mController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitIpController3s5m4mController(ILogger<LimitIpController3s5m4mController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}