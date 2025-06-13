using Microsoft.AspNetCore.Identity;
using MyResults;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace ID.Domain.Entities.AppUsers;

/// <summary>
/// Provides an abstraction for managing indiviual users.
/// Changing email, Phone etc. Generating User sp[ecific tokens , etc....
/// </summary>
/// <typeparam name="TUser"></typeparam>
public interface IIdUserMgmtService<TUser> where TUser : AppUser
{
    //- - - - - - - - - - - - //

    /// <summary>
    /// Increments the access failed count for the user as an asynchronous operation.
    /// If the failed access account is greater than or equal to the configured maximum number of attempts,
    /// the user will be locked out for the configured lockout time span.
    /// </summary>
    /// <param name="user">The user whose failed access count to increment.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/> of the operation.</returns>
    Task<GenResult<TUser>> AccessFailedAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Adds the specified <paramref name="claim"/> to the <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to add the claim to.</param>
    /// <param name="claim">The claim to add.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> AddClaimAsync(TUser user, Claim claim);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Adds the specified <paramref name="claims"/> to the <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to add the claim to.</param>
    /// <param name="claims">The claims to add.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> AddClaimsAsync(TUser user, IEnumerable<Claim> claims);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Adds an external <see cref="UserLoginInfo"/> to the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to add the login to.</param>
    /// <param name="login">The external <see cref="UserLoginInfo"/> to add to the specified <paramref name="user"/>.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> AddLoginAsync(TUser user, UserLoginInfo login);
    //- - - - - - - - - - - - //

    /// <summary>
    /// Adds the <paramref name="password"/> to the specified <paramref name="user"/> only if the user
    /// does not already have a password.
    /// </summary>
    /// <param name="user">The user whose password should be set.</param>
    /// <param name="password">The password to set.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> AddPasswordAsync(TUser user, string password);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Adds the specified <paramref name="user"/> to the named role.
    /// </summary>
    /// <param name="user">The user to add to the named role.</param>
    /// <param name="role">The name of the role to add the user to.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> AddToRoleAsync(TUser user, string role);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Adds the specified <paramref name="user"/> to the named roles.
    /// </summary>
    /// <param name="user">The user to add to the named roles.</param>
    /// <param name="roles">The name of the roles to add the user to.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> AddToRolesAsync(TUser user, IEnumerable<string> roles);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Updates a users emails if the specified email change <paramref name="token"/> is valid for the user.
    /// </summary>
    /// <param name="user">The user whose email should be updated.</param>
    /// <param name="newEmail">The new email address.</param>
    /// <param name="token">The change email token to be verified.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> ChangeEmailAsync(TUser user, string newEmail, string token);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Changes a user's password after confirming the specified <paramref name="currentPassword"/> is correct,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose password should be set.</param>
    /// <param name="currentPassword">The current password to validate before changing.</param>
    /// <param name="newPassword">The new password to set for the specified <paramref name="user"/>.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> ChangePasswordAsync(TUser user, string currentPassword, string newPassword);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Sets the phone number for the specified <paramref name="user"/> if the specified
    /// change <paramref name="token"/> is valid.
    /// </summary>
    /// <param name="user">The user whose phone number to set.</param>
    /// <param name="phoneNumber">The phone number to set.</param>
    /// <param name="token">The phone number confirmation token to validate.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> ChangePhoneNumberAsync(TUser user, string phoneNumber, string token);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Returns a flag indicating whether the given <paramref name="password"/> is valid for the
    /// specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose password should be validated.</param>
    /// <param name="password">The password to validate</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing true if
    /// the specified <paramref name="password" /> matches the one store for the <paramref name="user"/>,
    /// otherwise false.</returns>
    Task<bool> CheckPasswordAsync(TUser user, string? password);

    //- - - - - - - - - - - - //

    Task<IdentityResult> ConfirmEmailAsync(TUser user, string token);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Returns how many recovery code are still valid for a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>How many recovery code are still valid for a user.</returns>
    Task<int> CountRecoveryCodesAsync(TUser user);

    //- - - - - - - - - - - - //

    Task<byte[]> CreateSecurityTokenAsync(TUser user);

    //- - - - - - - - - - - - //
    
    Task<BasicResult> DeleteAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Releases all resources used by the user manager.
    /// </summary>
    void Dispose();


    //- - - - - - - - - - - - //

    Task<string> GenerateChangePhoneNumberTokenAsync(TUser user, string phoneNumber);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Generates a value suitable for use in concurrency tracking.
    /// </summary>
    /// <param name="user">The user to generate the stamp for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the security
    /// stamp for the specified <paramref name="user"/>.
    /// </returns>
    Task<string> GenerateConcurrencyStampAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Generates an email confirmation token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate an email confirmation token for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, an email confirmation token.
    /// </returns>
    Task<string> GenerateEmailConfirmationTokenAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Generates recovery codes for the user, this invalidates any previous recovery codes for the user.
    /// </summary>
    /// <param name="user">The user to generate recovery codes for.</param>
    /// <param name="number">The number of codes to generate.</param>
    /// <returns>The new recovery codes for the user.  Note: there may be less than number returned, as duplicates will be removed.</returns>
    Task<IEnumerable<string>?> GenerateNewTwoFactorRecoveryCodesAsync(TUser user, int number);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Generates a password reset token for the specified <paramref name="user"/>, using
    /// the configured password reset token provider.
    /// </summary>
    /// <param name="user">The user to generate a password reset token for.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation,
    /// containing a password reset token for the specified <paramref name="user"/>.</returns>
    Task<string> GeneratePasswordResetTokenAsync(TUser user);

