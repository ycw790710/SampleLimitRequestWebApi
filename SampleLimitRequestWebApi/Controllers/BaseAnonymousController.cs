using Microsoft.AspNetCore.Authorization;

namespace SampleLimitRequestWebApi.Controllers;

[AllowAnonymous]
public abstract class BaseAnonymousController : DefaultControllerBase
{
}
