using ID.GlobalSettings.Constants;
using ID.Infrastructure.Auth.AppAbs;
using ID.Infrastructure.Persistance.EF.Setup.Options;
using ID.Infrastructure.Setup;
using ID.Infrastructure.Setup.Options;
using ID.Infrastructure.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ID.Infrastructure.Auth.AppImps;

/// <summary>
/// Service for handling authentication for external pages. (hangfire, Swagger , etc.)
/// </summary>
public class ExternalPageAuthenticationService(
        IExternalPageListService externalPageList,
        IOptions<IdInfrastructureOptions> _optionsProvider,
        IHostEnvironment env) 
    : IExternalPageAuthenticationService
{
    private readonly IdInfrastructureOptions _options = _optionsProvider.Value;

    //--------------------------//

    /// <summary>
    /// Authenticates the user based on the context.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The authentication result.</returns>
    public async Task<AuthenticateResult> TryAuthenticateAsync(HttpContext context)
    {
        try
       {

            var request = context.Request;

            if (!IsFromExternalPage(request))
                return AuthenticateResult.NoResult();

            // Try authentication by cookie. We may have already authenticated
            var cookieResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (cookieResult.Succeeded)
                return cookieResult;

            if (request.ContainsParam(IdGlobalConstants.ExtraAuth.JWT_QUERY_KEY))
            {
                var tknStr = request.GetParamValue(IdGlobalConstants.ExtraAuth.JWT_QUERY_KEY);
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(tknStr);
                var claims = jwtToken.Claims;
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                // Create authentication properties
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddHours(IdGlobalConstants.ExtraAuth.COOKIE_DURATION_HOURS)
                };

                // Add no-cache header
                context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
                context.Response.Headers.Pragma = "no-cache";
                //context.Response.Headers.Expires = "0";

                await context.SignInAsync(
                      CookieAuthenticationDefaults.AuthenticationScheme,
                      new ClaimsPrincipal(identity),
                      authProperties
                  );

                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), CookieAuthenticationDefaults.AuthenticationScheme);
                return AuthenticateResult.Success(ticket);
            }

            if (env.IsDevelopment() && _options.AllowExternalPagesDevModeAccess)
                return await DevModeAuthenticationAsync(context);

            return AuthenticateResult.NoResult();
        }
        catch (Exception)
        {

            return AuthenticateResult.NoResult();
        }
    }

    
    //-----------------------//


    /// <summary>
    /// Checks if the request is from an external page.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>True if the request is from an external page, otherwise false.</returns>
    public bool IsFromExternalPage(HttpRequest request) =>
        externalPageList.IsFromExternalPage(request);


    //-----------------------//


    private static async Task<AuthenticateResult> DevModeAuthenticationAsync(HttpContext context)
    {

        var claims = new[] { new Claim(ClaimTypes.Name, "DevUser") };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false,
            ExpiresUtc = DateTime.UtcNow.AddHours(1)
        };

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            authProperties
        );
        
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), CookieAuthenticationDefaults.AuthenticationScheme);
        
        return AuthenticateResult.Success(ticket);

    }


    //-----------------------//



}//Cls