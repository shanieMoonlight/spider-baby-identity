using MyResults;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;


namespace ID.Application.AppImps.Permissions;
internal class CanUpdatePermissions<TUser>(IIdentityTeamManager<TUser> _teamMgr) : ICanUpdatePermissions<TUser> where TUser : AppUser
{

    //------------------------------------//

    /// <summary>
    /// Finds the user with Id <paramref name="updateUserId"/> and checks if the User/Principal making the <paramref name="request"/>
    /// can update the with Id <paramref name="updateUserId"/> 
    /// </summary>
    /// <param name="updateUserId">User to Update's Identifier</param>
    /// <param name="request">The Mediator request with Principal information</param>
    /// <returns>Result with the user, if it exists and the Principal has permission update` it</returns>
    public async Task<GenResult<TUser>> CanUpdateAsync(Guid updateUserId, IIdPrincipalInfoRequest request)
    {
        var updateUser = await _teamMgr.GetMemberAsync(request.PrincipalTeamId, updateUserId);

        //User must exist
        if (updateUser is null)
            return GenResult<TUser>.NotFoundResult(IDMsgs.Error.NotFound<TUser>(updateUserId));


        //Can't Update someone else
        if (request.PrincipalUserId != updateUser.Id)
            return GenResult<TUser>.BadRequestResult(IDMsgs.Error.Users.CanOnlyUpdateSelf);


        return GenResult<TUser>.Success(updateUser);
    }

    //------------------------------------//

    /// <summary>
    /// Finds the user with Id <paramref name="updateUserId"/> and checks if the User/Principal making the <paramref name="request"/>
    /// can update the with Id <paramref name="updateUserId"/> 
    /// </summary>
    /// <param name="updateUserId">User to Update's Identifier</param>
    /// <param name="request">The Mediator request with Principal information</param>
    /// <returns>Result with the user, if it exists and the Principal has permission update` it</returns>
    public Task<GenResult<TUser>> CanUpdateAsync(Guid updateUserId, IIdUserAwareRequest<TUser> request)
    {
        //User must exist
        if (request.PrincipalUser is null)
            return Task.FromResult(GenResult<TUser>.NotFoundResult(IDMsgs.Error.NotFound<TUser>(updateUserId)));


        //Can't Update someone else
        if (request.PrincipalUserId != updateUserId)
            return Task.FromResult(GenResult<TUser>.BadRequestResult(IDMsgs.Error.Users.CanOnlyUpdateSelf));


        return Task.FromResult(GenResult<TUser>.Success(request.PrincipalUser));
    }

    //------------------------------------//

}//Cls
