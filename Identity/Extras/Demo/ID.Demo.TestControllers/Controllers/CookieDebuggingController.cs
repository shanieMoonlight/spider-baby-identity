using ID.Application.Authenticators.Teams;
using ID.Demo.TestControllers.Controllers.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ID.Demo.TestControllers.Controllers;

[ApiController]
[Route("api/[controller]")]
[SuperMinimumAuthenticator.ActionFilter]
public class CookieDebuggingController : ControllerBase
{
    [HttpGet("cookie-info")]
    [SuperAuthenticator.ActionFilter]
    public IActionResult CookieInfo()
    {
        var user = User;
        // Extract all claims as name-value pairs
        var claims = User.Claims.Select(c => new
        {
            c.Type,
            c.Value
        }).ToList();
        var cookies = Request.Cookies;

        var firstCookie = Request.Cookies.FirstOrDefault();
        var firstCookieValue = firstCookie.Value ?? "No cookies present";
        var expiresUtc = User.FindFirst(".AspNetCore.Cookies.Expiration")?.Value;
        return Ok(new
        {
            User.Identity?.IsAuthenticated,
            ExpiresUtc = expiresUtc,
            User.Identity?.AuthenticationType,
            ClaimsCount = claims.Count,
            CookiesPresent = Request.Cookies.Select(c => c.Key).ToList(),
            Claims = claims,

            // Add more specific authentication details
        });
    }

}//Cls