    //- - - - - - - - - - - - //

    Task<string> GenerateTwoFactorTokenAsync(TUser user, string tokenProvider);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Retrieves the current number of failed accesses for the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose access failed count should be retrieved for.</param>
    /// <returns>The <see cref="Task"/> that contains the result the asynchronous operation, the current failed access count
    /// for the user.</returns>
    Task<int> GetAccessFailedCountAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Gets a list of <see cref="Claim"/>s to be belonging to the specified <paramref name="user"/> as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose claims to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <see cref="Claim"/>s.
    /// </returns>
    Task<IList<Claim>> GetClaimsAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a flag indicating whether user lockout can be enabled for the specified user.
    /// </summary>
    /// <param name="user">The user whose ability to be locked out should be returned.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, true if a user can be locked out, otherwise false.
    /// </returns>
    Task<bool> GetLockoutEnabledAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Gets the last <see cref="DateTimeOffset"/> a user's last lockout expired, if any.
    /// A time value in the past indicates a user is not currently locked out.
    /// </summary>
    /// <param name="user">The user whose lockout date should be retrieved.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the lookup, a <see cref="DateTimeOffset"/> containing the last time a user's lockout expired, if any.
    /// </returns>
    Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Retrieves the associated logins for the specified <param ref="user"/>.
    /// </summary>
    /// <param name="user">The user whose associated logins to retrieve.</param>
    /// <returns>
    /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
    /// </returns>
    Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Gets a list of role names the specified <paramref name="user"/> belongs to.
    /// </summary>
    /// <param name="user">The user whose role names to retrieve.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing a list of role names.</returns>
    Task<IList<string>> GetRolesAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get the security stamp for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose security stamp should be set.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the security stamp for the specified <paramref name="user"/>.</returns>
    Task<string> GetSecurityStampAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Returns a flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled or not,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose two factor authentication enabled status should be retrieved.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, true if the specified <paramref name="user "/>
    /// has two factor authentication enabled, otherwise false.
    /// </returns>
    Task<bool> GetTwoFactorEnabledAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Gets a list of valid two factor token providers for the specified <paramref name="user"/>,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user the whose two factor authentication providers will be returned.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents result of the asynchronous operation, a list of two
    /// factor authentication providers for the specified user.
    /// </returns>
    Task<IList<string>> GetValidTwoFactorProvidersAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Gets a flag indicating whether the specified <paramref name="user"/> has a password.
    /// </summary>
    /// <param name="user">The user to return a flag for, indicating whether they have a password or not.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a password
    /// otherwise false.
    /// </returns>
    Task<bool> HasPasswordAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Gets a flag indicating whether the email address for the specified <paramref name="user"/> has been verified, true if the email address is verified otherwise
    /// false.
    /// </summary>
    /// <param name="user">The user whose email confirmation status should be returned.</param>
    /// <returns>
    /// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user"/>
    /// has been confirmed or not.
    /// </returns>
    Task<bool> IsEmailConfirmedAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Returns a flag indicating whether the specified <paramref name="user"/> is a member of the given named role.
    /// </summary>
    /// <param name="user">The user whose role membership should be checked.</param>
    /// <param name="role">The name of the role to be checked.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified <paramref name="user"/> is
    /// a member of the named role.
    /// </returns>
    Task<bool> IsInRoleAsync(TUser user, string role);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Returns a flag indicating whether the specified <paramref name="user"/> is locked out,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose locked out status should be retrieved.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, true if the specified <paramref name="user "/>
    /// is locked out, otherwise false.
    /// </returns>
    Task<bool> IsLockedOutAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Gets a flag indicating whether the specified <paramref name="user"/>'s telephone number has been confirmed.
    /// </summary>
    /// <param name="user">The user to return a flag for, indicating whether their telephone number is confirmed.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a confirmed
    /// telephone number otherwise false.
    /// </returns>
    Task<bool> IsPhoneNumberConfirmedAsync(TUser user);

    //- - - - - - - - - - - - //

    [return: NotNullIfNotNull("name")]
    string? NormalizeName(string? name);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Returns whether a recovery code is valid for a user. Note: recovery codes are only valid
    /// once, and will be invalid after use.
    /// </summary>
    /// <param name="user">The user who owns the recovery code.</param>
    /// <param name="code">The recovery code to use.</param>
    /// <returns>True if the recovery code was found for the user.</returns>
    Task<GenResult<TUser>> RedeemTwoFactorRecoveryCodeAsync(TUser user, string code);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Removes the specified <paramref name="claim"/> from the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to remove the specified <paramref name="claim"/> from.</param>
    /// <param name="claim">The <see cref="Claim"/> to remove.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> RemoveClaimAsync(TUser user, Claim claim);

