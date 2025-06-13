using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Identity;
using MyResults;
using StringHelpers;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text;

namespace ID.Infrastructure.DomainServices.Members;
internal class MyIdUserMgmtService<TUser>(UserManager<TUser> _userMgr) : IIdUserMgmtService<TUser> where TUser : AppUser
{
    //-----------------------//

    /// <summary>
    /// Releases all resources used by the user manager.
    /// </summary>
    public void Dispose() => _userMgr.Dispose();

    //-----------------------//

    /// <summary>
    /// Generates a value suitable for use in concurrency tracking.
    /// </summary>
    /// <param name="user">The user to generate the stamp for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the security
    /// stamp for the specified <paramref name="user"/>.
    /// </returns>
    public virtual async Task<string> GenerateConcurrencyStampAsync(TUser user) => await _userMgr.GenerateConcurrencyStampAsync(user);

    //-----------------------//

    ///// <summary>
    ///// Updates the specified <paramref name="user"/> in the backing store.
    ///// </summary>
    ///// <param name="user">The user to update.</param>
    ///// <returns>
    ///// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    ///// of the operation.
    ///// </returns>
    //public virtual async Task<GenResult<TUser>> UpdateAsync(TUser user) =>
    //    (await _userMgr.UpdateAsync(user)).ToGenResult(user);

    //-----------------------//


    /// <summary>
    /// Normalize user or role name for consistent comparisons.
    /// </summary>
    /// <param name="name">The name to normalize.</param>
    /// <returns>A normalized value representing the specified <paramref name="name"/>.</returns>
    [return: NotNullIfNotNull(nameof(name))]
    public virtual string? NormalizeName(string? name) => _userMgr.NormalizeName(name);

    //-----------------------//

    /// <summary>
    /// Normalize email for consistent comparisons.
    /// </summary>
    /// <param name="email">The email to normalize.</param>
    /// <returns>A normalized value representing the specified <paramref name="email"/>.</returns>
    [return: NotNullIfNotNull(nameof(email))]
    public virtual string? NormalizeEmail(string? email) => _userMgr.NormalizeEmail(email);

    //-----------------------//

    /// <summary>
    /// Updates the normalized user name for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose user name should be normalized and updated.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public virtual async Task UpdateNormalizedUserNameAsync(TUser user) =>
        await _userMgr.UpdateNormalizedUserNameAsync(user);

    //-----------------------//
    
    /// <summary>
    /// Returns a flag indicating whether the given <paramref name="password"/> is valid for the
    /// specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose password should be validated.</param>
    /// <param name="password">The password to validate</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing true if
    /// the specified <paramref name="password" /> matches the one store for the <paramref name="user"/>,
    /// otherwise false.</returns>
    public async Task<bool> CheckPasswordAsync(TUser user, string? password)
    {
        if (password.IsNullOrWhiteSpace())
            return false;
        return await _userMgr.CheckPasswordAsync(user, password!);
    }

    //-----------------------//

    /// <summary>
    /// Should return <see cref="BasicResult.Succeeded"/> if validation is successful. This is
    /// called before updating the password hash.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="password">The password.</param>
    /// <returns>A <see cref="BasicResult"/> representing whether validation was successful.</returns>
    public async Task<BasicResult> ValidatePasswordAsync(TUser user, string? password)
    {
        var errors = new List<IdentityError>();
        var isValid = true;

        foreach (var v in _userMgr.PasswordValidators)
        {
            var result = await v.ValidateAsync(_userMgr, user, password);
            if (!result.Succeeded)
            {
                if (result.Errors.Any())
                    errors.AddRange(result.Errors);

                isValid = false;
            }
        }//foreach

        if (!isValid)
            return IdentityResult.Failed([.. errors]).ToBasicResult();

        return IdentityResult.Success.ToBasicResult();
    }

    //-----------------------//

    /// <summary>
    /// Deletes the specified <paramref name="user"/> from the backing store.
    /// </summary>
    /// <param name="user">The user to delete.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="BasicResult"/>
    /// of the operation.
    /// </returns>
    public async Task<BasicResult> DeleteAsync(TUser user) =>
        (await _userMgr.DeleteAsync(user))
      .ToBasicResult();

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the specified <paramref name="user"/> has a password.
    /// </summary>
    /// <param name="user">The user to return a flag for, indicating whether they have a password or not.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a password
    /// otherwise false.
    /// </returns>
    public virtual async Task<bool> HasPasswordAsync(TUser user) =>
        await _userMgr.HasPasswordAsync(user);

