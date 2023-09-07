using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitUsers.LimitActions;

public class LimitUserAction3s5m100hController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitUserAction3s5m100hController(ILogger<LimitUserAction3s5m100hController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    [UserRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
    [UserRequestRateLimit(100, RequestRateLimitPerTimeUnit.Hours)]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}