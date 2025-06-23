using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.Application.AppAbs.SignIn;
/// <summary>
/// Interface for handling cookie-based sign-in operations, including two-factor authentication and device tracking.
/// </summary>
/// <typeparam name="TUser">The type of the user.</typeparam>
public interface ICookieAuthService<TUser> where TUser : AppUser
{
    /// <summary>
    /// Sets cookies to require two-factor authentication for the user, including a two-factor token, remember-me flag, and optional device ID.
    /// </summary>
    /// <param name="isPersistent">Whether the authentication should persist across browser sessions (Remember Me).</param>
    /// <param name="user">The user for whom to require two-factor authentication.</param>
    /// <param name="currentDeviceId">Optional device identifier to associate with the session.</param>
    /// <remarks>
    /// This does not sign in the user, but sets cookies to indicate that two-factor authentication is required.
    /// </remarks>
    Task CreateWithTwoFactorRequiredAsync(bool isPersistent, TUser user, string? currentDeviceId = null);

    /// <summary>
    /// Reads the 'Remember Me' flag from the authentication cookies.
    /// </summary>
    /// <returns>True if the persistent flag is set in the cookies; otherwise, false.</returns>
    bool GetRememberMe();

    /// <summary>
    /// Signs in a user by clearing any two-factor cookies and issuing a new authentication cookie with claims.
    /// </summary>
    /// <param name="isPersistent">Whether the sign-in is persistent (Remember Me).</param>
    /// <param name="user">The user to sign in.</param>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="twoFactorVerified">Whether two-factor authentication has been verified.</param>
    /// <param name="currentDeviceId">Optional device identifier to associate with the session.</param>
    /// <remarks>
    /// This method clears any two-factor authentication cookies and signs in the user with the specified claims.
    /// </remarks>
    Task SignInAsync(bool isPersistent, TUser user, Team team, bool twoFactorVerified, string? currentDeviceId = null);

    /// <summary>
    /// Signs out the current user from the cookie authentication scheme.
    /// </summary>
    Task SignOutAsync();

    /// <summary>
    /// Attempts to read the device ID from the authentication cookies.
    /// </summary>
    /// <returns>The device ID if present; otherwise, null.</returns>
    string? TryGetDeviceId();

    /// <summary>
    /// Attempts to read the two-factor authentication token from the cookies.
    /// </summary>
    /// <returns>The two-factor token if present; otherwise, null.</returns>
    string? TryGetTwoFactorToken();

}
