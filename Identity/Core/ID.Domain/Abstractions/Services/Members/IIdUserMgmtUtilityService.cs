using ID.Domain.Entities.AppUsers;
using Microsoft.AspNetCore.Identity;

namespace ID.Domain.Abstractions.Services.Members;
/// <summary>
/// Provides the APIs for managing user in a persistence store.
/// </summary>
/// <typeparam name="TUser">The type encapsulating a user.</typeparam>
public interface IIdUserMgmtUtilityService<TUser> where TUser : AppUser
{
    /// <summary>
    /// The <see cref="IdentityErrorDescriber"/> used to generate error messages.
    /// </summary>
    IdentityErrorDescriber ErrorDescriber { get; set; }

    /// <summary>
    /// The <see cref="ILookupNormalizer"/> used to normalize things like user and role names.
    /// </summary>
    ILookupNormalizer KeyNormalizer { get; set; }

    /// <summary>
    /// The <see cref="IdentityOptions"/> used to configure Identity.
    /// </summary>
    IdentityOptions Options { get; set; }

    /// <summary>
    /// The <see cref="IPasswordHasher{TUser}"/> used to hash passwords.
    /// </summary>
    IPasswordHasher<TUser> PasswordHasher { get; set; }

    /// <summary>
    /// The <see cref="IPasswordValidator{TUser}"/> used to validate passwords.
    /// </summary>
    IList<IPasswordValidator<TUser>> PasswordValidators { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports returning
    /// <see cref="IQueryable"/> collections of information.
    /// </summary>
    bool SupportsQueryableUsers { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports authentication tokens.
    /// </summary>
    bool SupportsUserAuthenticationTokens { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports a user authenticator.
    /// </summary>
    bool SupportsUserAuthenticatorKey { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user claims.
    /// </summary>
    bool SupportsUserClaim { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user emails.
    /// </summary>
    bool SupportsUserEmail { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user lock-outs.
    /// </summary>
    bool SupportsUserLockout { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports external logins.
    /// </summary>
    bool SupportsUserLogin { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user passwords.
    /// </summary>
    bool SupportsUserPassword { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user telephone numbers.
    /// </summary>
    bool SupportsUserPhoneNumber { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports user roles.
    /// </summary>
    bool SupportsUserRole { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports security stamps.
    /// </summary>
    bool SupportsUserSecurityStamp { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports two factor authentication.
    /// </summary>
    bool SupportsUserTwoFactor { get; }

    /// <summary>
    /// Gets a flag indicating whether the backing user store supports recovery codes.
    /// </summary>
    bool SupportsUserTwoFactorRecoveryCodes { get; }

    /// <summary>
    /// The <see cref="IUserValidator{TUser}"/> used to validate users.
    /// </summary>
    IList<IUserValidator<TUser>> UserValidators { get; }

    /// <summary>
    /// Releases all resources used by the user manager.
    /// </summary>
    void Dispose();

    /// <summary>
    /// Generates a new base32 encoded 160-bit security secret (size of SHA1 hash).
    /// </summary>
    /// <returns>The new security secret.</returns>
    string GenerateNewAuthenticatorKey();

    /// <summary>
    /// Normalize email for consistent comparisons.
    /// </summary>
    /// <param name="email">The email to normalize.</param>
    /// <returns>A normalized value representing the specified <paramref name="email"/>.</returns>
    string? NormalizeEmail(string? email);

    /// <summary>
    /// Normalize user or role name for consistent comparisons.
    /// </summary>
    /// <param name="name">The name to normalize.</param>
    /// <returns>A normalized value representing the specified <paramref name="name"/>.</returns>
    string? NormalizeName(string? name);
}
