using CollectionHelpers;
using ID.Application.Authenticators.Teams;
using ID.Domain.Claims;
using ID.Tests.Data.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Shouldly;
using System.Security.Claims;
using Xunit.Abstractions;

namespace ID.Application.Tests.Authenticators;

public class MntcMinimumAuthenticatorTests(ITestOutputHelper _output)
{

    record TestData(List<Claim> Claims, int? RequiredPosition);

    [Fact]
    public async Task ShouldReturnTrue_WhenUserIsAuthorizedAsync()
    {
        List<TestData> dataLists = [
            new TestData([IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.TEAM_POSITION(0)], null),
            new TestData([IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.TEAM_POSITION(11)], 10),
            new TestData([IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.TEAM_POSITION(5)],4  ),
            new TestData([IdTeamClaims.SUPER_TEAM, IdTeamClaims.TEAM_POSITION(0)], null),  //Higher Team
            new TestData([IdTeamClaims.SUPER_TEAM, IdTeamClaims.TEAM_POSITION(11)], 10),   //Higher Team
            new TestData([IdTeamClaims.SUPER_TEAM, IdTeamClaims.TEAM_POSITION(5)],4  ),    //Higher Team
            new TestData([IdTeamClaims.SUPER_TEAM, IdTeamClaims.TEAM_POSITION(5)],4  ),    //Higher Team
            new TestData([IdTeamClaims.SUPER_TEAM,],4  ),    //Higher Team
        ];

        foreach (var data in dataLists)
            await TestShouldPassClaims(data.Claims, data.RequiredPosition);
    }

    //- - - - - - - - - - - - - - - //

    private static async Task TestShouldPassClaims(List<Claim> claims, int? requiredPosition)
    {
        // Arrange
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new MntcMinimumAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        MntcMinimumAuthenticator.ResourceFilter resourceFilter = requiredPosition is null
            ? new()
            : new(requiredPosition.Value);


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        MntcMinimumAuthenticator.ActionFilter actionFilter = requiredPosition is null
            ? new()
            : new(requiredPosition.Value);


        // Act
        var handleResult = authHandler.IsAuthorized(httpContext, requiredPosition);
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

        List<TestData> datasLists = [
            new TestData([IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.TEAM_POSITION(-5)], null),
            new TestData([IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.TEAM_POSITION(10)], 12),
            new TestData([IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.TEAM_POSITION(4)],5  ),
            new TestData([IdTeamClaims.CUSTOMER_TEAM, IdTeamClaims.TEAM_POSITION(0)], null),//Wrong Team
            new TestData([IdTeamClaims.CUSTOMER_TEAM, IdTeamClaims.TEAM_POSITION(11)], 10),//Wrong Team
            new TestData([IdTeamClaims.CUSTOMER_TEAM, IdTeamClaims.TEAM_POSITION(5)],4  ),//Wrong Team
            new TestData([IdTeamClaims.TEAM_POSITION(5)],4  ), //No team
        ];

        foreach (var data in datasLists)
            await TestShouldForbidClaims(data.Claims, data.RequiredPosition);
    }

    //- - - - - - - - - - - - - - - //

    private async Task TestShouldForbidClaims(List<Claim> claims, int? requiredPosition)
    {
        // Arrange
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new MntcMinimumAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        MntcMinimumAuthenticator.ResourceFilter resourceFilter = requiredPosition is null
              ? new()
              : new(requiredPosition.Value);


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        MntcMinimumAuthenticator.ActionFilter actionFilter = requiredPosition is null
              ? new()
              : new(requiredPosition.Value);

        // Act
        var handleResult = authHandler.IsAuthorized(httpContext, requiredPosition);
        actionFilter.OnActionExecuting(actionContext);
        await resourceFilter.OnResourceExecutionAsync(resourceExecutingContext, resourceFilterNext.Object);

        // Assert
        _output.WriteLine($"requiredPosition: {requiredPosition}");
        _output.WriteLine($"claims: {claims.Select(c => $"{c.Type}: {c.Value}{Environment.NewLine}").JoinStr(", ")}");
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
        var authHandler = new MntcMinimumAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new MntcMinimumAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new MntcMinimumAuthenticator.ActionFilter();

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
