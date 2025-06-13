using MyResults;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;


namespace ID.Application.AppImps.Permissions;
internal class CanChangeLeaderPermissions<TUser>(IIdentityTeamManager<TUser> _teamMgr) : ICanChangeLeaderPermissions<TUser> where TUser : AppUser
{

    //------------------------------------//

    /// <summary>
    /// Finds the user with Id <paramref name="newLeaderId"/> and checks if the User/Principal making the <paramref name="request"/>
    /// can make the user with Id <paramref name="newLeaderId"/> the new Team Leader
    /// </summary>
    /// <param name="newLeaderId">User to Update's Identifier</param>
    /// <param name="request">The Mediator request with Principal information</param>
    /// <returns>Result with the user, if it exists and the Principal has permission to make change Leader</returns>
    public Task<GenResult<TUser>> CanChange_MyTeam_LeaderAsync(Guid newLeaderId, IIdUserAndTeamAwareRequest<TUser> request)
    {
        var team = request.PrincipalTeam;

        //Principal must be Leader
        var principalId = request.PrincipalUserId;
        if (team.LeaderId != principalId)
            return Task.FromResult(GenResult<TUser>.ForbiddenResult(IDMsgs.Error.Authorization.UNAUTHORIZED_FOR_POSITION("Leader")));

        //NewLeader must be in team
        var dbUser = team.Members.FirstOrDefault(m => m.Id == newLeaderId);
        if (dbUser is null)
            return Task.FromResult(GenResult<TUser>.BadRequestResult(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(newLeaderId, team.Name)));


        return Task.FromResult(GenResult<TUser>.Success(dbUser as TUser));
        // TODO: Remove "as TUser" when we a Generic Db Context
    }

    //------------------------------------//

    /// <summary>
    /// Finds the user with Id <paramref name="newLeaderId"/> and checks if the User/Principal making the <paramref name="request"/>
    /// can make the user with Id <paramref name="newLeaderId"/> the new Team Leader
    /// Use this if the actual team members can't do access their account for some reason .
    /// Must be Mntc or Super Team Member to access this method, otherwise it will return Forbidden.
    /// </summary>
    /// <param name="newLeaderId">User to Update's Identifier</param>
    /// <param name="request">The Mediator request with Principal information</param>
    /// <returns>Result with the user, if it exists and the Principal has permission to make change Leader</returns>
    public async Task<GenResult<Team>> CanChange_SpecifiedTeam_LeaderAsync(Guid? teamId, Guid? newLeaderId, IIdUserAndTeamAwareRequest<TUser> request)
    {

        if (teamId is null)
            return GenResult<Team>.NotFoundResult(IDMsgs.Error.NotFound<Team>(teamId));

        if (newLeaderId is null)
            return GenResult<Team>.NotFoundResult(IDMsgs.Error.NotFound<TUser>(newLeaderId));


        //Only Mntc & Super can access
        if (request.IsCustomer)
            return GenResult<Team>.ForbiddenResult();

        var newLeaderTeam = await _teamMgr.GetByIdWithMemberAsync(teamId, newLeaderId);

        //Team must exist
        if (newLeaderTeam is null)
            return GenResult<Team>.NotFoundResult(IDMsgs.Error.NotFound<Team>(newLeaderId));


        //Mntc Members can't change the Super Team
        if (request.IsMntc && newLeaderTeam.TeamType == TeamType.Super)
            return GenResult<Team>.ForbiddenResult(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(TeamType.Super));


        //NewLeader must be in team
        var dbUser = newLeaderTeam.Members.FirstOrDefault(m => m.Id == newLeaderId);
        if (dbUser is null)
            return GenResult<Team>.BadRequestResult(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(newLeaderId.Value, newLeaderTeam.Name));

        //Mntc & Super can adminster any Customer
        if (newLeaderTeam.IsCustomer())
            return GenResult<Team>.Success(newLeaderTeam);


        // Super can adminster any Mntc
        if (newLeaderTeam.IsMntc() && request.IsSuper)
            return GenResult<Team>.Success(newLeaderTeam);

        //If we're here the request.PrincipalTeam and the newLeaderTeam are the same team so only the leader can update
        //Principal must be Leader
        var principalId = request.PrincipalUserId;
        if (newLeaderTeam.LeaderId != principalId)
            return GenResult<Team>.ForbiddenResult(IDMsgs.Error.Authorization.UNAUTHORIZED_FOR_POSITION("Leader"));

        return GenResult<Team>.Success(newLeaderTeam);
    }

    //------------------------------------//

}//Cls
