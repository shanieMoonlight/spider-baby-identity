using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using MyResults;


namespace ID.Application.AppImps.Permissions;
internal class CanChangePositionPermissions<TUser> : ICanChangePositionPermissions<TUser> where TUser : AppUser
{

    //------------------------------------//

    //Leder can do what ever they want. Position is capped at IdDomainSettings.MinTeamPosition - IdDomainSettings.MaxTeamPosition. But let the 
    // ValueObjects throw the error
    // Can't promote to higher Position than themselves (Principal.Position >= newPosition)
    // Team Leader Position cannot be changed (newPositionUser != Leader)
    // newPositionUser Must exist (!!newPositionUser)
    // Can't change the Position of a member that has a higher than or equal to Position than themselves (newPositionUser.Position < Principal)


    /// <summary>
    /// Finds the user with Id <paramref name="newPositionUserId"/> and checks if the User/Principal making the <paramref name="request"/>
    /// can delete the with Id <paramref name="newPositionUserId"/> 
    /// </summary>
    /// <param name="newPositionUserId">User to Delete's Identifier</param>
    /// <param name="request">The Mediator request with Principal information</param>
    /// <returns>Result with the user, if it exists and the Principal has permission delete it</returns>
    public Task<GenResult<TUser>> CanChangePositionAsync(Guid? newPositionUserId, int newPosition, IIdUserAndTeamAwareRequest<TUser> request)
    {
        //TODO Double check tests
        var principalTeam = request.PrincipalTeam;
        var newPositionUser = principalTeam.Members.FirstOrDefault(m => m.Id == newPositionUserId);
        var newPositionUserIsLeader = newPositionUser?.Id == principalTeam.LeaderId;
        var principalUser = request.PrincipalUser;
        var principalIsLeader = principalTeam.LeaderId == principalUser.Id;

        //User must exist and be on the same team
        if (newPositionUser is null)
            return Task.FromResult(GenResult<TUser>.ForbiddenResult(IDMsgs.Error.Users.CantUpdateFromAnotherTeam));


        //Leader can do whatever they want
        if (principalIsLeader)
            return Task.FromResult(GenResult<TUser>.Success((newPositionUser as TUser)!)); // TODO: Remove "as TUser" when we have a Generic Db Context


        //Can't change Leader position
        if (newPositionUserIsLeader)
            return Task.FromResult(GenResult<TUser>.BadRequestResult(IDMsgs.Error.Teams.CANT_CHANGE_POSITION_OF_LEADER));


        //Can't promote to higher level than themselves
        //Principal must NOT have a TeamPosition less than newPosition
        if (principalUser.TeamPosition < newPosition)
            return Task.FromResult(GenResult<TUser>.BadRequestResult(IDMsgs.Error.Teams.CANT_ADMINISTER_HIGHER_POSITIONS(newPosition)));


        //Can't update members with the same or higher positions.
        //Principal must NOT have a TeamPosition less than the current newPositionUser.TeamPosition
        if (principalUser.TeamPosition <= newPositionUser.TeamPosition)
            return Task.FromResult(GenResult<TUser>.BadRequestResult(IDMsgs.Error.Teams.CANT_ADMINISTER_HIGHER_POSITIONS(newPosition)));


        return Task.FromResult(GenResult<TUser>.Success((newPositionUser as TUser)!)); // TODO: Remove "as TUser" when we a Generic Db Context
    }


}//Cls
