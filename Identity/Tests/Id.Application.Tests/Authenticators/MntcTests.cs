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

public class MntcAuthenticatorTests(ITestOutputHelper _output)
{

    record TestData(List<Claim> Claims, int? RequiredPosition);

    [Fact]
    public async Task ShouldReturnTrue_WhenUserIsAuthorizedAsync()
    {
        List<TestData> dataLists = [
            new TestData([IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.TEAM_POSITION(-5)], null), //Null means don't check posiition
            new TestData([IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.TEAM_POSITION(0)], 0),
            new TestData([IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.TEAM_POSITION(11)], 11),
            new TestData([IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.TEAM_POSITION(555)],555  ),
        ];

        foreach (var data in dataLists)
            await TestShouldPassClaims(data.Claims, data.RequiredPosition);
    }

    //- - - - - - - - - - - - - - - //

    private static async Task TestShouldPassClaims(List<Claim> claims, int? requiredPosition)
    {
        // Arrange
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new MntcAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        MntcAuthenticator.ResourceFilter resourceFilter = requiredPosition is null
            ? new()
            : new(requiredPosition.Value);


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        MntcAuthenticator.ActionFilter actionFilter = requiredPosition is null
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
            new TestData([IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.TEAM_POSITION(10)], 12),//Wrong Position
            new TestData([IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.TEAM_POSITION(4)],5  ),//Wrong Position
            new TestData([IdTeamClaims.CUSTOMER_TEAM, IdTeamClaims.TEAM_POSITION(-1)], null),//Wrong Team - Right position
            new TestData([IdTeamClaims.CUSTOMER_TEAM, IdTeamClaims.TEAM_POSITION(11)], 11),//Wrong Team - Right position
            new TestData([IdTeamClaims.CUSTOMER_TEAM, IdTeamClaims.TEAM_POSITION(14)],14  ),//Wrong Team - Right position
            new TestData([IdTeamClaims.TEAM_POSITION(5)],4  ), //No team
            new TestData([IdTeamClaims.SUPER_TEAM, IdTeamClaims.TEAM_POSITION(0)], null),  //Wrong Team - Right position
            new TestData([IdTeamClaims.SUPER_TEAM, IdTeamClaims.TEAM_POSITION(11)], 10),   //Wrong Team - Right position
            new TestData([IdTeamClaims.SUPER_TEAM, IdTeamClaims.TEAM_POSITION(5)],5  ),    //Wrong Team - Right position
            new TestData([IdTeamClaims.SUPER_TEAM, IdTeamClaims.TEAM_POSITION(4)],4  ),    //Wrong Team - Right position
            new TestData([IdTeamClaims.SUPER_TEAM,],4  ),    //Wrong Team
        ];

        foreach (var data in datasLists)
            await TestShouldForbidClaims(data.Claims, data.RequiredPosition);
    }

    //- - - - - - - - - - - - - - - //

    private async Task TestShouldForbidClaims(List<Claim> claims, int? requiredPosition)
    {
        // Arrange
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new MntcAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        MntcAuthenticator.ResourceFilter resourceFilter = requiredPosition is null
              ? new()
              : new(requiredPosition.Value);


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        MntcAuthenticator.ActionFilter actionFilter = requiredPosition is null
              ? new()
              : new(requiredPosition.Value);

        // Act
        var handleResult = authHandler.IsAuthorized(httpContext, requiredPosition);
        actionFilter.OnActionExecuting(actionContext);
        await resourceFilter.OnResourceExecutionAsync(resourceExecutingContext, resourceFilterNext.Object);

        // Assert
        _output.WriteLine($"requiredPosition: {requiredPosition}");
        _output.WriteLine($"claims: {claims.Select(c => $"{c.Type}: {c.Value}{Environment.NewLine}").JoinStr(", ")}");
        _output.WriteLine($"handleResult: {handleResult}");
        _output.WriteLine($"actionContext: {actionContext.Result}");
        _output.WriteLine($"resourceExecutingContext: {resourceExecutingContext.Result}");
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
        var authHandler = new MntcAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new MntcAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new MntcAuthenticator.ActionFilter();

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
