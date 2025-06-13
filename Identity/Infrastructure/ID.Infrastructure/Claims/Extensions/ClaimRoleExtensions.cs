using CollectionHelpers;
using System.Security.Claims;

namespace ID.Infrastructure.Claims.Extensions;

internal static class ClaimRoleExtensions
{

    internal static IList<Claim> AddRolesToClaims(this IList<Claim> claims, IList<string> userRoles)
    {
        if (!userRoles.AnyValues())
            return claims;

        foreach (string role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }


}//Cls
