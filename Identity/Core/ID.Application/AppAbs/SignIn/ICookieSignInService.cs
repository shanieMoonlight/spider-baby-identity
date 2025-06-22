using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.Application.AppAbs.SignIn;
/// <summary>
/// Interface for handling cookie-based sign-in operations.
/// </summary>
/// <typeparam name="TUser">The type of the user.</typeparam>
public interface ICookieSignInService<TUser> where TUser : AppUser
{
    /// <summary>
    /// Signs in a user with the specified parameters.
    /// </summary>
    /// <param name="isPersistent">Whether the sign-in is persistent.</param>
    /// <param name="user">The user to sign in.</param>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="twoFactorVerified">Whether two-factor authentication is verified.</param>
    /// <param name="currentDeviceId">The ID of the current device.</param>
    Task SignInAsync(bool isPersistent, TUser user, Team team, bool twoFactorVerified, string? currentDeviceId = null);

    //- - - - - - - - - - - - - - - - - - // 

    /// <summary>
    /// Signs in a user with two-factor authentication required.
    /// </summary>
    /// <param name="isPersistent">Whether the sign-in is persistent.</param>
    /// <param name="user">The user to sign in.</param>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="currentDeviceId">The ID of the current device.</param>
    Task SignInWithTwoFactorRequiredAsync(bool isPersistent, TUser user, Team team, string? currentDeviceId = null);

    //- - - - - - - - - - - - - - - - - - // 

    /// <summary>
    /// Signs out the current user.
    /// </summary>
    Task SignOutAsync();

    //- - - - - - - - - - - - - - - - - - // 
}
