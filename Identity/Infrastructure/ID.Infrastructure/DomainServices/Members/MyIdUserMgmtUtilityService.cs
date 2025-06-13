using ID.Domain.Abstractions.Services.Members;
using ID.Domain.Entities.AppUsers;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace ID.Infrastructure.DomainServices.Members;
internal class MyIdUserMgmtUtilityService<TUser>(UserManager<TUser> _userMgr) : IIdUserMgmtUtilityService<TUser> where TUser : AppUser
{
    /// <summary>
    /// The <see cref="IPasswordHasher{TUser}"/> used to hash passwords.
    /// </summary>
    public IPasswordHasher<TUser> PasswordHasher { get => _userMgr.PasswordHasher; set { _userMgr.PasswordHasher = value; } }

    //-----------------------//

    /// <summary>
    /// The <see cref="IUserValidator{TUser}"/> used to validate users.
    /// </summary>
    public IList<IUserValidator<TUser>> UserValidators { get => _userMgr.UserValidators; }

    //-----------------------//

    /// <summary>
    /// The <see cref="IPasswordValidator{TUser}"/> used to validate passwords.
    /// </summary>
    public IList<IPasswordValidator<TUser>> PasswordValidators { get => _userMgr.PasswordValidators; }

    //-----------------------//

    /// <summary>
    /// The <see cref="ILookupNormalizer"/> used to normalize things like user and role names.
    /// </summary>
    public ILookupNormalizer KeyNormalizer { get => _userMgr.KeyNormalizer; set { _userMgr.KeyNormalizer = value; } }

    //-----------------------//

    /// <summary>
    /// The <see cref="IdentityErrorDescriber"/> used to generate error messages.
    /// </summary>
    public IdentityErrorDescriber ErrorDescriber { get => _userMgr.ErrorDescriber; set { _userMgr.ErrorDescriber = value; } }

    //-----------------------//

    /// <summary>
    /// The <see cref="IdentityOptions"/> used to configure Identity.
    /// </summary>
    public IdentityOptions Options { get => _userMgr.Options; set { _userMgr.Options = value; } }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports authentication tokens.
    /// </summary>
    /// <value>
    /// true if the backing user store supports authentication tokens, otherwise false.
    /// </value>
    public virtual bool SupportsUserAuthenticationTokens { get => _userMgr.SupportsUserAuthenticationTokens; }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports a user authenticator.
    /// </summary>
    /// <value>
    /// true if the backing user store supports a user authenticator, otherwise false.
    /// </value>
    public virtual bool SupportsUserAuthenticatorKey { get => _userMgr.SupportsUserAuthenticatorKey; }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports recovery codes.
    /// </summary>
    /// <value>
    /// true if the backing user store supports a user authenticator, otherwise false.
    /// </value>
    public virtual bool SupportsUserTwoFactorRecoveryCodes { get => _userMgr.SupportsUserTwoFactorRecoveryCodes; }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports two factor authentication.
    /// </summary>
    /// <value>
    /// true if the backing user store supports user two factor authentication, otherwise false.
    /// </value>
    public virtual bool SupportsUserTwoFactor { get => _userMgr.SupportsUserTwoFactor; }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user passwords.
    /// </summary>
    /// <value>
    /// true if the backing user store supports user passwords, otherwise false.
    /// </value>
    public virtual bool SupportsUserPassword { get => _userMgr.SupportsUserPassword; }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports security stamps.
    /// </summary>
    /// <value>
    /// true if the backing user store supports user security stamps, otherwise false.
    /// </value>
    public virtual bool SupportsUserSecurityStamp { get => _userMgr.SupportsUserSecurityStamp; }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user roles.
    /// </summary>
    /// <value>
    /// true if the backing user store supports user roles, otherwise false.
    /// </value>
    public virtual bool SupportsUserRole { get => _userMgr.SupportsUserRole; }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports external logins.
    /// </summary>
    /// <value>
    /// true if the backing user store supports external logins, otherwise false.
    /// </value>
    public virtual bool SupportsUserLogin { get => _userMgr.SupportsUserLogin; }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user emails.
    /// </summary>
    /// <value>
    /// true if the backing user store supports user emails, otherwise false.
    /// </value>
    public virtual bool SupportsUserEmail { get => _userMgr.SupportsUserEmail; }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user telephone numbers.
    /// </summary>
    /// <value>
    /// true if the backing user store supports user telephone numbers, otherwise false.
    /// </value>
    public virtual bool SupportsUserPhoneNumber { get => _userMgr.SupportsUserPhoneNumber; }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user claims.
    /// </summary>
    /// <value>
    /// true if the backing user store supports user claims, otherwise false.
    /// </value>
    public virtual bool SupportsUserClaim { get => _userMgr.SupportsUserClaim; }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user lock-outs.
    /// </summary>
    /// <value>
    /// true if the backing user store supports user lock-outs, otherwise false.
    /// </value>
    public virtual bool SupportsUserLockout { get => _userMgr.SupportsUserLockout; }

    //-----------------------//

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports returning
    /// <see cref="IQueryable"/> collections of information.
    /// </summary>
    /// <value>
    /// true if the backing user store supports returning <see cref="IQueryable"/> collections of
    /// information, otherwise false.
    /// </value>
    public virtual bool SupportsQueryableUsers { get => _userMgr.SupportsQueryableUsers; }

    //-----------------------//

    /// <summary>
    /// Releases all resources used by the user manager.
    /// </summary>
    public void Dispose() => _userMgr.Dispose();

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
    /// Generates a new base32 encoded 160-bit security secret (size of SHA1 hash).
    /// </summary>
    /// <returns>The new security secret.</returns>
    public string GenerateNewAuthenticatorKey()
        => _userMgr.GenerateNewAuthenticatorKey();

    //-----------------------//
}//Cls
