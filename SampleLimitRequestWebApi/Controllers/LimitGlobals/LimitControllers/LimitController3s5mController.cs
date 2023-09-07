﻿using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers.LimitGlobals.LimitControllers;

[GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
[GlobalRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
public class LimitController3s5mController : BaseAnonymousController
{
    private readonly ILogger _logger;

    public LimitController3s5mController(ILogger<LimitController3s5mController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        return Ok();
    }

}