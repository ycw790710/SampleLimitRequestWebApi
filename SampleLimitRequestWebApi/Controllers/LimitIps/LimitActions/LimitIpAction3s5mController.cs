using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitActions;

public class LimitIpAction3s5mController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitIpAction3s5mController(ILogger<LimitIpAction3s5mController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    [IpRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}