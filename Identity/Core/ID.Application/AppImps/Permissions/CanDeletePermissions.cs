using MyResults;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;


namespace ID.Application.AppImps.Permissions;

//IS this how to add GEneric TUser everwhere???
internal class CanDeletePermissions<TUser>(IIdentityTeamManager<TUser> _teamMgr) : ICanDeletePermissions<TUser> where TUser : AppUser
{

    //------------------------------------//

    /// <summary>
    /// Finds the user with Id <paramref name="deleteUserId"/> and checks if the User/Principal making the <paramref name="request"/>
    /// can delete the with Id <paramref name="deleteUserId"/> 
    /// </summary>
    /// <param name="deleteUserId">User to Delete's Identifier</param>
    /// <param name="request">The Mediator request with Principal information</param>
    /// <returns>Result with the user, if it exists and the Principal has permission delete it</returns>
    public async Task<GenResult<TUser>> CanDeleteAsync(Guid? deleteUserId, IIdPrincipalInfoRequest request)
    {
        var deleteUser = await _teamMgr.GetMemberAsync(request.PrincipalTeamId, deleteUserId);

        return CanDeleteAsync(deleteUserId, request, deleteUser);
    }

    //------------------------------------//

    /// <summary>
    /// Finds the user with Id <paramref name="deleteUserId"/> and checks if the User/Principal making the <paramref name="request"/>
    /// can delete the with Id <paramref name="deleteUserId"/> 
    /// </summary>
    /// <param name="deleteUserId">User to Delete's Identifier</param>
    /// <param name="request">The Mediator request with Principal information</param>
    /// <returns>Result with the user, if it exists and the Principal has permission delete it</returns>
    public Task<GenResult<TUser>> CanDeleteAsync(Guid? deleteUserId, IIdUserAndTeamAwareRequest<TUser> request)
    {
        var deleteUser = request.PrincipalTeam.Members
            .FirstOrDefault(u => u.Id == deleteUserId);
        return Task.FromResult(CanDeleteAsync(deleteUserId, request, deleteUser as TUser));
        // TODO: Remove "as TUser" when we a Generic Db Context

    }


    //------------------------------------//


    private static GenResult<TUser> CanDeleteAsync(Guid? deleteUserId, IIdPrincipalInfoRequest request, TUser? deleteUser)
    {
        //User must exist
        if (deleteUser is null)
            return GenResult<TUser>.NotFoundResult(IDMsgs.Error.NotFound<AppUser>(deleteUserId));

        //Can't delete yourself
        if (request.PrincipalUserId == deleteUser.Id)
            return GenResult<TUser>.BadRequestResult(IDMsgs.Error.Users.CantDeleteSelf);

        //User must be on the same team (might be unnecessary)
        if (deleteUser.TeamId != request.PrincipalTeamId)
            return GenResult<TUser>.ForbiddenResult(IDMsgs.Error.Users.CantDeleteFromAnotherTeam);

        //Leader can do whatever they want
        if (request.IsLeader)
            return GenResult<TUser>.Success(deleteUser);

        //Can only delete lower positions unless you're the leader
        if (request.PrincipalTeamPosition <= deleteUser.TeamPosition)
            return GenResult<TUser>.ForbiddenResult(IDMsgs.Error.Authorization.UNAUTHORIZED_FOR_POSITION(deleteUser.TeamPosition));

        return GenResult<TUser>.Success(deleteUser);
    }

    //------------------------------------//

}//Cls
