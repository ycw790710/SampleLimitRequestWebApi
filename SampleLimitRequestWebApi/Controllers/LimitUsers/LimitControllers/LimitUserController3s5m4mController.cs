using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitUsers.LimitActions;

[UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
[UserRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
[UserRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
public class LimitUserController3s5m4mController : BaseAuthorizeController
{
    private readonly ILogger _logger;

    public LimitUserController3s5m4mController(ILogger<LimitUserController3s5m4mController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}