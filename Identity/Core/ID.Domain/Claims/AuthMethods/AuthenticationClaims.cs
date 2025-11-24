using ID.Domain.Claims.Utils;
using ID.Domain.Utility.Dates;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ID.Domain.Claims.AuthMethods;

public class AuthenticationClaims
{
    /// <summary>
    /// User Has completed multifactor auth.
    /// </summary>
    public readonly static Claim TwoFactorVerified = ClaimHelpers.GenerateClaim(MyIdClaimTypes.TWO_FACTOR_VERIFIED, true, ClaimValueTypes.Boolean);

    /// <summary>
    /// User Has completed multifactor auth.
    /// </summary>
    public readonly static Claim TwoFactor_NOT_Verified = ClaimHelpers.GenerateClaim(MyIdClaimTypes.TWO_FACTOR_VERIFIED, false, ClaimValueTypes.Boolean);
    /// <summary>
    /// User Has completed login with pwd bu still needs multifactor auth.
    /// </summary>
    public readonly static Claim TwoFactorRequired = ClaimHelpers.GenerateClaim(MyIdClaimTypes.TWO_FACTOR_REQUIRED, true, ClaimValueTypes.Boolean);

    /// <summary>
    /// Authentication Methods References - User must authenticaste with MFA
    /// </summary>
    public readonly static Claim AmrMultiFactor = ClaimHelpers.GenerateClaim(JwtRegisteredClaimNames.Amr, AuthMethodClaimValues.MULTI_FACTOR, ClaimValueTypes.String);

    /// <summary>
    /// Authentication Methods References - Can authenticate with password only
    /// </summary>
    public readonly static Claim AmrPassword = ClaimHelpers.GenerateClaim(JwtRegisteredClaimNames.Amr, AuthMethodClaimValues.PASSWORD, ClaimValueTypes.String);

    /// <summary>
    /// Generate a claim for when the user authenticated (UTC)
    /// </summary>
    public static Claim AuthTime(DateTime authTimeUtc) =>       
        ClaimHelpers.GenerateClaim(JwtRegisteredClaimNames.AuthTime, $"{authTimeUtc.ConvertToUnixTimestamp()}", ClaimValueTypes.Integer);

    /// <summary>
    /// Generate a claim for when the user authenticated (set to Now inUTC)
    /// </summary>
    public static Claim AuthTime() =>
        ClaimHelpers.GenerateClaim(JwtRegisteredClaimNames.AuthTime, $"{DateTime.UtcNow.ConvertToUnixTimestamp()}", ClaimValueTypes.Integer);


    /// <summary>
    /// Authentication Methods References - Can authenticate with password only
    /// </summary>
    public static Claim Amr(AuthMethodRef amrValue) => ClaimHelpers.GenerateClaim(JwtRegisteredClaimNames.Amr, $"{amrValue}", ClaimValueTypes.String);

}
