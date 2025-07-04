using ID.Domain.Claims.Utils;
using ID.Domain.Entities.AppUsers;
using StringHelpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ID.Infrastructure.Claims.Extensions;

internal static class ClaimUserInfoExtensions
{

    private const string _unknown_value = "unknown";

    internal static IList<Claim> AddUserInfoClaims(this IList<Claim> claims, AppUser user)
    {
        if (user is null)
            return claims;

        if (!claims.HasClaim(MyIdClaimTypes.NAME) && !user.UserName.IsNullOrWhiteSpace())
            claims.Add(new Claim(MyIdClaimTypes.NAME, user.UserName ?? _unknown_value));

        if (!claims.HasClaim(JwtRegisteredClaimNames.Sub)) //Use the JWT version even for cookies (it's smaller)
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));

        //if (!claims.HasClaim(MyIdClaimTypes.LAST_NAME) && !user.LastName.IsNullOrWhiteSpace())
        //    claims.Add(new Claim(MyIdClaimTypes.LAST_NAME, user.LastName ?? _unknown_value));

        if (!claims.HasClaim(JwtRegisteredClaimNames.Email) && !user.Email.IsNullOrWhiteSpace())
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email ?? _unknown_value));

        return claims;

    }

}//Cls