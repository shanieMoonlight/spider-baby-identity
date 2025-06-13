using ID.Application.Mediatr.Cqrslmps;

namespace ID.Application.Customers.Tests.Features.@Utility;
internal static class AuthRequestHelpersExtensions
{

    /// <summary>
    /// Set the request to be authenticated as a MNTC user.
    /// For use in Validation Tests
    /// </summary>
    internal static TRequest SetAuthenticated_MNTC<TRequest>(this TRequest request) where TRequest : APrincipalInfoRequest
    {
        request.IsAuthenticated = true;
        request.IsCustomer = false;
        request.IsMntc = true;
        request.IsSuper = false;
        request.Principal = new System.Security.Claims.ClaimsPrincipal();
        request.PrincipalEmail = "a@b.cd";
        request.PrincipalTeamId = Guid.NewGuid();
        request.PrincipalTeamPosition = 500;
        request.PrincipalUserId = Guid.NewGuid();
        request.PrincipalUsername = "abc";

        return request;
    }

    //------------------------------------//

    /// <summary>
    /// Set the request to be authenticated as a SUPER user.
    /// For use in Validation Tests
    /// </summary>
    internal static TRequest SetAuthenticated_SUPER<TRequest>(this TRequest request) where TRequest : APrincipalInfoRequest
    {
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

        return request;
    }

    //------------------------------------//

    /// <summary>
    /// Set the request to be authenticated as a CUSTOMER user.
    /// For use in Validation Tests
    /// </summary>
    internal static TRequest SetAuthenticated_CUSTOMER<TRequest>(this TRequest request) where TRequest : APrincipalInfoRequest
    {
        request.IsAuthenticated = true;
        request.IsCustomer = true;
        request.IsMntc = false;
        request.IsSuper = false;
        request.Principal = new System.Security.Claims.ClaimsPrincipal();
        request.PrincipalEmail = "a@b.cd";
        request.PrincipalTeamId = Guid.NewGuid();
        request.PrincipalTeamPosition = 500;
        request.PrincipalUserId = Guid.NewGuid();
        request.PrincipalUsername = "abc";

        return request;
    }

    //------------------------------------//

}
