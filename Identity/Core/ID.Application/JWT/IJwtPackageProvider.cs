using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;

namespace ID.Application.JWT;

/// <summary>
/// Interface for high-level JWT package creation service.
/// Provides a clean API for authentication handlers to create complete authentication packages
/// without needing to understand the internal complexities of refresh token generation rules.
/// </summary>
public interface IJwtPackageProvider
{
    /// <summary>
    /// Creates a complete JWT package for two-factor authentication required scenarios.
    /// This package contains a limited JWT token and no refresh token (refresh token will be provided after 2FA verification).
    /// </summary>
    /// <param name="user">User requiring two-factor authentication</param>
    /// <param name="provider">Two-factor authentication provider being used</param>
    /// <param name="extraInfo">Optional additional information for the 2FA process</param>
    /// <param name="currentDeviceId">Optional device identifier for the current session</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>JWT package configured for two-factor authentication flow</returns>
    Task<JwtPackage> CreateJwtPackageWithTwoFactorRequiredAsync(
        AppUser user,
        TwoFactorProvider provider,
        string? extraInfo = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a complete JWT package for successful authentication scenarios.
    /// Automatically handles refresh token generation based on user 2FA status and global configuration.
    /// </summary>
    /// <param name="user">Authenticated user</param>
    /// <param name="team">Team context for the user</param>
    /// <param name="currentDeviceId">Optional device identifier for the current session</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>Complete JWT package with access token and optional refresh token</returns>
    Task<JwtPackage> CreateJwtPackageAsync(
        AppUser user,
        Team team,
        string? currentDeviceId = null,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Creates a new JWT package using an existing refresh token. Intelligently updates the refresh token
    /// based on the configured <see cref="RefreshTokenUpdatePolicy"/> to balance security and performance.
    /// Assumes two-factor authentication is already verified since the user possesses a valid refresh token.
    /// </summary>
    /// <param name="existingToken">The valid refresh token being refreshed</param>
    /// <param name="user">The authenticated user (explicit validation required)</param>
    /// <param name="team">The user's team context (explicit validation required)</param>
    /// <param name="currentDeviceId">Optional device identifier for audit and security</param>
    /// <returns>A JWT package with new access token and potentially updated refresh token</returns>
    Task<JwtPackage> RefreshJwtPackageAsync(
        IdRefreshToken existingToken, 
        AppUser user, 
        Team team, 
        string? currentDeviceId = null);
}
