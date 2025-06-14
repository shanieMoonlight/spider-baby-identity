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
    IPreSignInService<AppUser> _preSignInService,
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
    /// Signs in a user using their user ID and password.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="password">The password.</param>
    /// <param name="isPersistent">Whether the sign-in is persistent.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the sign-in attempt.</returns>
    public async Task<MyIdSignInResult> PasswordAndIdSignInAsync(
        Guid userId,
        string password,
        bool isPersistent,
        CancellationToken cancellationToken = default)
    {
        var dto = new CookieSignInDto
        {
            UserId = userId,
            Password = password,
            RememberMe = isPersistent
        };
        return await PasswordSignInAsync(dto, cancellationToken);
    }

    //-----------------------//

    /// <summary>
    /// Signs in a user using their email and password.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="password">The password.</param>
    /// <param name="isPersistent">Whether the sign-in is persistent.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the sign-in attempt.</returns>
    public async Task<MyIdSignInResult> PasswordAndEmailSignInAsync(
        string email,
        string password,
        bool isPersistent,
        CancellationToken cancellationToken = default)
    {
        var dto = new CookieSignInDto
        {
            Email = email,
            Password = password,
            RememberMe = isPersistent
        };
        return await PasswordSignInAsync(dto, cancellationToken);
    }

    //-----------------------//

    /// <summary>
    /// Signs in a user using their username and password.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="isPersistent">Whether the sign-in is persistent.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the sign-in attempt.</returns>
    public async Task<MyIdSignInResult> PasswordAndUsernameSignInAsync(
        string username,
        string password,
        bool isPersistent,
        CancellationToken cancellationToken = default)
    {
        var dto = new CookieSignInDto
        {
            Username = username,
            Password = password,
            RememberMe = isPersistent
        };
        return await PasswordSignInAsync(dto, cancellationToken);
    }

    //-----------------------//

    /// <summary>
    /// Signs in a user using the provided sign-in DTO.
    /// </summary>
    /// <param name="dto">The sign-in DTO.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the sign-in attempt.</returns>
    public async Task<MyIdSignInResult> PasswordSignInAsync(CookieSignInDto dto, CancellationToken cancellationToken = default)
    {
        MyIdSignInResult signInResult = await _preSignInService.Authenticate(dto, cancellationToken);

        // Check if user exists
        if (!signInResult.Succeeded)
            return signInResult;

        var user = (signInResult.User as TUser)!; // TODO remove when fully Gentrified
        var team = signInResult.Team!;

        await SignInAsync(dto.RememberMe, user, team, false, dto.DeviceId);

        return signInResult;
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

}
