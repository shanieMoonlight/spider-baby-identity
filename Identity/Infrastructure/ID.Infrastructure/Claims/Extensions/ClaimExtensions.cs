using System.Security.Claims;


namespace ID.Infrastructure.Claims.Extensions;

internal static class ClaimExtensions
{

    internal static bool HasClaim(this IEnumerable<Claim> claims, string claimType) =>
        claims.Any(c => c.Type == claimType);


}//Cls