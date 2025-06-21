using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using MyResults;

namespace ID.Application.AppAbs.TokenVerificationServices;
public interface ITwoFactorVerificationService<TUser> where TUser : AppUser
{
    /// <summary>
    /// Disable TwoFactor Authentication on this <paramref name="user"/>
    /// </summary>
    /// <param name="user">The user whose email confirmation status should be returned.</param>
    /// <returns>
    /// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user"/>
    /// has been confirmed or not.
    /// </returns>
    Task<GenResult<TUser>> DisableTwoFactorTokenAsync(TUser user);

    //------------------------------------//

    /// <summary>
    /// Enable TwoFactor Authentication on this <paramref name="user"/>
    /// </summary>
    /// <param name="user">The user whose email confirmation status should be returned.</param>
    /// <returns>
    /// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user"/>
    /// has been confirmed or not.
    /// </returns>
    Task<GenResult<TUser>> EnableTwoFactorTokenAsync(TUser user);

    //------------------------------------//


    /// <summary>
    /// Gets a two factor authentication token for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user the token is for.</param>
    /// <param name="tokenProvider">The provider which will generate the token.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents result of the asynchronous operation, a two factor authentication token
    /// for the user.
    /// </returns>
    Task<string> GenerateTwoFactorTokenAsync(Team team, TUser user, string tokenProvider);

    //------------------------------------//

    /// <summary>
    /// Gets the first two factor provider on  for this user.
    /// </summary>
    /// <param name="user">The user the whose two factor authentication providers will be returned.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents result of the asynchronous operation, a list of two
    /// factor authentication providers for the specified user.
    /// </returns>
    Task<string?> GetFirstValidTwoFactorProviderAsync(TUser user);

    //------------------------------------//

    /// <summary>
    /// Returns a flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled or not,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose two factor authentication enabled status should be retrieved.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, true if the specified <paramref name="user "/>
    /// has two factor authentication enabled, otherwise false.
    /// </returns>
    Task<bool> IsTwoFactorEnabledAsync(TUser user);

    //------------------------------------//

    /// <summary>
    /// Gets a flag indicating whether the email address for the specified <paramref name="user"/> has been verified, true if the email address is verified otherwise
    /// false.
    /// </summary>
    /// <param name="user">The user whose email confirmation status should be returned.</param>
    /// <returns>
    /// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user"/>
    /// has been confirmed or not.
    /// </returns>
    Task<GenResult<TUser>> SetTwoFactorEnabledAsync(TUser user, bool enabled);

    //------------------------------------//

    /// <summary>
    /// Verifies the specified two factor authentication <paramref name="token" /> against the <paramref name="user"/>. using
    /// the provider on the <paramref name="user"/>
    /// </summary>
    /// <param name="user">The user the token is supposed to be for.</param>
    /// <param name="token">The token to verify.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents result of the asynchronous operation, true if the token is valid,
    /// otherwise false.
    /// </returns>
    Task<bool> VerifyTwoFactorTokenAsync(Team team, TUser user, string code);

    //------------------------------------//

    /// <summary>
    /// Verifies the specified two factor authentication <paramref name="token" /> against the <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user the token is supposed to be for.</param>
    /// <param name="tokenProvider">The provider which will verify the token.</param>
    /// <param name="token">The token to verify.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents result of the asynchronous operation, true if the token is valid,
    /// otherwise false.
    /// </returns>
    Task<bool> VerifyTwoFactorTokenAsync(Team team, TUser user, TwoFactorProvider tokenProvider, string token);

    //------------------------------------//
}