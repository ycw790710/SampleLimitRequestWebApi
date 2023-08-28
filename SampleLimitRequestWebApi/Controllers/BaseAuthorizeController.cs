using Microsoft.AspNetCore.Authorization;

namespace SampleLimitRequestWebApi.Controllers;

[Authorize]
public abstract class BaseAuthorizeController : DefaultControllerBase
{
}
