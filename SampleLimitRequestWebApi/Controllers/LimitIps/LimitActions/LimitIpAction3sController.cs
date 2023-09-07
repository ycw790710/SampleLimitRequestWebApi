using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitActions;

public class LimitIpAction3sController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitIpAction3sController(ILogger<LimitIpAction3sController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [IpRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}