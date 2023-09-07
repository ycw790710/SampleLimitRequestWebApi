using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitUsers.LimitActions;

public class LimitUserAction3sController : BaseAuthorizeController
{
    private readonly ILogger _logger;

    public LimitUserAction3sController(ILogger<LimitUserAction3sController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}