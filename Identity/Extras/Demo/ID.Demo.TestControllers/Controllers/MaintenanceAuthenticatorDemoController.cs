using ID.Application.Authenticators.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ID.Demo.TestControllers.Controllers;

[ApiController]
[Route("identity/[controller]")]
[Authorize]
public class MaintenanceAuthenticatorDemoController : ControllerBase
{
    [HttpGet("mntc")]
    [MntcAuthenticator.ActionFilter]
    public IActionResult TestMntcAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "MntcAuthenticator"
        });
    }

    [HttpGet("mntc-minimum")]
    [MntcMinimumAuthenticator.ActionFilter]
    public IActionResult TestMntcMinimumAuthenticator()
    {
        return BadRequest(new { Message = "This is a mess" });
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "MntcMinimumAuthenticator"
        });
    }

    [HttpGet("mntc-leader")]
    [MntcLeaderAuthenticator.ActionFilter]
    public IActionResult TestMntcLeaderAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "MntcLeaderAuthenticator"
        });
    }

    [HttpGet("mntc-leader-minimum")]
    [MntcLeaderMinimumAuthenticator.ActionFilter]
    public IActionResult TestMntcLeaderMinimumAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "MntcLeaderMinimumAuthenticator"
        });
    }

    [HttpGet("mntc-minimum-or-dev")]
    [MntcMinimumOrDevAuthenticator.ActionFilter]
    public IActionResult TestMntcMinimumOrDevAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "MntcMinimumOrDevAuthenticator"
        });
    }

    [HttpGet("mntc-resource-filter")]
    [MntcMinimumAuthenticator.ResourceFilter(2)]
    public IActionResult TestMntcResourceFilter()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "MntcMinimumAuthenticator.ResourceFilter(2)"
        });
    }
}
