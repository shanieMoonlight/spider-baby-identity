using ID.Application.Authenticators.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ID.Demo.TestControllers.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MiscellaneousAuthenticatorDemoController : ControllerBase
{
    [HttpGet("leader")]
    [LeaderAuthenticator.ActionFilter]
    public IActionResult TestLeaderAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "LeaderAuthenticator"
        });
    }

    [HttpGet("position-minimum")]
    [PositionMinimumAuthenticator.ActionFilter]
    public IActionResult TestPositionMinimumAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "PositionMinimumAuthenticator"
        });
    }

    [HttpGet("position-minimum-with-level")]
    [PositionMinimumAuthenticator.ActionFilter(3)]
    public IActionResult TestPositionMinimumWithLevelAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "PositionMinimumAuthenticator.ActionFilter(3)"
        });
    }

    [HttpGet("position-minimum-resource-filter")]
    [PositionMinimumAuthenticator.ResourceFilter(2)]
    public IActionResult TestPositionMinimumResourceFilter()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "PositionMinimumAuthenticator.ResourceFilter(2)"
        });
    }

    [HttpGet("leader-resource-filter")]
    [LeaderAuthenticator.ResourceFilter]
    public IActionResult TestLeaderResourceFilter()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "LeaderAuthenticator.ResourceFilter"
        });
    }    [HttpGet("policy-example")]
    [Authorize(Policy = CustomerAuthenticator.Policy.NAME)]
    public IActionResult TestPolicyBasedAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = $"Policy: {CustomerAuthenticator.Policy.NAME}"
        });
    }
}
