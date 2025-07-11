using ID.GlobalSettings.Claims;
using ID.TeamRoles.UserToAdmin.Authenticators.Spr;
using ID.TeamRoles.UserToAdmin.Claims;
using ID.Tests.Data.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Shouldly;
using System.Security.Claims;

namespace ID.TeamRoles.UserToAdmin.Tests.Authenticators;

public class SuperMgrTests
{
    [Fact]
    public async Task AuthHandler_IsAuthorized_ShouldReturnTrue_WhenUserIsInSuperTeamAndManagerRoleAsync()
    {
        // Arrange
        List<Claim> claims = [IdTeamClaims.SUPER_TEAM, IdTeamRoleClaims.MANAGER];
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new SuperManagerAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new SuperManagerAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new SuperManagerAuthenticator.ActionFilter();

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
    public async Task AuthHandler_IsAuthorized_ShouldReturnFalse_WhenUserIsNotInSuperTeamAsync()
    {
        // Arrange
        List<Claim> claims = [IdTeamClaims.MAINTENANCE_TEAM, IdTeamRoleClaims.MANAGER];
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new SuperManagerAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new SuperManagerAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new SuperManagerAuthenticator.ActionFilter();

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
    public async Task AuthHandler_IsAuthorized_ShouldReturnFalse_WhenUserIsNotInManagerRoleAsync()
    {
        // Arrange
        List<Claim> claims = [IdTeamClaims.SUPER_TEAM, IdTeamRoleClaims.USER];
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new SuperManagerAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new SuperManagerAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new SuperManagerAuthenticator.ActionFilter();

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
        var authHandler = new SuperManagerAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new SuperManagerAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new SuperManagerAuthenticator.ActionFilter();

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
