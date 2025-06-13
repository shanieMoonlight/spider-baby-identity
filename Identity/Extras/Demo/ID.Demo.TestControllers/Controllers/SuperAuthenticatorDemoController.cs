using ID.Application.Authenticators.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ID.Demo.TestControllers.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuperAuthenticatorDemoController : ControllerBase
{
    [HttpGet("super")]
    [SuperAuthenticator.ActionFilter]
    public IActionResult TestSuperAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "SuperAuthenticator"
        });
    }

    [HttpGet("super-minimum")]
    [SuperMinimumAuthenticator.ActionFilter]
    public IActionResult TestSuperMinimumAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "SuperMinimumAuthenticator"
        });
    }

    [HttpGet("super-leader")]
    [SuperLeaderAuthenticator.ActionFilter]
    public IActionResult TestSuperLeaderAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "SuperLeaderAuthenticator"
        });
    }

    [HttpGet("super-minimum-or-dev")]
    [SuperMinimumOrDevAuthenticator.ActionFilter]
    public IActionResult TestSuperMinimumOrDevAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "SuperMinimumOrDevAuthenticator"
        });
    }

    [HttpGet("super-resource-filter")]
    [SuperMinimumAuthenticator.ResourceFilter(1)]
    public IActionResult TestSuperResourceFilter()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "SuperMinimumAuthenticator.ResourceFilter(1)"
        });
    }
}
