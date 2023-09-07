using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitActions;

public class LimitIpAction3s5m100hController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitIpAction3s5m100hController(ILogger<LimitIpAction3s5m100hController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    [IpRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
    [IpRequestRateLimit(100, RequestRateLimitPerTimeUnit.Hours)]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}