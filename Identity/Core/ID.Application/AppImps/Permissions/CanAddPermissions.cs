using MyResults;
using ID.Application.AppAbs.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;


namespace ID.Application.AppImps.Permissions;
internal class CanAddPermissions() : ICanAddPermissions
{

    //------------------------------------//

    /// <summary>
    /// Checks if request.Principal is allowed to add a Team Member with position newMemberPosition
    /// </summary>
    /// <param name="newMemberPosition">New memeber's Position in the Team</param>
    /// <param name="request">The Mediator request with Principal information</param>
    /// <returns>BasicResult</returns>
    public Task<BasicResult> CanAddTeamMember(int newMemberPosition, IIdPrincipalInfoRequest request)
    {
        var principalPosition = request.PrincipalTeamPosition;
        var principalIsLeader = request.IsLeader;


        //Leader can do whatever they want
        if (principalIsLeader)
            return Task.FromResult(BasicResult.Success());


        //Can't add higher positions
        //Principal position must be at least newMemberPosition
        if (newMemberPosition > principalPosition)
            return Task.FromResult(BasicResult.ForbiddenResult(IDMsgs.Error.Authorization.UNAUTHORIZED_FOR_POSITION(newMemberPosition)));

        //Extra conditions go here
        //...
        //Min position to add member?

        return Task.FromResult(BasicResult.Success());
    }

    //------------------------------------//

    //In case CUstomer teams have separate rules.
    /// <summary>
    /// Checks if request.Principal is allowed to add a Team Member with position newMemberPosition
    /// </summary>
    /// <param name="newMemberPosition">New memeber's Position in the Team</param>
    /// <param name="request">The Mediator request with Principal information</param>
    /// <returns>BasicResult</returns>
    public Task<BasicResult> CanAddCustomerTeamMember(int newMemberPosition, IIdPrincipalInfoRequest request)
    {
        var principalPosition = request.PrincipalTeamPosition;
        var principalIsLeader = request.IsLeader;

        if (!request.IsCustomer)
            return Task.FromResult(BasicResult.ForbiddenResult(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(TeamType.customer)));


        //Leader can do whatever they want
        if (principalIsLeader)
            return Task.FromResult(BasicResult.Success());

        //Principal position must be at least newMemberPosition
        if (newMemberPosition > principalPosition)
            return Task.FromResult(BasicResult.ForbiddenResult(IDMsgs.Error.Authorization.UNAUTHORIZED_FOR_POSITION(newMemberPosition)));

        //Extra conditions go here
        //...
        //Min position to add member?

        return Task.FromResult(BasicResult.Success());
    }

    //------------------------------------//

}//Cls
