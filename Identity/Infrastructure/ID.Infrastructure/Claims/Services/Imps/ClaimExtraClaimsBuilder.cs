using ID.Domain.Claims.Utils;
using ID.GlobalSettings.Setup.Options;
using ID.Infrastructure.Claims.Extensions;
using ID.Infrastructure.Claims.Services.Abs;
using Microsoft.Extensions.Options;
using StringHelpers;
using System.Security.Claims;


namespace ID.Infrastructure.Claims.Services.Imps;

internal class ClaimExtraClaimsBuilder(IOptions<IdGlobalOptions> _globalOptionsProvider)
    : IClaimExtraClaimsBuilder
{

    private readonly IdGlobalOptions _globalOptions = _globalOptionsProvider.Value;

    //------------------------------//

    public IList<Claim> AddApplicationInfoClaims(IList<Claim> claims)
    {
        if (!claims.HasClaim(MyIdClaimTypes.APPLICATION) && _globalOptions.ApplicationName.IsNullOrWhiteSpace())
            claims.Add(new Claim(MyIdClaimTypes.APPLICATION, _globalOptions.ApplicationName));

        return claims;
    }




}//Cls