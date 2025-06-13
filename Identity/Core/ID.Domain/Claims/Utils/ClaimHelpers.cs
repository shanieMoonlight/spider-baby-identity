using ID.GlobalSettings.Setup.Defaults;
using System.Security.Claims;

namespace ID.Domain.Claims.Utils;

public class ClaimHelpers
{
    public static Claim GenerateRoleClaim(string roleValue) =>
        GenerateClaim(MyIdClaimTypes.ROLE, roleValue);

    //----------------------------------------------------//

    public static Claim GenerateClaim(string claimType, string claimValue, string valueType = ClaimValueTypes.String) =>
        new(claimType, claimValue, valueType, IdGlobalDefaultValues.TOKEN_ISSUER, IdGlobalDefaultValues.TOKEN_ISSUER);

    //----------------------------------------------------//

    public static Claim GenerateClaim(string claimType, bool claimValue, string valueType = ClaimValueTypes.String) =>
        new(claimType, $"{claimValue}".ToLower(), valueType, IdGlobalDefaultValues.TOKEN_ISSUER, IdGlobalDefaultValues.TOKEN_ISSUER);

}//Cls