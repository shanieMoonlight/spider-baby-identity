using ID.Infrastructure.Auth.AppAbs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace ID.Infrastructure.Auth;

public class CustomAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    IExternalPageAuthenticationService externalPageAuth,
    ILoggerFactory logger,
    UrlEncoder encoder) 
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "MyIdCustomAuthenticationScheme";


    //-----------------------//


    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        //Try External pages first (Swagger, Hangfire, they should have a jwt in the query params or else a cookie)
        var result = await externalPageAuth.TryAuthenticateAsync(Context);
        if (result.Succeeded)
            return result;

        // Try Jwt before cookie
        var jwtAuthResult = await TryJwtAuthenticationAsync();
        if (jwtAuthResult.Succeeded)
            return jwtAuthResult;

        // Fallback to Cookie authentication
        var cookieAuthResult = await TryCookieAuthenticationAsync();
        if (cookieAuthResult.Succeeded)
            return cookieAuthResult;

        return AuthenticateResult.NoResult();
    }


    //-----------------------//


    private async Task<AuthenticateResult> TryJwtAuthenticationAsync()
    {
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        return authHeader != null && authHeader.StartsWith("Bearer ")
            ? await Context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme)
            : AuthenticateResult.NoResult();
    }


    //-----------------------//


    private async Task<AuthenticateResult> TryCookieAuthenticationAsync() =>
        await Context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);


}//Cls
