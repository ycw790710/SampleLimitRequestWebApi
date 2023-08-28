using Microsoft.AspNetCore.Mvc;
using SampleLimitRequestWebApi.RequestRateLimits.Components;

namespace SampleLimitRequestWebApi.Controllers
{
    public class TestController : BaseAnonymousController
    {
        private readonly ILogger _logger;

        public TestController(ILogger<TestController> logger)
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
        public async Task<ActionResult<string>> Post_SizeLimit_10Bytes([FromBody] string? data)
        {
            return Ok();
        }

        [GlobalRequestRateLimit(1, RequestRateLimitPerTimeUnit.Seconds)]
        [HttpGet]
        public async Task<ActionResult<string>> GetRateLimit_Global_1_times_preSecond([FromQuery] string? data)
        {
            return Ok();
        }

        [UserRequestRateLimit(3, RequestRateLimitPerTimeUnit.Seconds)]
        [HttpGet]
        public async Task<ActionResult<string>> GetRateLimit_User_3_times_preSecond([FromQuery] string? data)
        {
            return Ok();
        }

    }
}