using MyResults;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;


namespace ID.Application.AppImps.Permissions;
internal class CanViewTeamMemberPermissions<TUser>(IIdentityTeamManager<TUser> _teamMgr) : ICanViewTeamMemberPermissions<TUser> where TUser : AppUser
{

    //------------------------------------//

    /// <summary>
    /// Finds the user with Id <paramref name="memberId"/> and checks if the User/Principal making the <paramref name="request"/>
    /// can view/access the with Id <paramref name="memberId"/> 
    /// </summary>
    /// <param name="memberId">User Identifier</param>
    /// <param name="request">The Mediatr request with Principal information</param>
    /// <returns>Result with the user, if it exists and the Principal has permission View it</returns>
    public async Task<GenResult<TUser>> CanViewTeamMemberAsync(Guid memberTeamId, Guid memberId, IIdUserAwareRequest<TUser> request)
    {
        var principalUser = request.PrincipalUser;
        var memberTeam = await _teamMgr.GetByIdWithMemberAsync(memberTeamId, memberId);
        var member = memberTeam?.Members?.FirstOrDefault(m => m.Id == memberId);

        //User must exist
        if (member is null)
            return GenResult<TUser>.NotFoundResult(IDMsgs.Error.NotFound<TUser>(memberId));

        //If User is on the same team as the principalUser, the principalUser must be in the same position or higher.
        if (member.TeamId == principalUser.TeamId)
        {
            if (principalUser.TeamPosition > member.TeamPosition)
                return GenResult<TUser>.ForbiddenResult(IDMsgs.Error.Authorization.UNAUTHORIZED_FOR_POSITION(member.TeamPosition));
        }

        //If not on the same team. The principalUser must be in a Higher Team.
        //TeamType.Customer < TeamType.Maintenance < TeamType.Super
        if (member.TeamId != principalUser.TeamId)
        {
            if (request.TeamType <= memberTeam!.TeamType)
                return GenResult<TUser>.ForbiddenResult(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(memberTeam.TeamType));
        }


        return GenResult<TUser>.Success(member as TUser);
        // TODO: Remove "as TUser" when we a Generic Db Context
    }

    //------------------------------------//

}//Cls
