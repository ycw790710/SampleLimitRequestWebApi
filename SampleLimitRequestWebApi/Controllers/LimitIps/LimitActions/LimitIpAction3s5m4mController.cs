using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitActions;

public class LimitIpAction3s5m4mController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitIpAction3s5m4mController(ILogger<LimitIpAction3s5m4mController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    [IpRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
    [IpRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}