    //- - - - - - - - - - - - //
    /// <summary>
    /// Removes the specified <paramref name="claims"/> from the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to remove the specified <paramref name="claims"/> from.</param>
    /// <param name="claims">A collection of <see cref="Claim"/>s to remove.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Removes the specified <paramref name="user"/> from the named role.
    /// </summary>
    /// <param name="user">The user to remove from the named role.</param>
    /// <param name="role">The name of the role to remove the user from.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> RemoveFromRoleAsync(TUser user, string role);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Removes the specified <paramref name="user"/> from the named roles.
    /// </summary>
    /// <param name="user">The user to remove from the named roles.</param>
    /// <param name="roles">The name of the roles to remove the user from.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> RemoveFromRolesAsync(TUser user, IEnumerable<string> roles);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Attempts to remove the provided external login information from the specified <paramref name="user"/>.
    /// and returns a flag indicating whether the removal succeed or not.
    /// </summary>
    /// <param name="user">The user to remove the login information from.</param>
    /// <param name="loginProvider">The login provide whose information should be removed.</param>
    /// <param name="providerKey">The key given by the external login provider for the specified user.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> RemoveLoginAsync(TUser user, string loginProvider, string providerKey);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Removes a user's password.
    /// </summary>
    /// <param name="user">The user whose password should be removed.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> RemovePasswordAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Replaces the given <paramref name="claim"/> on the specified <paramref name="user"/> with the <paramref name="newClaim"/>
    /// </summary>
    /// <param name="user">The user to replace the claim on.</param>
    /// <param name="claim">The claim to replace.</param>
    /// <param name="newClaim">The new claim to replace the existing <paramref name="claim"/> with.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Resets the access failed count for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose failed access count should be reset.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/> of the operation.</returns>
    Task<GenResult<TUser>> ResetAccessFailedCountAsync(TUser user);

    //- - - - - - - - - - - - //

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
    Task<GenResult<TUser>> ResetPasswordAsync(TUser user, string token, string newPassword);

    //- - - - - - - - - - - - //
    /// <summary>
    /// Sets the <paramref name="email"/> address for a <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose email should be set.</param>
    /// <param name="email">The email to set.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> SetEmailAsync(TUser user, string? email);

    //- - - - - - - - - - - - //
    /// <summary>
    /// Sets a flag indicating whether the specified <paramref name="user"/> is locked out,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose locked out status should be set.</param>
    /// <param name="enabled">Flag indicating whether the user is locked out or not.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, the <see cref="GenResult<AppUser>"/> of the operation
    /// </returns>
    Task<GenResult<TUser>> SetLockoutEnabledAsync(TUser user, bool enabled);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Locks out a user until the specified end date has passed. Setting a end date in the past immediately unlocks a user.
    /// </summary>
    /// <param name="user">The user whose lockout date should be set.</param>
    /// <param name="lockoutEnd">The <see cref="DateTimeOffset"/> after which the <paramref name="user"/>'s lockout should end.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/> of the operation.</returns>
    Task<GenResult<TUser>> SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Sets the phone number for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose phone number to set.</param>
    /// <param name="phoneNumber">The phone number to set.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    Task<GenResult<TUser>> SetPhoneNumberAsync(TUser user, string? phoneNumber);

    //- - - - - - - - - - - - //
    /// <summary>
    /// Sets a flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled or not,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
    /// <param name="enabled">A flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, the <see cref="GenResult<AppUser>"/> of the operation
    /// </returns>
    Task<GenResult<TUser>> SetTwoFactorEnabledAsync(TUser user, bool enabled);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Updates the normalized email for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose email address should be normalized and updated.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task UpdateNormalizedEmailAsync(TUser user);

    //- - - - - - - - - - - - //
    /// <summary>
    /// Updates the normalized user name for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose user name should be normalized and updated.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task UpdateNormalizedUserNameAsync(TUser user);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Regenerates the security stamp for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose security stamp should be regenerated.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    /// <remarks>
    /// Regenerating a security stamp will sign out any saved login for the user.
    /// </remarks>
    Task<GenResult<TUser>> UpdateSecurityStampAsync(TUser user);

    //- - - - - - - - - - - - //
    /// <summary>
    /// Should return <see cref="BasicResult.Succeeded"/> if validation is successful. This is
    /// called before updating the password hash.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="password">The password.</param>
    /// <returns>A <see cref="BasicResult"/> representing whether validation was successful.</returns>
    Task<BasicResult> ValidatePasswordAsync(TUser user, string? password);

    //- - - - - - - - - - - - //

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
    Task<bool> VerifyChangePhoneNumberTokenAsync(TUser user, string token, string phoneNumber);

    //- - - - - - - - - - - - //

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
    Task<bool> VerifyTwoFactorTokenAsync(TUser user, string tokenProvider, string token);

    //- - - - - - - - - - - - //

}//Cls