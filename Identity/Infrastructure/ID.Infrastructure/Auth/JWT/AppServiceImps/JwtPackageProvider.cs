using ID.Application.JWT;
using ID.Application.MFA;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.Domain.Utility.Dates;
using ID.GlobalSettings.Setup.Options;
using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.Setup;
using Microsoft.Extensions.Options;

namespace ID.Infrastructure.Auth.JWT.AppServiceImps;

/// <summary>
/// High-level service responsible for creating complete JWT authentication packages.
/// This service orchestrates JWT creation and refresh token generation based on business rules,
/// providing a clean API for authentication handlers while maintaining separation of concerns.
/// </summary>
/// <param name="_jwtBuilder">Service for creating JWT tokens</param>
/// <param name="_refreshTokenService">Service for managing refresh token lifecycle</param>
/// <param name="_globalOptionsProvider">Global application configuration options</param>
public class JwtPackageProvider(
    IJwtBuilder _jwtBuilder,
    IJwtRefreshTokenService<AppUser> _refreshTokenService,
    ITwofactorUserIdCacheService _twofactorUserIdCache,
    IOptions<JwtOptions> _jwtOptions,
    IOptions<IdGlobalOptions> _globalOptionsProvider) : IJwtPackageProvider
{
    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;
    private readonly TimeSpan _tokenExpiration = TimeSpan.FromMinutes(_jwtOptions.Value.TokenExpirationMinutes);

    //-----------------------------//

    /// <summary>
    /// Creates a complete JWT package for two-factor authentication required scenarios.
    /// This package contains a limited JWT token and no refresh token (refresh token will be provided after 2FA verification).
    /// </summary>
    /// <param name="user">User requiring two-factor authentication</param>
    /// <param name="team">Team context for the user</param>
    /// <param name="provider">Two-factor authentication provider being used</param>
    /// <param name="extraInfo">Optional additional information for the 2FA process</param>
    /// <param name="currentDeviceId">Optional device identifier for the current session</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>JWT package configured for two-factor authentication flow</returns>
    public Task<JwtPackage> CreateJwtPackageWithTwoFactorRequiredAsync(
        AppUser user,
        TwoFactorProvider provider,
        string? extraInfo = null,
        CancellationToken cancellationToken = default)
    {
        long expiration = GetTokenExpirationUnixTimestamp();
        string twoFactorToken = _twofactorUserIdCache.StoreUserId(user.Id); 

        var pkg =  JwtPackage.CreateWithTwoFactoRequired(
            twoFactorToken,
            expiration,
            provider,
            extraInfo);

        return Task.FromResult(pkg);
    }

    //-----------------------------//

    /// <summary>
    /// Creates a complete JWT package for successful authentication scenarios.
    /// Automatically handles refresh token generation based on user 2FA status and global configuration.
    /// </summary>
    /// <param name="user">Authenticated user</param>
    /// <param name="team">Team context for the user</param>
    /// <param name="twoFactorVerified">Whether the user has completed 2FA verification</param>
    /// <param name="currentDeviceId">Optional device identifier for the current session</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>Complete JWT package with access token and optional refresh token</returns>
    public async Task<JwtPackage> CreateJwtPackageAsync(
        AppUser user,
        Team team,
        bool twoFactorVerified,
        string? currentDeviceId = null,
        CancellationToken cancellationToken = default)
    {
        // Generate JWT token
        string encodedToken = await _jwtBuilder.CreateJwtAsync(user, team, twoFactorVerified, currentDeviceId);

        // Generate refresh token if eligible
        IdRefreshToken? refreshToken = await GenerateRefreshTokenIfEligibleAsync(user, twoFactorVerified, cancellationToken);

        long expiration = GetTokenExpirationUnixTimestamp();

        return JwtPackage.Create(
            encodedToken,
            expiration,
            user.TwoFactorProvider,
            refreshToken?.Payload);
    }

    //-----------------------------//

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
    public async Task<JwtPackage> RefreshJwtPackageAsync(
        IdRefreshToken existingToken,
        AppUser user,
        Team team,
        string? currentDeviceId = null)
    {

        string encodedToken = await _jwtBuilder.CreateJwtAsync(
            user: user,
            team: team,
            twoFactorVerified: true,//If user has a valid RefreshToken, then we can assume that the user is logged in. And if the user has two-factor authentication enabled the got here by Verifying that 2-factor auth.
            currentDeviceId: currentDeviceId);

        var refreshToken = await GetRefreshTokenWithSmartUpdateAsync(existingToken);

        long expiration = GetTokenExpirationUnixTimestamp();

        return JwtPackage.Create(
               encodedToken,
               expiration,
               user.TwoFactorProvider,
               refreshToken?.Payload);
    }

    //-----------------------------//

    /// <summary>
    /// Determines whether a refresh token should be generated for the user based on business rules.
    /// 
    /// Refresh token generation rules:
    /// - Must be globally enabled via configuration
    /// - If user has 2FA enabled, only generate after 2FA verification
    /// - If user doesn't have 2FA enabled, generate immediately
    /// </summary>
    /// <param name="user">User requesting authentication</param>
    /// <param name="twoFactorVerified">Whether 2FA has been completed</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>Generated refresh token if eligible, null otherwise</returns>
    private async Task<IdRefreshToken?> GenerateRefreshTokenIfEligibleAsync(
        AppUser user,
        bool twoFactorVerified,
        CancellationToken cancellationToken)
    {
        // Check if refresh tokens are globally disabled
        if (!_globalOptions.JwtRefreshTokensEnabled)
            return null;

        // Business rule: Only generate refresh token if:
        // 1. Two-factor authentication has been verified, OR
        // 2. User doesn't have two-factor authentication enabled
        if (user.TwoFactorEnabled && !twoFactorVerified)
            return null;


        return await _refreshTokenService.GenerateTokenAsync(user, cancellationToken);

    }

    //-----------------------------//

    /// <summary>
    /// Intelligently determines whether to update the refresh token based on the configured policy.
    /// This prevents unnecessary database updates while maintaining security through token rotation.
    /// </summary>
    /// <param name="existingToken">The current refresh token</param>
    /// <returns>Either the updated token or the existing token based on policy</returns>
    private async Task<IdRefreshToken> GetRefreshTokenWithSmartUpdateAsync(IdRefreshToken existingToken)
    {
        if (ShouldUpdateRefreshToken(existingToken))
            return await _refreshTokenService.UpdateTokenPayloadAsync(existingToken);

        // Reuse existing token (no database hit)
        return existingToken;
    }

    //- - - - - - - - - - - - - - -//

    /// <summary>
    /// Determines if the refresh token should be updated based on the configured refresh policy.
    /// </summary>
    /// <param name="token">The refresh token to evaluate</param>
    /// <returns>True if the token should be updated, false to reuse existing token</returns>
    private bool ShouldUpdateRefreshToken(IdRefreshToken token)
    {
        var policy = _jwtOptions.Value.RefreshTokenUpdatePolicy;

        return policy switch
        {
            RefreshTokenUpdatePolicy.Always => true,
            RefreshTokenUpdatePolicy.Never => false,
            RefreshTokenUpdatePolicy.QuarterLife => IsTokenOlderThan(token, 0.25),
            RefreshTokenUpdatePolicy.HalfLife => IsTokenOlderThan(token, 0.50),
            RefreshTokenUpdatePolicy.ThreeQuarterLife => IsTokenOlderThan(token, 0.75),
            _ => true // Default to Always for safety
        };
    }

    //- - - - - - - - - - - - - - -//

    /// <summary>
    /// Checks if a refresh token has passed a certain percentage of its lifetime.
    /// </summary>
    /// <param name="token">The refresh token to check</param>
    /// <param name="lifetimePercentage">The percentage of lifetime to check (0.25 = 25%, 0.50 = 50%, etc.)</param>
    /// <returns>True if the token is older than the specified percentage of its lifetime</returns>
    private static bool IsTokenOlderThan(IdRefreshToken token, double lifetimePercentage)
    {
        var tokenAge = DateTime.UtcNow - token.CreatedUtc;
        var tokenLifetime = token.ExpiresOnUtc - token.CreatedUtc;
        var threshold = tokenLifetime.TotalMinutes * lifetimePercentage;

        return tokenAge.TotalMinutes >= threshold;

    }

    //- - - - - - - - - - - - - - -//


    /// <summary>
    /// Gets the token expiration time from JWT options.
    /// This is a placeholder - in the real implementation, this would come from the JwtOptions.
    /// </summary>
    /// <returns>Token expiration timespan</returns>
    private long GetTokenExpirationUnixTimestamp() =>
        (long)DateTime.UtcNow.Add(_tokenExpiration).ConvertToUnixTimestamp();



}//Cls
