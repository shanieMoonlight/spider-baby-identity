using ID.Application.Authenticators;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using ID.Tests.Data.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit.Abstractions;

namespace ID.Application.Tests.Authenticators;

public class InitializedAuthenticatorTests(ITestOutputHelper _output)
{
    //------------------------------//

    private static ServiceProvider GetServiceProvider(bool initialized)
    {
        var mockEnvironment = new Mock<IIdentityTeamManager<AppUser>>();
        if (!initialized)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            mockEnvironment.Setup(e => e.GetSuperTeamWithMembersAsync(It.IsAny<int>())).ReturnsAsync((Team)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        }
        else
        {
            var teamId = Guid.NewGuid();
            var superLeader = AppUserDataFactory.Create(teamId: teamId);
            var team = TeamDataFactory.Create(id: teamId, leader: superLeader);
            mockEnvironment.Setup(e => e.GetSuperTeamWithMembersAsync(It.IsAny<int>())).ReturnsAsync(team);
        }

        return new ServiceCollection()
            .AddSingleton(mockEnvironment.Object)
            .BuildServiceProvider();
    }

    //- - - - - - - - - - - - - - - //

    private static HttpContext GetHttpContext(bool initialized)
    {
        var serviceProvider = GetServiceProvider(initialized);
        var httpContext = ContextProvider.GetHttpContext([], isAuthenticated: true);
        httpContext.RequestServices = serviceProvider;
        return httpContext;
    }

    //------------------------------//

    [Fact]
    public async Task ShouldPassWhen_NOT_Initialized ()
    {

        var httpContext = GetHttpContext(false); 


        var authHandler = new InitializedAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        InitializedAuthenticator.ResourceFilter resourceFilter = new();


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        InitializedAuthenticator.ActionFilter actionFilter = new();


        // Act
        var handleResult = authHandler.IsAuthorized(httpContext);
        actionFilter.OnActionExecuting(actionContext);
        await resourceFilter.OnResourceExecutionAsync(resourceExecutingContext, resourceFilterNext.Object);

        // Assert
        handleResult.ShouldBeTrue();
        actionContext.Result.ShouldBeNull();
        resourceFilterNext.Verify(n => n(), Times.Once);
        resourceExecutingContext.Result.ShouldBeNull();
        _output.WriteLine("Result: " + resourceExecutingContext.Result?.ToString());
    }


    //------------------------------//

    [Fact]
    public async Task TestShouldUnauthorized_WhenInitialized()
    {

        // Arrange
        var httpContext = GetHttpContext(true);
        var authHandler = new InitializedAuthenticator.AuthHandler();


        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        InitializedAuthenticator.ResourceFilter resourceFilter = new();


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        InitializedAuthenticator.ActionFilter actionFilter = new();

        // Act
        var handleResult = authHandler.IsAuthorized(httpContext);
        actionFilter.OnActionExecuting(actionContext);
        await resourceFilter.OnResourceExecutionAsync(resourceExecutingContext, resourceFilterNext.Object);

        // Assert
        handleResult.ShouldBeFalse();
        resourceExecutingContext.Result.ShouldBeOfType<NotFoundResult>();
        actionContext.Result.ShouldBeOfType<NotFoundResult>();
    }

    //------------------------------//

}//Cls
