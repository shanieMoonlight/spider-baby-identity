using ID.GlobalSettings.Claims;
using ID.TeamRoles.UserToAdmin.Authenticators.Customers;
using ID.TeamRoles.UserToAdmin.Claims;
using ID.Tests.Data.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Shouldly;
using System.Security.Claims;

namespace ID.TeamRoles.UserToAdmin.Tests.Authenticators;

public class CustomerMgrMinTests
{
    [Fact]
    public async Task AuthHandler_IsAuthorized_ShouldReturnTrue_WhenUserIsAuthorizedAsync()
    {

        List<List<Claim>> claimsLists = [
            [IdTeamClaims.CUSTOMER_TEAM, IdTeamRoleClaims.MANAGER],
            [IdTeamClaims.CUSTOMER_TEAM, IdTeamRoleClaims.ADMIN],
            [IdTeamClaims.SUPER_TEAM],
            [IdTeamClaims.MAINTENANCE_TEAM]
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
        var authHandler = new CustomerManagerMinimumAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new CustomerManagerMinimumAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new CustomerManagerMinimumAuthenticator.ActionFilter();

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
    public async Task AuthHandler_IsAuthorized_ShouldReturnFalse_WhenUserIsNotInManagerRoleAsync()
    {
        // Arrange
        List<Claim> claims = [IdTeamClaims.CUSTOMER_TEAM, IdTeamRoleClaims.USER];
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new CustomerManagerMinimumAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new CustomerManagerMinimumAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new CustomerManagerMinimumAuthenticator.ActionFilter();

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
        var authHandler = new CustomerManagerMinimumAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new CustomerManagerMinimumAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new CustomerManagerMinimumAuthenticator.ActionFilter();

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
