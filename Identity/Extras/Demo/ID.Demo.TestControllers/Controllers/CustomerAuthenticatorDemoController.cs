using ID.Application.Authenticators.Teams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ID.Demo.TestControllers.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerAuthenticatorDemoController : ControllerBase
{
    [HttpGet("customer")]
    [CustomerAuthenticator.ActionFilter]
    public IActionResult TestCustomerAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "CustomerAuthenticator"
        });
    }

    [HttpGet("customer-minimum")]
    [CustomerMinimumAuthenticator.ActionFilter]
    public IActionResult TestCustomerMinimumAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "CustomerMinimumAuthenticator"
        });
    }

    [HttpGet("customer-leader")]
    [CustomerLeaderAuthenticator.ActionFilter]
    public IActionResult TestCustomerLeaderAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "CustomerLeaderAuthenticator"
        });
    }    [HttpGet("customer-leader-minimum")]
    [CustomerLeaderMinimumAuthenticator.ActionFilter]
    public IActionResult TestCustomerLeaderMinimumAuthenticator()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "CustomerLeaderMinimumAuthenticator"
        });
    }

    [HttpGet("customer-resource-filter")]
    [CustomerMinimumAuthenticator.ResourceFilter(1)]
    public IActionResult TestCustomerResourceFilter()
    {
        return Ok(new
        {
            message = "You are authenticated!!!",
            Authenticator = "CustomerMinimumAuthenticator.ResourceFilter(1)"
        });
    }
}
