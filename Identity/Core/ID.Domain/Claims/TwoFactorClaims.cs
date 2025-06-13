using ID.Domain.Claims.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ID.Domain.Claims;

public class TwoFactorClaims
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
    public readonly static Claim AmrMultiFactor = ClaimHelpers.GenerateClaim(JwtRegisteredClaimNames.Amr, MyClaimValues.MULTI_FACTOR, ClaimValueTypes.String);

    /// <summary>
    /// Authentication Methods References - Can authenticate with password only
    /// </summary>
    public readonly static Claim AmrPassword = ClaimHelpers.GenerateClaim(JwtRegisteredClaimNames.Amr, MyClaimValues.PASSWORD, ClaimValueTypes.String);

}
