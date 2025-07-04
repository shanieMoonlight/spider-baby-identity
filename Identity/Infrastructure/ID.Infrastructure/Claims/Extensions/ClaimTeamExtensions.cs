using CollectionHelpers;
using ID.Application.JWT.Subscriptions;
using ID.Domain.Claims;
using ID.Domain.Claims.Utils;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using System.Security.Claims;

namespace ID.Infrastructure.Claims.Extensions;

internal static class ClaimTeamExtensions
{

    internal static IList<Claim> AddTeamDataToClaims(this IList<Claim> claims, AppUser user, Team team, string? currentDeviceId)
    {

        if (!claims.HasClaim(MyIdClaimTypes.TEAM_ID))
            claims.Add(IdTeamClaims.TEAM_ID(user.TeamId));

        if (!claims.HasClaim(MyIdClaimTypes.TEAM_POSITION))
            claims.Add(IdTeamClaims.TEAM_POSITION(user.TeamPosition));

        if (team.LeaderId == user.Id)
            claims.Add(IdTeamClaims.LEADER);

        switch (team.TeamType)
        {
            case TeamType.customer:
                claims.AddRange([IdTeamClaims.CUSTOMER_TEAM]);
                break;
            case TeamType.maintenance:
                claims.AddRange([IdTeamClaims.MAINTENANCE_TEAM]);
                break;
            case TeamType.super:
                claims.AddRange([IdTeamClaims.SUPER_TEAM]);
                break;
        }

        // Add subscription data to claims
        claims.AddRange(team.Subscriptions.ToClaims(currentDeviceId));

        return claims;
    }

    //-----------------------//

}//Cls
