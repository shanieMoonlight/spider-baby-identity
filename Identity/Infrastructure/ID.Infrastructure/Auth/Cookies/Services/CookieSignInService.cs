using ID.Application.AppAbs.SignIn;
using ID.Application.Features.Account.Cmd.Cookies.SignIn;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Infrastructure.Claims.Services.Abs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ID.Infrastructure.Auth.Cookies.Services;

/// <summary>
/// Service for handling cookie-based sign-in operations.
/// </summary>
/// <typeparam name="TUser">The type of the user.</typeparam>
internal class CookieSignInService<TUser>(
    SignInManager<TUser> _signInManager,
    IClaimsBuilderService _claimsBuilder,
    IHttpContextAccessor httpContextAccessor)
    : ICookieSignInService<TUser> where TUser : AppUser
{
    //-----------------------//

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
        bool twoFactorVerified,
        string? currentDeviceId = null)
    {
        HttpContext httpContext = httpContextAccessor.HttpContext!;
        var claims = await _claimsBuilder.BuildClaimsAsync(user, team, twoFactorVerified, currentDeviceId);
        var principal = CreateClaimsPrincipal(claims);
        var authProps = CreateAuthenticationProperties(isPersistent);

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);
    }

    //-----------------------//

    /// <summary>
    /// Signs in a user with two-factor authentication required.
    /// </summary>
    /// <param name="isPersistent">Whether the sign-in is persistent.</param>
    /// <param name="user">The user to sign in.</param>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="currentDeviceId">The ID of the current device.</param>
    public async Task SignInWithTwoFactorRequiredAsync(
        bool isPersistent,
        TUser user,
        Team team,
        string? currentDeviceId = null)
    {
        HttpContext httpContext = httpContextAccessor.HttpContext!;
        var claims = await _claimsBuilder.BuildClaimsWithTwoFactorRequiredAsync(user, team, currentDeviceId);
        var principal = CreateClaimsPrincipal(claims);
        var props = CreateAuthenticationProperties(isPersistent);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

        await _signInManager.SignInAsync(user, isPersistent, CookieAuthenticationDefaults.AuthenticationScheme);
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
