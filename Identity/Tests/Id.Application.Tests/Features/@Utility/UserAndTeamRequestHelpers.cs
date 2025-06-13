using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;

namespace ID.Application.Tests.Features.Utility;
internal static class UserAndTeamRequestHelpers
{

    /// <summary>
    /// Set the request to be authenticated as a MNTC user.
    /// For use in Validation Tests
    /// </summary>
    internal static TRequest SetAuthenticated_MNTC<TRequest, TUser>(this TRequest request, bool isLeader = false)
        where TRequest : IIdUserAndTeamAwareRequest<TUser>
        where TUser : AppUser
    {
        var leaderId = Guid.NewGuid();
        var team = TeamDataFactory.Create(leaderId: isLeader ? leaderId: Guid.NewGuid());

        request.IsAuthenticated = true;
        request.IsCustomer = false;
        request.IsMntc = true;
        request.IsSuper = false;
        request.IsSuper = false;
        request.Principal = new System.Security.Claims.ClaimsPrincipal();
        request.PrincipalEmail = "a@b.cd";
        request.PrincipalTeamId = Guid.NewGuid();
        request.PrincipalTeamPosition = 500;
        request.PrincipalUserId = isLeader? leaderId: Guid.NewGuid();
        request.PrincipalUsername = "abc";
        request.SetTeam(team);

        return request;
    }

    //------------------------------------//

    /// <summary>
    /// Set the request to be authenticated as a SUPER user.
    /// For use in Validation Tests
    /// </summary>
    internal static TRequest SetAuthenticated_SUPER<TRequest, TUser>(this TRequest request, bool isLeader = false)
        where TRequest : IIdUserAndTeamAwareRequest<TUser>
        where TUser : AppUser
    {
        var leaderId = Guid.NewGuid();
        var team = TeamDataFactory.Create(leaderId: isLeader ? leaderId : Guid.NewGuid());
        request.IsAuthenticated = true;
        request.IsCustomer = false;
        request.IsMntc = false;
        request.IsSuper = true;
        request.IsSuper = true;
        request.Principal = new System.Security.Claims.ClaimsPrincipal();
        request.PrincipalEmail = "a@b.cd";
        request.PrincipalTeamId = Guid.NewGuid();
        request.PrincipalTeamPosition = 500;
        request.PrincipalUserId = Guid.NewGuid();
        request.PrincipalUsername = "abc";
        request.SetTeam(team);

        return request;
    }

    //------------------------------------//

    /// <summary>
    /// Set the request to be authenticated as a CUSTOMER user.
    /// For use in Validation Tests
    /// </summary>
    internal static TRequest SetAuthenticated_CUSTOMER<TRequest, TUser>(this TRequest request, bool isLeader = false)
        where TRequest : IIdUserAndTeamAwareRequest<TUser>
        where TUser : AppUser
    {
        var leaderId = Guid.NewGuid();
        var team = TeamDataFactory.Create(leaderId: isLeader ? leaderId : Guid.NewGuid());
        request.IsAuthenticated = true;
        request.IsCustomer = true;
        request.IsMntc = false;
        request.IsMntc = false;
        request.IsSuper = false;
        request.IsSuper = false;
        request.Principal = new System.Security.Claims.ClaimsPrincipal();
        request.PrincipalEmail = "a@b.cd";
        request.PrincipalTeamId = Guid.NewGuid();
        request.PrincipalTeamPosition = 500;
        request.PrincipalUserId = Guid.NewGuid();
        request.PrincipalUsername = "abc";
        request.SetTeam(team);

        return request;
    }

    //------------------------------------//

}
