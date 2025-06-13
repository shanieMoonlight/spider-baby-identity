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

public class LeaderAuthenticatorTests(ITestOutputHelper _output)
{

    record TestData(List<Claim> Claims, int? RequiredPosition);

    [Fact]
    public async Task ShouldReturnTrue_WhenUserIsAuthorizedAsync()
    {
        List<List<Claim>> claimsLists = [
            [IdTeamClaims.CUSTOMER_TEAM, IdTeamClaims.LEADER], 
            [IdTeamClaims.SUPER_TEAM, IdTeamClaims.LEADER],
            [IdTeamClaims.MAINTENANCE_TEAM, IdTeamClaims.LEADER],
            [IdTeamClaims.LEADER], //No team - but still leader.
        ];

        foreach (var claims in claimsLists)
            await TestShouldPassClaims(claims);
    }

    //- - - - - - - - - - - - - - - //

    private static async Task TestShouldPassClaims(List<Claim> claims)
    {
        // Arrange
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new LeaderAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        LeaderAuthenticator.ResourceFilter resourceFilter = new();


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        LeaderAuthenticator.ActionFilter actionFilter =  new();


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
            [IdTeamClaims.CUSTOMER_TEAM],//Not leader
            [IdTeamClaims.MAINTENANCE_TEAM],//Not leader
            [IdTeamClaims.SUPER_TEAM]//Not leader
        ];

        foreach (var data in claimsLists)
            await TestShouldForbidClaims(data);
    }

    //- - - - - - - - - - - - - - - //

    private async Task TestShouldForbidClaims(List<Claim> claims)
    {
        // Arrange
        var httpContext = ContextProvider.GetHttpContext(claims, true);
        var authHandler = new LeaderAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        LeaderAuthenticator.ResourceFilter resourceFilter = new();


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        LeaderAuthenticator.ActionFilter actionFilter = new();

        // Act
        var handleResult = authHandler.IsAuthorized(httpContext);
        actionFilter.OnActionExecuting(actionContext);
        await resourceFilter.OnResourceExecutionAsync(resourceExecutingContext, resourceFilterNext.Object);

        // Assert
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
        var authHandler = new LeaderAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        var resourceFilter = new LeaderAuthenticator.ResourceFilter();

        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        var actionFilter = new LeaderAuthenticator.ActionFilter();

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
