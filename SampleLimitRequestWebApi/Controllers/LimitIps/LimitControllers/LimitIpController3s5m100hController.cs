using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitControllers;

[IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
[IpRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
[IpRequestRateLimit(100, RequestRateLimitPerTimeUnit.Hours)]
public class LimitIpController3s5m100hController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitIpController3s5m100hController(ILogger<LimitIpController3s5m100hController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}