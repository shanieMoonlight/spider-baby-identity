using ID.GlobalSettings.Claims;
using ID.TeamRoles.UserToAdmin.Authenticators.Customers;
using ID.TeamRoles.UserToAdmin.Authenticators.Mntc;
using ID.TeamRoles.UserToAdmin.Claims;
using ID.Tests.Data.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Shouldly;
using System.Security.Claims;

namespace ID.TeamRoles.UserToAdmin.Tests.Authenticators;

public class MntcAdminMinTests
{
    [Fact]
    public async Task ShouldReturnTrue_WhenUserIsAuthorizedAsync()
    {

        List<List<Claim>> claimsLists = [
            [IdTeamClaims.MAINTENANCE_TEAM, IdTeamRoleClaims.ADMIN],
            [IdTeamClaims.SUPER_TEAM]
        ];

        foreach (var item in claimsLists)
        {
            await TestShouldPassClaims(item);
        }
    }

    //- - - - - - - - - - - - - - - //

    private static async Task TestShouldPassClaims(List<Claim> claims)
    {
        // Arrange
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new MntcAdminMinimumAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new MntcAdminMinimumAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new MntcAdminMinimumAuthenticator.ActionFilter();

        // Act
        var handleResult = authHandler.IsAuthorized(httpContext);
        actionFilter.OnActionExecuting(actionContext);
        await resourceFilter.OnResourceExecutionAsync(resourceExecutingContext, resourceFilterNext.Object);

        // Assert
        handleResult.ShouldBeTrue();
        actionContext.Result.ShouldBeNull();
        resourceFilterNext.Verify(n => n(), Times.Once);
        resourceExecutingContext.Result.ShouldBeNull();
    }

    //------------------------------//

    [Fact]
    public async Task ShouldReturn_Forbid_WhenUserIsAuthorizedAsync()
    {

        List<List<Claim>> claimsLists = [
            [IdTeamClaims.CUSTOMER_TEAM, IdTeamRoleClaims.USER],
            [IdTeamClaims.CUSTOMER_TEAM, IdTeamRoleClaims.MANAGER],
            [IdTeamClaims.CUSTOMER_TEAM, IdTeamRoleClaims.ADMIN],
            [IdTeamClaims.MAINTENANCE_TEAM],
            [IdTeamClaims.MAINTENANCE_TEAM, IdTeamRoleClaims.USER],
            [IdTeamClaims.MAINTENANCE_TEAM, IdTeamRoleClaims.MANAGER],
            []
        ];

        foreach (var item in claimsLists)
        {
            await TestShouldForbidClaims(item);
        }
    }
    //- - - - - - - - - - - - - - - //

    private static async Task TestShouldForbidClaims(List<Claim> claims)
    {
        // Arrange
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new MntcAdminMinimumAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new MntcAdminMinimumAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new MntcAdminMinimumAuthenticator.ActionFilter();

        // Act
        var handleResult = authHandler.IsAuthorized(httpContext);
        actionFilter.OnActionExecuting(actionContext);
        await resourceFilter.OnResourceExecutionAsync(resourceExecutingContext, resourceFilterNext.Object);

        // Assert
        handleResult.ShouldBeFalse();
        resourceExecutingContext.Result.ShouldBeOfType<ForbidResult>();
        actionContext.Result.ShouldBeOfType<ForbidResult>();
    }

    //------------------------------//

    [Fact]
    public async Task AuthHandler_IsAuthorized_ShouldReturnFalse_WhenUserIsNotAuthenticatedAsync()
    {
        // Arrange
        List<Claim> claims = [];
        var httpContext = ContextProvider.GetHttpContext(claims, false);
        var authHandler = new MntcAdminMinimumAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new MntcAdminMinimumAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new MntcAdminMinimumAuthenticator.ActionFilter();

        // Act
        var handleResult = authHandler.IsAuthorized(httpContext);
        actionFilter.OnActionExecuting(actionContext);
        await resourceFilter.OnResourceExecutionAsync(resourceExecutingContext, resourceFilterNext.Object);

        // Assert
        handleResult.ShouldBeFalse();
        resourceExecutingContext.Result.ShouldBeOfType<UnauthorizedResult>();
        actionContext.Result.ShouldBeOfType<UnauthorizedResult>();
    }

    //------------------------------//

}
