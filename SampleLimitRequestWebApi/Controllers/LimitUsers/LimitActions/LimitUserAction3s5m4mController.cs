using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitUsers.LimitActions;

public class LimitUserAction3s5m4mController : BaseAuthorizeController
{
    private readonly ILogger _logger;

    public LimitUserAction3s5m4mController(ILogger<LimitUserAction3s5m4mController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    [UserRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
    [UserRequestRateLimit(4, RequestRateLimitPerTimeUnit.Minutes)]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}