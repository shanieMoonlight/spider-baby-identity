using ID.Domain.Claims.AuthMethods;
using System.Security.Claims;

namespace ID.Infrastructure.Claims.Extensions;

internal static class ClaimAuthMethodExtensions
{

    internal static IList<Claim> AddAuthMethodsToClaims(this IList<Claim> claims, IEnumerable<AuthMethodRef> authMethods)
    {

        foreach (AuthMethodRef method in authMethods)
        {
            claims.Add(AuthenticationClaims.Amr(method));
        }

        return claims;
    }


    internal static IList<Claim> AddAuthTimeToClaims(this IList<Claim> claims, DateTime? authTime = null)
    {
        claims.Add(AuthenticationClaims.AuthTime(authTime ?? DateTime.UtcNow));
        return claims;
    }


}//Cls
