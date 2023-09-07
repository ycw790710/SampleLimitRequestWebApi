using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitUsers.LimitActions;

[UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
public class LimitUserController3sController : BaseAuthorizeController
{
    private readonly ILogger _logger;

    public LimitUserController3sController(ILogger<LimitUserController3sController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}