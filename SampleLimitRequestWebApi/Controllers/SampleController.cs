using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleLimitRequestWebApi.RequestRateLimits.Components;

namespace SampleLimitRequestWebApi.Controllers
{
    public class SampleController : DefaultControllerBase
    {
        private readonly ILogger _logger;

        public SampleController(ILogger<SampleController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get_Normal([FromQuery] string? data)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post_Normal([FromBody] string? data)
        {
            return Ok();
        }

        [HttpPost]
        [RequestSizeLimit(10)]
        public async Task<ActionResult<string>> Post_Limit10BytesSize([FromBody] string? data)
        {
            return Ok();
        }

        [GlobalRequestRateLimit(1, RequestRateLimitPerTimeUnit.Seconds)]
        [HttpGet]
        public async Task<ActionResult<string>> Get_LimitGlobal1PreSecond([FromQuery] string? data)
        {
            return Ok();
        }

        [GlobalRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
        [GlobalRequestRateLimit(5, RequestRateLimitPerTimeUnit.Minutes)]
        [HttpGet]
        public async Task<ActionResult<string>> Get_LimitGlobal3PreSecond5PerMinutes([FromQuery] string? data)
        {
            return Ok();
        }

        [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<string>> Get_LimitUser3PreSecond([FromQuery] string? data)
        {
            return Ok();
        }

        [GlobalRequestRateLimit(5, RequestRateLimitPerTimeUnit.Seconds)]
        [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<string>> Get_LimitGlobal5PreSecondUser3PreSecond([FromQuery] string? data)
        {
            return Ok();
        }

    }
}