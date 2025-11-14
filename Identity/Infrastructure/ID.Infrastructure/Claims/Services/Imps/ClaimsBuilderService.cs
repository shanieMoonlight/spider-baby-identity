using CollectionHelpers;
using ID.Application.AppAbs.ExtraClaims;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Infrastructure.Claims.Extensions;
using ID.Infrastructure.Claims.Services.Abs;
using System.Security.Claims;

namespace ID.Infrastructure.Claims.Services.Imps;

public class ClaimsBuilderService(
    IIdUserMgmtService<AppUser> userMgr,
    IExtraClaimsGenerator extraClaimsGenerator)
    : IClaimsBuilderService
{

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
