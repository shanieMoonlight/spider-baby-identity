using ID.GlobalSettings.Claims;
using ID.TeamRoles.UserToAdmin.Authenticators.Mntc;
using ID.TeamRoles.UserToAdmin.Claims;
using ID.Tests.Data.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Shouldly;
using System.Security.Claims;

namespace ID.TeamRoles.UserToAdmin.Tests.Authenticators;

public class MntcUserTests
{
    [Fact]
    public async Task AuthHandler_IsAuthorized_ShouldReturnTrue_WhenUserIsInMntcTeamAndUserRoleAsync()
    {
        // Arrange
        List<Claim> claims = [IdTeamClaims.MAINTENANCE_TEAM, IdTeamRoleClaims.USER];
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new MntcUserAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new MntcUserAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new MntcUserAuthenticator.ActionFilter();

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
    public async Task AuthHandler_IsAuthorized_ShouldReturnFalse_WhenUserIsNotInMntcTeamAsync()
    {
        // Arrange
        List<Claim> claims = [IdTeamClaims.SUPER_TEAM, IdTeamRoleClaims.USER];
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new MntcUserAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new MntcUserAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new MntcUserAuthenticator.ActionFilter();

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
    public async Task AuthHandler_IsAuthorized_ShouldReturnFalse_WhenUserIsNotInUserRoleAsync()
    {
        // Arrange
        List<Claim> claims = [IdTeamClaims.MAINTENANCE_TEAM, IdTeamRoleClaims.ADMIN];
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new MntcUserAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new MntcUserAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new MntcUserAuthenticator.ActionFilter();

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
        var authHandler = new MntcUserAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new MntcUserAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new MntcUserAuthenticator.ActionFilter();

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

