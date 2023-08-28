using Microsoft.AspNetCore.Mvc;

namespace SampleLimitRequestWebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public abstract class DefaultControllerBase : ControllerBase
{
}
