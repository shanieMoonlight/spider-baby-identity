using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Identity;
using MyResults;
using System.Text;

namespace ID.Infrastructure.TokenServices.Abstractions;
internal class IdTokenService<TUser>(UserManager<TUser> _userMgr) where TUser : AppUser
{
    /// <summary>
    /// Creates bytes to use as a security token from the user's security stamp.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>The security token bytes.</returns>
    public virtual async Task<byte[]> CreateSecurityTokenAsync(TUser user) =>
        Encoding.Unicode.GetBytes(await _userMgr.GetSecurityStampAsync(user).ConfigureAwait(false));

    //-----------------------//

    /// <summary>
    /// Generates an email confirmation token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate an email confirmation token for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, an email confirmation token.
    /// </returns>
    public async Task<string> GenerateEmailConfirmationTokenAsync(TUser user) =>
        await _userMgr.GenerateEmailConfirmationTokenAsync(user);

    //-----------------------//

    /// <summary>
    /// Generates a password reset token for the specified <paramref name="user"/>, using
    /// the configured password reset token provider.
    /// </summary>
    /// <param name="user">The user to generate a password reset token for.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation,
    /// containing a password reset token for the specified <paramref name="user"/>.</returns>
    public virtual async Task<string> GeneratePasswordResetTokenAsync(TUser user) =>
        await _userMgr.GeneratePasswordResetTokenAsync(user);

    //-----------------------//

    /// <summary>
    /// Resets the <paramref name="user"/>'s password to the specified <paramref name="newPassword"/> after
    /// validating the given password reset <paramref name="token"/>.
    /// </summary>
    /// <param name="user">The user whose password should be reset.</param>
    /// <param name="token">The password reset token to verify.</param>
    /// <param name="newPassword">The new password to set if reset token verification succeeds.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<GenResult<TUser>> ResetPasswordAsync(TUser user, string token, string newPassword) =>
        (await _userMgr.ResetPasswordAsync(user, token, newPassword)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Returns a flag indicating whether the specified <paramref name="user"/>'s phone number change verification
    /// token is valid for the given <paramref name="phoneNumber"/>.
    /// </summary>
    /// <param name="user">The user to validate the token against.</param>
    /// <param name="token">The telephone number change token to validate.</param>
    /// <param name="phoneNumber">The telephone number the token was generated for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, returning true if the <paramref name="token"/>
    /// is valid, otherwise false.
    /// </returns>
    public async Task<bool> VerifyChangePhoneNumberTokenAsync(TUser user, string token, string phoneNumber) =>
        await _userMgr.VerifyChangePhoneNumberTokenAsync(user, token, phoneNumber);


    //-----------------------//

    /// <summary>
    /// Generates a telephone number change token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate a telephone number token for.</param>
    /// <param name="phoneNumber">The new phone number the validation token should be sent to.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the telephone change number token.
    /// </returns>
    public async Task<string> GenerateChangePhoneNumberTokenAsync(TUser user, string phoneNumber) =>
        await _userMgr.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

    //-----------------------//

    /// <summary>
    /// Gets a two factor authentication token for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user the token is for.</param>
    /// <param name="tokenProvider">The provider which will generate the token.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents result of the asynchronous operation, a two factor authentication token
    /// for the user.
    /// </returns>
    public async Task<string> GenerateTwoFactorTokenAsync(TUser user, string tokenProvider)
        => await _userMgr.GenerateTwoFactorTokenAsync(user, tokenProvider);

    //-----------------------//

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
    public async Task<bool> VerifyTwoFactorTokenAsync(TUser user, string tokenProvider, string token) =>
        await _userMgr.VerifyTwoFactorTokenAsync(user, tokenProvider, token);

    //-----------------------//

}//Cls