    //-----------------------//

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
    public virtual async Task<GenResult<TUser>> AddPasswordAsync(TUser user, string password) =>
        (await _userMgr.AddPasswordAsync(user, password)).ToGenResult(user);

    //-----------------------//

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
    public virtual async Task<GenResult<TUser>> ChangePasswordAsync(TUser user, string currentPassword, string newPassword) =>
        (await _userMgr.ChangePasswordAsync(user, currentPassword, newPassword)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Removes a user's password.
    /// </summary>
    /// <param name="user">The user whose password should be removed.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<GenResult<TUser>> RemovePasswordAsync(TUser user) =>
        (await _userMgr.RemovePasswordAsync(user)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Get the security stamp for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose security stamp should be set.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the security stamp for the specified <paramref name="user"/>.</returns>
    public virtual async Task<string> GetSecurityStampAsync(TUser user) =>
        await _userMgr.GetSecurityStampAsync(user);

    //-----------------------//

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
    public virtual async Task<GenResult<TUser>> UpdateSecurityStampAsync(TUser user) =>
        (await _userMgr.UpdateSecurityStampAsync(user)).ToGenResult(user);

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
    /// Retrieves the user associated with the specified external login provider and login provider key.
    /// </summary>
    /// <param name="loginProvider">The login provider who provided the <paramref name="providerKey"/>.</param>
    /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
    /// <returns>
    /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified login provider and key.
    /// </returns>
    public virtual async Task<TUser?> FindByLoginAsync(string loginProvider, string providerKey) =>
        await _userMgr.FindByLoginAsync(loginProvider, providerKey);

    //-----------------------//

    /// <summary>
    /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">The user ID to search for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
    /// </returns>
    public virtual async Task<TUser?> FindByIdAsync(Guid userId) =>
        await _userMgr.FindByIdAsync(userId.ToString());

    //-----------------------//

    /// <summary>
    /// Finds and returns a user, if any, who has the specified user name.
    /// </summary>
    /// <param name="userName">The user name to search for.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userName"/> if it exists.
    /// </returns>
    public virtual async Task<TUser?> FindByNameAsync(string userName) =>
        await _userMgr.FindByNameAsync(userName);

    //-----------------------//

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
    public virtual async Task<GenResult<TUser>> RemoveLoginAsync(TUser user, string loginProvider, string providerKey) =>
        (await _userMgr.RemoveLoginAsync(user, loginProvider, providerKey)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Adds an external <see cref="UserLoginInfo"/> to the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to add the login to.</param>
    /// <param name="login">The external <see cref="UserLoginInfo"/> to add to the specified <paramref name="user"/>.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<GenResult<TUser>> AddLoginAsync(TUser user, UserLoginInfo login) =>
        (await _userMgr.AddLoginAsync(user, login)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Retrieves the associated logins for the specified <param ref="user"/>.
    /// </summary>
    /// <param name="user">The user whose associated logins to retrieve.</param>
    /// <returns>
    /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
    /// </returns>
    public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user) =>
        await _userMgr.GetLoginsAsync(user);

    /// <summary>
    /// Adds the specified <paramref name="claim"/> to the <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to add the claim to.</param>
    /// <param name="claim">The claim to add.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<GenResult<TUser>> AddClaimAsync(TUser user, Claim claim) =>
        (await _userMgr.AddClaimAsync(user, claim)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Adds the specified <paramref name="claims"/> to the <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to add the claim to.</param>
    /// <param name="claims">The claims to add.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<GenResult<TUser>> AddClaimsAsync(TUser user, IEnumerable<Claim> claims) =>
        (await _userMgr.AddClaimsAsync(user, claims)).ToGenResult(user);

    //-----------------------//

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
    public virtual async Task<GenResult<TUser>> ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim) =>
        (await _userMgr.ReplaceClaimAsync(user, claim, newClaim)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Removes the specified <paramref name="claim"/> from the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to remove the specified <paramref name="claim"/> from.</param>
    /// <param name="claim">The <see cref="Claim"/> to remove.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<GenResult<TUser>> RemoveClaimAsync(TUser user, Claim claim) =>
        (await _userMgr.RemoveClaimAsync(user, claim)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Removes the specified <paramref name="claims"/> from the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to remove the specified <paramref name="claims"/> from.</param>
    /// <param name="claims">A collection of <see cref="Claim"/>s to remove.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<GenResult<TUser>> RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims) =>
        (await _userMgr.RemoveClaimsAsync(user, claims)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Gets a list of <see cref="Claim"/>s to be belonging to the specified <paramref name="user"/> as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose claims to retrieve.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <see cref="Claim"/>s.
    /// </returns>
    public virtual async Task<IList<Claim>> GetClaimsAsync(TUser user) =>
        await _userMgr.GetClaimsAsync(user);

    //-----------------------//

    /// <summary>
    /// Add the specified <paramref name="user"/> to the named role.
    /// </summary>
    /// <param name="user">The user to add to the named role.</param>
    /// <param name="role">The name of the role to add the user to.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<GenResult<TUser>> AddToRoleAsync(TUser user, string role) =>
        (await _userMgr.AddToRoleAsync(user, role)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Add the specified <paramref name="user"/> to the named roles.
    /// </summary>
    /// <param name="user">The user to add to the named roles.</param>
    /// <param name="roles">The name of the roles to add the user to.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<GenResult<TUser>> AddToRolesAsync(TUser user, IEnumerable<string> roles) =>
        (await _userMgr.AddToRolesAsync(user, roles)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Removes the specified <paramref name="user"/> from the named role.
    /// </summary>
    /// <param name="user">The user to remove from the named role.</param>
    /// <param name="role">The name of the role to remove the user from.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public async Task<GenResult<TUser>> RemoveFromRoleAsync(TUser user, string role) =>
        (await _userMgr.RemoveFromRoleAsync(user, role)).ToGenResult(user);

    //-----------------------//


    /// <summary>
    /// Removes the specified <paramref name="user"/> from the named roles.
    /// </summary>
    /// <param name="user">The user to remove from the named roles.</param>
    /// <param name="roles">The name of the roles to remove the user from.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<GenResult<TUser>> RemoveFromRolesAsync(TUser user, IEnumerable<string> roles) =>
        (await _userMgr.RemoveFromRolesAsync(user, roles)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Gets a list of role names the specified <paramref name="user"/> belongs to.
    /// </summary>
    /// <param name="user">The user whose role names to retrieve.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing a list of role names.</returns>
    public async Task<IList<string>> GetRolesAsync(TUser user) =>
        await _userMgr.GetRolesAsync(user);

    //-----------------------//

    /// <summary>
    /// Returns a flag indicating whether the specified <paramref name="user"/> is a member of the given named role.
    /// </summary>
    /// <param name="user">The user whose role membership should be checked.</param>
    /// <param name="role">The name of the role to be checked.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified <paramref name="user"/> is
    /// a member of the named role.
    /// </returns>
    public async Task<bool> IsInRoleAsync(TUser user, string role) =>
        await _userMgr.IsInRoleAsync(user, role);

    //-----------------------//

    /// <summary>
    /// Validates that an email confirmation token matches the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user to validate the token against.</param>
    /// <param name="token">The email confirmation token to validate.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
    /// of the operation.
    /// </returns>
    public async Task<IdentityResult> ConfirmEmailAsync(TUser user, string token) => await _userMgr.ConfirmEmailAsync(user, token);

    //-----------------------//

    /// <summary>
    /// Sets the <paramref name="email"/> address for a <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose email should be set.</param>
    /// <param name="email">The email to set.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public async Task<GenResult<TUser>> SetEmailAsync(TUser user, string? email) =>
        (await _userMgr.SetEmailAsync(user, email)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Updates the normalized email for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose email address should be normalized and updated.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public async Task UpdateNormalizedEmailAsync(TUser user) =>
        await _userMgr.UpdateNormalizedEmailAsync(user);

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
    /// Gets a flag indicating whether the email address for the specified <paramref name="user"/> has been verified, true if the email address is verified otherwise
    /// false.
    /// </summary>
    /// <param name="user">The user whose email confirmation status should be returned.</param>
    /// <returns>
    /// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user"/>
    /// has been confirmed or not.
    /// </returns>
    public virtual async Task<bool> IsEmailConfirmedAsync(TUser user) =>
        await _userMgr.IsEmailConfirmedAsync(user);

    //-----------------------//

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
    public virtual async Task<GenResult<TUser>> ChangeEmailAsync(TUser user, string newEmail, string token) =>
        (await _userMgr.ChangeEmailAsync(user, newEmail, token)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Sets the phone number for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose phone number to set.</param>
    /// <param name="phoneNumber">The phone number to set.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/>
    /// of the operation.
    /// </returns>
    public virtual async Task<GenResult<TUser>> SetPhoneNumberAsync(TUser user, string? phoneNumber) =>
        (await _userMgr.SetPhoneNumberAsync(user, phoneNumber)).ToGenResult(user);

    //-----------------------//

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
    public virtual async Task<GenResult<TUser>> ChangePhoneNumberAsync(TUser user, string phoneNumber, string token) =>
        (await _userMgr.ChangePhoneNumberAsync(user, phoneNumber, token)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the specified <paramref name="user"/>'s telephone number has been confirmed.
    /// </summary>
    /// <param name="user">The user to return a flag for, indicating whether their telephone number is confirmed.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a confirmed
    /// telephone number otherwise false.
    /// </returns>
    public virtual async Task<bool> IsPhoneNumberConfirmedAsync(TUser user) =>
        await _userMgr.IsPhoneNumberConfirmedAsync(user);

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
    /// Gets a list of valid two factor token providers for the specified <paramref name="user"/>,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user the whose two factor authentication providers will be returned.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents result of the asynchronous operation, a list of two
    /// factor authentication providers for the specified user.
    /// </returns>
    public virtual async Task<IList<string>> GetValidTwoFactorProvidersAsync(TUser user) =>
        await _userMgr.GetValidTwoFactorProvidersAsync(user);

    //-----------------------//

    /// <summary>
    /// Returns a flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled or not,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose two factor authentication enabled status should be retrieved.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, true if the specified <paramref name="user "/>
    /// has two factor authentication enabled, otherwise false.
    /// </returns>
    public virtual async Task<bool> GetTwoFactorEnabledAsync(TUser user) =>
        await _userMgr.GetTwoFactorEnabledAsync(user);

    //-----------------------//

    /// <summary>
    /// Sets a flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled or not,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
    /// <param name="enabled">A flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, the <see cref="GenResult<AppUser>"/> of the operation
    /// </returns>
    public virtual async Task<GenResult<TUser>> SetTwoFactorEnabledAsync(TUser user, bool enabled) =>
        (await _userMgr.SetTwoFactorEnabledAsync(user, enabled)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Returns a flag indicating whether the specified <paramref name="user"/> is locked out,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose locked out status should be retrieved.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, true if the specified <paramref name="user "/>
    /// is locked out, otherwise false.
    /// </returns>
    public virtual async Task<bool> IsLockedOutAsync(TUser user) =>
        await _userMgr.IsLockedOutAsync(user);

    //-----------------------//

    /// <summary>
    /// Sets a flag indicating whether the specified <paramref name="user"/> is locked out,
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose locked out status should be set.</param>
    /// <param name="enabled">Flag indicating whether the user is locked out or not.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, the <see cref="GenResult<AppUser>"/> of the operation
    /// </returns>
    public virtual async Task<GenResult<TUser>> SetLockoutEnabledAsync(TUser user, bool enabled) =>
        (await _userMgr.SetLockoutEnabledAsync(user, enabled)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Retrieves a flag indicating whether user lockout can be enabled for the specified user.
    /// </summary>
    /// <param name="user">The user whose ability to be locked out should be returned.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, true if a user can be locked out, otherwise false.
    /// </returns>
    public virtual async Task<bool> GetLockoutEnabledAsync(TUser user) =>
        await _userMgr.GetLockoutEnabledAsync(user);

    //-----------------------//

    /// <summary>
    /// Gets the last <see cref="DateTimeOffset"/> a user's last lockout expired, if any.
    /// A time value in the past indicates a user is not currently locked out.
    /// </summary>
    /// <param name="user">The user whose lockout date should be retrieved.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the lookup, a <see cref="DateTimeOffset"/> containing the last time a user's lockout expired, if any.
    /// </returns>
    public virtual async Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user) =>
        await _userMgr.GetLockoutEndDateAsync(user);

    //-----------------------//

    /// <summary>
    /// Locks out a user until the specified end date has passed. Setting a end date in the past immediately unlocks a user.
    /// </summary>
    /// <param name="user">The user whose lockout date should be set.</param>
    /// <param name="lockoutEnd">The <see cref="DateTimeOffset"/> after which the <paramref name="user"/>'s lockout should end.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/> of the operation.</returns>
    public virtual async Task<GenResult<TUser>> SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd) =>
        (await _userMgr.SetLockoutEndDateAsync(user, lockoutEnd)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Increments the access failed count for the user as an asynchronous operation.
    /// If the failed access account is greater than or equal to the configured maximum number of attempts,
    /// the user will be locked out for the configured lockout time span.
    /// </summary>
    /// <param name="user">The user whose failed access count to increment.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/> of the operation.</returns>
    public virtual async Task<GenResult<TUser>> AccessFailedAsync(TUser user) =>
        (await _userMgr.AccessFailedAsync(user)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Resets the access failed count for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose failed access count should be reset.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="GenResult<AppUser>"/> of the operation.</returns>
    public virtual async Task<GenResult<TUser>> ResetAccessFailedCountAsync(TUser user) =>
        (await _userMgr.ResetAccessFailedCountAsync(user)).ToGenResult(user);

    //-----------------------//

    /// <summary>
    /// Retrieves the current number of failed accesses for the given <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose access failed count should be retrieved for.</param>
    /// <returns>The <see cref="Task"/> that contains the result the asynchronous operation, the current failed access count
    /// for the user.</returns>
    public virtual async Task<int> GetAccessFailedCountAsync(TUser user) =>
        await _userMgr.GetAccessFailedCountAsync(user);

    //-----------------------//


    ///// <summary>
    ///// Generates a new base32 encoded 160-bit security secret (size of SHA1 hash).
    ///// </summary>
    ///// <returns>The new security secret.</returns>
    //public  string GenerateNewAuthenticatorKey()
    //    => _userMgr.GenerateNewAuthenticatorKey();

    ////-----------------------//

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
    /// Generates recovery codes for the user, this invalidates any previous recovery codes for the user.
    /// </summary>
    /// <param name="user">The user to generate recovery codes for.</param>
    /// <param name="number">The number of codes to generate.</param>
    /// <returns>The new recovery codes for the user.  Note: there may be less than number returned, as duplicates will be removed.</returns>
    public async Task<IEnumerable<string>?> GenerateNewTwoFactorRecoveryCodesAsync(TUser user, int number)
        => await _userMgr.GenerateNewTwoFactorRecoveryCodesAsync(user, number);

    //-----------------------//

    /// <summary>
    /// Returns whether a recovery code is valid for a user. Note: recovery codes are only valid
    /// once, and will be invalid after use.
    /// </summary>
    /// <param name="user">The user who owns the recovery code.</param>
    /// <param name="code">The recovery code to use.</param>
    /// <returns>True if the recovery code was found for the user.</returns>
    public async Task<GenResult<TUser>> RedeemTwoFactorRecoveryCodeAsync(TUser user, string code) =>
        (await _userMgr.RedeemTwoFactorRecoveryCodeAsync(user, code)).ToGenResult(user);

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

    /// <summary>
    /// Returns how many recovery code are still valid for a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>How many recovery code are still valid for a user.</returns>
    public virtual async Task<int> CountRecoveryCodesAsync(TUser user)
        => await _userMgr.CountRecoveryCodesAsync(user);

    //-----------------------//

    /// <summary>
    /// Creates bytes to use as a security token from the user's security stamp.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>The security token bytes.</returns>
    public virtual async Task<byte[]> CreateSecurityTokenAsync(TUser user) =>
        Encoding.Unicode.GetBytes(await GetSecurityStampAsync(user).ConfigureAwait(false));

    //-----------------------//

    /// <summary>
    /// Generates the token purpose used to change email.
    /// </summary>
    /// <param name="newEmail">The new email address.</param>
    /// <returns>The token purpose.</returns>
    public static string GetChangeEmailTokenPurpose(string newEmail) => "ChangeEmail:" + newEmail;

    //-----------------------//

}//Cls
