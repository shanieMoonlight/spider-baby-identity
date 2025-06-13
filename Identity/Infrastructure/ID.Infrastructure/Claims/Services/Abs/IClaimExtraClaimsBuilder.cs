using System.Security.Claims;

namespace ID.Infrastructure.Claims.Services.Abs;
internal interface IClaimExtraClaimsBuilder
{
    IList<Claim> AddApplicationInfoClaims(IList<Claim> claims);
}