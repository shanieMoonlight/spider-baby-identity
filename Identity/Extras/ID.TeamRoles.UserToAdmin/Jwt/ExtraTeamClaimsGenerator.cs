using ID.Application.AppAbs.ExtraClaims;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.TeamRoles.UserToAdmin.Claims;
using System.Security.Claims;

namespace ID.TeamRoles.UserToAdmin.Jwt;
public class TeamRole_User_to_Mgr_ClaimsGenerator : IExtraClaimsGenerator
{

    public IEnumerable<Claim> Generate(AppUser user, Team team)
    {
        List<Claim> claims = [];

        if (user.TeamPosition <= team.MinPosition)
        {
            claims.Add(IdTeamRoleClaims.USER);
        }
        else if (user.TeamPosition >= team.MaxPosition)
        {
            claims.Add(IdTeamRoleClaims.ADMIN);
        }
        else
        {
            switch (user.TeamPosition)
            {
                case (int)Data.TeamPositionRoles.User:
                    claims.Add(IdTeamRoleClaims.USER);
                    break;
                case (int)Data.TeamPositionRoles.Manager:
                    claims.Add(IdTeamRoleClaims.MANAGER);
                    break;
                case (int)Data.TeamPositionRoles.Admin:
                    claims.Add(IdTeamRoleClaims.ADMIN);
                    break;
            }
        }

        return claims;
    }

}//Cls