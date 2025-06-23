using Microsoft.AspNetCore.Mvc;

namespace ID.Demo.TestControllers.Controllers;

[ApiController]
[Route("identity/[controller]")]
public class AuthenticatorDemoController : ControllerBase
{
    [HttpGet("endpoints")]
    public IActionResult GetAvailableEndpoints()
    {
        var endpoints = new
        {
            message = "MyId Demo - Authenticator Endpoints",
            description = "Test different authenticators by calling these endpoints while authenticated with different team/position combinations",
            endpoints = new
            {
                super_authenticators = new
                {
                    base_url = "/api/SuperAuthenticatorDemo",
                    endpoints = new[]
                    {
                        "GET /super - SuperAuthenticator",
                        "GET /super-minimum - SuperMinimumAuthenticator", 
                        "GET /super-leader - SuperLeaderAuthenticator",
                        "GET /super-minimum-or-dev - SuperMinimumOrDevAuthenticator",
                        "GET /super-resource-filter - SuperMinimumAuthenticator.ResourceFilter(1)"
                    }
                },
                maintenance_authenticators = new
                {
                    base_url = "/api/MaintenanceAuthenticatorDemo",
                    endpoints = new[]
                    {
                        "GET /mntc - MntcAuthenticator",
                        "GET /mntc-minimum - MntcMinimumAuthenticator",
                        "GET /mntc-leader - MntcLeaderAuthenticator", 
                        "GET /mntc-leader-minimum - MntcLeaderMinimumAuthenticator",
                        "GET /mntc-minimum-or-dev - MntcMinimumOrDevAuthenticator",
                        "GET /mntc-resource-filter - MntcMinimumAuthenticator.ResourceFilter(2)"
                    }
                },
                customer_authenticators = new
                {
                    base_url = "/api/CustomerAuthenticatorDemo",
                    endpoints = new[]
                    {
                        "GET /customer - CustomerAuthenticator",
                        "GET /customer-minimum - CustomerMinimumAuthenticator",
                        "GET /customer-leader - CustomerLeaderAuthenticator",
                        "GET /customer-leader-minimum - CustomerLeaderMinimumAuthenticator", 
                        "GET /customer-resource-filter - CustomerMinimumAuthenticator.ResourceFilter(1)"
                    }
                },
                miscellaneous_authenticators = new
                {
                    base_url = "/api/MiscellaneousAuthenticatorDemo",
                    endpoints = new[]
                    {
                        "GET /leader - LeaderAuthenticator",
                        "GET /position-minimum - PositionMinimumAuthenticator",
                        "GET /position-minimum-with-level - PositionMinimumAuthenticator.ActionFilter(3)",
                        "GET /position-minimum-resource-filter - PositionMinimumAuthenticator.ResourceFilter(2)",
                        "GET /leader-resource-filter - LeaderAuthenticator.ResourceFilter",
                        "GET /policy-example - Policy-based authentication"
                    }
                }
            },
            authentication_note = "All endpoints require authentication. Use JWT tokens with appropriate team and position claims.",
            expected_response_format = new
            {
                message = "You are authenticated!!!",
                Authenticator = "<Name of authenticator>"
            }
        };

        return Ok(endpoints);
    }

    [HttpGet("test-info")]
    public IActionResult GetTestingInfo()
    {
#pragma warning disable CA1861 // Avoid constant arrays as arguments
        return Ok(new
        {
            message = "Authenticator Testing Information",
            testing_scenarios = new
            {
                customer_team = new
                {
                    description = "Test with customer team member JWT",
                    should_pass = new[] 
                    {
                        "/api/CustomerAuthenticatorDemo/customer",
                        "/api/CustomerAuthenticatorDemo/customer-minimum",
                        "/api/MiscellaneousAuthenticatorDemo/position-minimum"
                    },
                    should_fail = new[]
                    {
                        "/api/SuperAuthenticatorDemo/super",
                        "/api/MaintenanceAuthenticatorDemo/mntc"
                    }
                },
                maintenance_team = new
                {
                    description = "Test with maintenance team member JWT",
                    should_pass = new[]
                    {
                        "/api/MaintenanceAuthenticatorDemo/mntc",
                        "/api/MaintenanceAuthenticatorDemo/mntc-minimum",
                        "/api/MiscellaneousAuthenticatorDemo/position-minimum"
                    },
                    should_fail = new[]
                    {
                        "/api/SuperAuthenticatorDemo/super",
                        "/api/CustomerAuthenticatorDemo/customer"
                    }
                },
                super_team = new
                {
                    description = "Test with super team member JWT",
                    should_pass = new[]
                    {
                        "/api/SuperAuthenticatorDemo/super",
                        "/api/SuperAuthenticatorDemo/super-minimum",
                        "/api/MiscellaneousAuthenticatorDemo/leader"
                    },
                    should_fail = new[]
                    {
                        "/api/CustomerAuthenticatorDemo/customer",
                        "/api/MaintenanceAuthenticatorDemo/mntc"
                    }
                }
            },
            response_codes = new
            {
                success = "200 OK - Authentication passed",
                unauthorized = "401 Unauthorized - Not authenticated",
                forbidden = "403 Forbidden - Authenticated but insufficient permissions"
            }
        });
#pragma warning restore CA1861 // Avoid constant arrays as arguments
    }
}
