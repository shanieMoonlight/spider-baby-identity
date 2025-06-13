using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.GlobalSettings.Setup.Options;
using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Claims.Services.Abs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ID.Infrastructure.Auth.JWT.LocalServices.Imps;

/// <summary>
/// Focused service responsible for creating JWT tokens only.
/// This service handles the low-level JWT creation without managing refresh tokens or packaging concerns.
/// </summary>
/// <param name="_claimsBuilder">Service for building user claims for JWT tokens</param>
/// <param name="_jwtClaims">Service for adding registered JWT claims</param>
/// <param name="_keyHelper">Helper for building cryptographic keys for JWT signing</param>
/// <param name="_jwtOptions">JWT-specific configuration options</param>
/// <param name="_globalOptionsProvider">Global application configuration options</param>
public class JwtBuilder(
    IClaimsBuilderService _claimsBuilder,
    IJwtClaimsService _jwtClaims,
    IKeyHelper _keyHelper,
    IOptions<JwtOptions> _jwtOptions,
    IOptions<IdGlobalOptions> _globalOptionsProvider) : IJwtBuilder
{
    private readonly JwtOptions _jwt = _jwtOptions.Value;
    private readonly TimeSpan _tokenExpiration = TimeSpan.FromMinutes(_jwtOptions.Value.TokenExpirationMinutes);
    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;

    //-----------------------------//

    /// <summary>
    /// Creates a JWT token for two-factor authentication required scenarios.
    /// This token contains limited claims and indicates that 2FA verification is needed.
    /// </summary>
    /// <param name="user">User for whom the token is being created</param>
    /// <param name="team">Team context for the user</param>
    /// <param name="currentDeviceId">Optional device identifier for the current session</param>
    /// <returns>Encoded JWT token string ready for transmission</returns>
    public async Task<string> CreateJwtWithTwoFactorRequiredAsync(
        AppUser user,
        Team team,
        string? currentDeviceId = null)
    {
        var mfaRequiredClaims = await _claimsBuilder.BuildClaimsWithTwoFactorRequiredAsync(user, team, currentDeviceId);
        var claims = _jwtClaims.AddRegisteredClaims(mfaRequiredClaims, user);
        return GenerateAndSerializeToken(claims);
    }

    //-----------------------------//

    /// <summary>
    /// Creates a full JWT token with complete user claims and permissions.
    /// Use this when the user has verified 2FA or is not using 2FA.
    /// </summary>
    /// <param name="user">User for whom the token is being created</param>
    /// <param name="team">Team context for the user</param>
    /// <param name="twoFactorVerified">Whether the user has completed 2FA verification</param>
    /// <param name="currentDeviceId">Optional device identifier for the current session</param>
    /// <returns>Encoded JWT token string ready for transmission</returns>
    public async Task<string> CreateJwtAsync(
        AppUser user,
        Team team,
        bool twoFactorVerified,
        string? currentDeviceId = null)
    {
        var regularClaims = await _claimsBuilder.BuildClaimsAsync(user, team, twoFactorVerified, currentDeviceId);
        List<Claim> claims = _jwtClaims.AddRegisteredClaims(regularClaims, user);
        return GenerateAndSerializeToken(claims);
    }

    //-----------------------------//

    /// <summary>
    /// Internal method for generating and serializing JWT tokens with the provided claims.
    /// Handles token creation, signing, and serialization to string format.
    /// </summary>
    /// <param name="claims">Claims to include in the JWT token</param>
    /// <returns>Serialized JWT token as a string</returns>
    private string GenerateAndSerializeToken(IEnumerable<Claim> claims)
    {
        DateTime now = DateTime.UtcNow;

        JwtSecurityToken token = new(
            issuer: _jwt.TokenIssuer,
            claims: claims,
            notBefore: now,
            expires: now.Add(_tokenExpiration),
            signingCredentials: CreateSigningCredentials(),
            audience: _globalOptions.ApplicationName ?? "public"
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    //-----------------------------//

    /// <summary>
    /// Creates the appropriate signing credentials based on configuration.
    /// Supports both symmetric and asymmetric cryptography.
    /// </summary>
    /// <returns>Signing credentials for JWT token signing</returns>
    private SigningCredentials CreateSigningCredentials() =>
        _jwt.UseAsymmetricCrypto
            ? CreateAsymmetricSigningCredentials()
            : CreateSymmetricSigningCredentials();

    //-----------------------------//

    /// <summary>
    /// Creates symmetric signing credentials using a shared secret key.
    /// </summary>
    /// <returns>Symmetric signing credentials</returns>
    private SigningCredentials CreateSymmetricSigningCredentials() =>
        new(_jwt.SecurityKey, _jwt.SecurityAlgorithm);

    //-----------------------------//

    /// <summary>
    /// Creates asymmetric signing credentials using RSA private key.
    /// </summary>
    /// <returns>Asymmetric signing credentials</returns>
    private SigningCredentials CreateAsymmetricSigningCredentials()
    {
        SecurityKey key = _keyHelper.BuildRsaSigningKey(_jwt.AsymmetricTokenPrivateKey_Xml);
        return new SigningCredentials(key, _jwt.AsymmetricAlgorithm);
    }

}//Cls
