using ID.Application.AppAbs.SignIn;
using ID.Application.MFA;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Infrastructure.Claims.Services.Abs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ID.Infrastructure.Auth.Cookies.Services;

/// <summary>
/// Service for handling cookie-based sign-in operations.
/// </summary>
/// <typeparam name="TUser">The type of the user.</typeparam>
internal class CookieAuthService<TUser>(
    IClaimsBuilderService _claimsBuilder,
    ITwofactorUserIdCacheService _twofactorUserIdCache,
    IHttpContextAccessor httpContextAccessor)
    : ICookieAuthService<TUser> where TUser : AppUser
{

    /// <summary>
    /// Signs in a user with the specified parameters.
    /// </summary>
    /// <param name="isPersistent">Whether the sign-in is persistent.</param>
    /// <param name="user">The user to sign in.</param>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="twoFactorVerified">Whether two-factor authentication is verified.</param>
    /// <param name="currentDeviceId">The ID of the current device.</param>
    public async Task SignInAsync(
        bool isPersistent,
        TUser user,
        Team team,
        string? currentDeviceId = null)
    {
        HttpContext httpContext = httpContextAccessor.HttpContext!;

        // Clear any existing 2-factor stuff 
        httpContext.Response.Cookies.Delete(CookieConstants.TwoFactorTokenKey);
        httpContext.Response.Cookies.Delete(CookieConstants.IsPersistentKey);
        httpContext.Response.Cookies.Delete(CookieConstants.DeviceIdKey);

        var claims = await _claimsBuilder.BuildClaimsAsync(user, team, currentDeviceId);
        var principal = CreateClaimsPrincipal(claims);
        var authProps = CreateAuthenticationProperties(isPersistent);


        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);
    }

    //-----------------------//

    /// <summary>
    /// Signs in a user with two-factor authentication required.
    /// </summary>
    /// <param name="user">The user to sign in.</param>
    public Task CreateWithTwoFactorRequiredAsync(
        bool isPersistent,
        TUser user,
        string? currentDeviceId = null)
    {
        HttpContext httpContext = httpContextAccessor.HttpContext!;
        string twoFactorToken = _twofactorUserIdCache.StoreUserId(user.Id);
        Microsoft.AspNetCore.Http.CookieOptions options = new()
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps, // Use secure cookies in HTTPS
            SameSite = SameSiteMode.Strict, // Set SameSite to Strict for better security
            Expires = DateTimeOffset.UtcNow.AddMinutes(CookieConstants.TwoFactorCookieDurationMins)
        };

        httpContext.Response.Cookies.Append(CookieConstants.TwoFactorTokenKey, twoFactorToken, options);
        httpContext.Response.Cookies.Append(CookieConstants.IsPersistentKey, isPersistent.ToString(), options);
        if (!string.IsNullOrWhiteSpace(currentDeviceId))
            httpContext.Response.Cookies.Append(CookieConstants.DeviceIdKey, currentDeviceId, options);


        return Task.CompletedTask;
    }

    //-----------------------//

    public string? TryGetTwoFactorToken()
    {
        httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(CookieConstants.TwoFactorTokenKey, out string? value);
        return value;
    }

    //-----------------------//

    public bool GetRememberMe()
    {
        if (!httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(CookieConstants.IsPersistentKey, out string? value))
            return default;

        if (!bool.TryParse(value, out var result))
            return default;

        return result;
    }

    //-----------------------//

    public string? TryGetDeviceId()
    {
        httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(CookieConstants.DeviceIdKey, out string? value);
        return value;
    }

    //-----------------------//

    /// <summary>
    /// Signs out the current user.
    /// </summary>
    public async Task SignOutAsync() =>
        await httpContextAccessor
            .HttpContext!
            .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    //-----------------------//

    /// <summary>
    /// Creates a claims principal from a list of claims.
    /// </summary>
    /// <param name="claims">The list of claims.</param>
    /// <returns>The created claims principal.</returns>
    private static ClaimsPrincipal CreateClaimsPrincipal(List<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }

    //-----------------------//

    /// <summary>
    /// Creates authentication properties.
    /// </summary>
    /// <param name="isPersistent">Whether the authentication is persistent.</param>
    /// <returns>The created authentication properties.</returns>
    private static AuthenticationProperties CreateAuthenticationProperties(bool isPersistent) =>
        new()
        {
            IsPersistent = isPersistent
        };

}//Cls
