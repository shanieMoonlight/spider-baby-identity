using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Claims.Extensions;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ID.Infrastructure.Auth.JWT.LocalServices.Imps;

internal class JwtClaimsService(IOptions<JwtOptions> _jwtOptionsProvider)
    : IJwtClaimsService
{

    private readonly JwtOptions _jwtOptions = _jwtOptionsProvider.Value;


    //-----------------------//


    /// <summary>
    /// Add the registered claims for JWT (RFC7519) If not already there. 
    /// </summary>
    /// <param name="claims">Current list of claims</param>
    /// <param name="userId">User identifier</param>
    /// <returns>Claims List with Registers claims added</returns>
    public List<Claim> AddRegisteredClaims<TUser>(IEnumerable<Claim> initialClaims, TUser user) where TUser : AppUser
    {

        List<Claim> claims = [.. initialClaims];

        if (!claims.HasClaim(JwtRegisteredClaimNames.Sub)) //May have already been added by the default claims builder
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));

        if (!claims.HasClaim(JwtRegisteredClaimNames.Iss))
            claims.Add(new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.TokenIssuer));


        if (!claims.HasClaim(JwtRegisteredClaimNames.Jti))
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        if (!claims.HasClaim(JwtRegisteredClaimNames.Iat))
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, NowInUnixTimeSecondsString(), ClaimValueTypes.Integer64));


        return claims;
    }


    //-----------------------//


    internal static string NowInUnixTimeSecondsString() =>
        new DateTimeOffset(DateTime.UtcNow)
           .ToUnixTimeSeconds()
           .ToString(CultureInfo.InvariantCulture);


}//Cls