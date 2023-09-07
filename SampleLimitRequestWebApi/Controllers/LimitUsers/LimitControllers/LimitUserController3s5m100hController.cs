using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitUsers.LimitActions;

[UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
[UserRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
[UserRequestRateLimit(100, RequestRateLimitPerTimeUnit.Hours)]
public class LimitUserController3s5m100hController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitUserController3s5m100hController(ILogger<LimitUserAction3s5m100hController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}