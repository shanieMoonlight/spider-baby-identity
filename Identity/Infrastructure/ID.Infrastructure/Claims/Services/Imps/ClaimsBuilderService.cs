using CollectionHelpers;
using ID.Application.AppAbs.ExtraClaims;
using ID.Application.AppAbs.FromApp;
using ID.Domain.Claims;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Infrastructure.Claims.Extensions;
using ID.Infrastructure.Claims.Services.Abs;
using System.Security.Claims;

namespace ID.Infrastructure.Claims.Services.Imps;

public class ClaimsBuilderService(
    IIdUserMgmtService<AppUser> userMgr,
    IExtraClaimsGenerator extraClaimsGenerator,
    IIsFromMobileApp _fromAppService)
    : IClaimsBuilderService
{

    public async Task<List<Claim>> BuildClaimsAsync(AppUser user, Team team, bool twoFactorVerified, string? currentDeviceId)
    {

        List<Claim> claims = await BuildClaimsAsync(user, team, currentDeviceId);

        if (twoFactorVerified)
            claims.Add(TwoFactorClaims.TwoFactorVerified);
        else if (user.TwoFactorEnabled && !_fromAppService.IsFromApp)
            claims.Add(TwoFactorClaims.TwoFactorRequired);

        return [.. claims];
    }

    //--------------------------------//

    public async Task<List<Claim>> BuildClaimsAsync(AppUser user, Team team, string? currentDeviceId)
    {
        IList<string> userRoles = await userMgr.GetRolesAsync(user);

        IList<Claim> userClaims = (await userMgr.GetClaimsAsync(user))
            .AddRolesToClaims(userRoles)
            .AddTeamDataToClaims(user, team, currentDeviceId)
            .AddUserInfoClaims(user);

        userClaims.AddRange(extraClaimsGenerator.Generate(user, team) ?? []);

        return [.. userClaims];
    }


}//Cls
