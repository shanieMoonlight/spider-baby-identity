using ID.Application.Features.Account.Cmd.Cookies.SignIn;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.Application.AppAbs.SignIn;
/// <summary>
/// Interface for handling cookie-based sign-in operations.
/// </summary>
/// <typeparam name="TUser">The type of the user.</typeparam>
public interface ICookieSignInService<TUser> where TUser : AppUser
{
    ///// <summary>
    ///// Signs in a user using their email and password.
    ///// </summary>
    ///// <param name="email">The email.</param>
    ///// <param name="password">The password.</param>
    ///// <param name="isPersistent">Whether the sign-in is persistent.</param>
    ///// <param name="cancellationToken">The cancellation token.</param>
    ///// <returns>The result of the sign-in attempt.</returns>
    //Task<MyIdSignInResult> PasswordAndEmailSignInAsync(string email, string password, bool isPersistent, CancellationToken cancellationToken = default);

    ////- - - - - - - - - - - - - - - - - - // 

    ///// <summary>
    ///// Signs in a user using their user ID and password.
    ///// </summary>
    ///// <param name="userId">The user ID.</param>
    ///// <param name="password">The password.</param>
    ///// <param name="isPersistent">Whether the sign-in is persistent.</param>
    ///// <param name="cancellationToken">The cancellation token.</param>
    ///// <returns>The result of the sign-in attempt.</returns>
    //Task<MyIdSignInResult> PasswordAndIdSignInAsync(Guid userId, string password, bool isPersistent, CancellationToken cancellationToken = default);

    ////- - - - - - - - - - - - - - - - - - // 

    ///// <summary>
    ///// Signs in a user using their username and password.
    ///// </summary>
    ///// <param name="username">The username.</param>
    ///// <param name="password">The password.</param>
    ///// <param name="isPersistent">Whether the sign-in is persistent.</param>
    ///// <param name="cancellationToken">The cancellation token.</param>
    ///// <returns>The result of the sign-in attempt.</returns>
    //Task<MyIdSignInResult> PasswordAndUsernameSignInAsync(string username, string password, bool isPersistent, CancellationToken cancellationToken = default);

    ////- - - - - - - - - - - - - - - - - - // 

    ///// <summary>
    ///// Signs in a user using the provided sign-in DTO.
    ///// </summary>
    ///// <param name="dto">The sign-in DTO.</param>
    ///// <param name="cancellationToken">The cancellation token.</param>
    ///// <returns>The result of the sign-in attempt.</returns>
    //Task<MyIdSignInResult> PasswordSignInAsync(CookieSignInDto dto, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - - - - - - - // 

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
