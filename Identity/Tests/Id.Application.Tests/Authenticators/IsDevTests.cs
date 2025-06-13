using ID.Application.Authenticators;
using ID.Tests.Data.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Shouldly;
using System.Security.Claims;
using Xunit.Abstractions;

namespace ID.Application.Tests.Authenticators;

public class DevAccessAuthenticatorTests(ITestOutputHelper _output)
{

    record TestData(List<Claim> Claims, int? RequiredPosition);
 

    //- - - - - - - - - - - - - - - //

    [Fact]
    public async Task TestShouldInDevMode()
    {

        var httpContext = ContextProvider.GetHttpContext(true, [], isAuthenticated: false); // isAuthenticated: false => Should fail without dev mode


        var authHandler = new DevAccessAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        DevAccessAuthenticator.ResourceFilter resourceFilter = new();


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        DevAccessAuthenticator.ActionFilter actionFilter = new();


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
    public  async Task TestShouldUnauthorized_WhenNOtAuthenticate()
    {

        // Arrange
        var httpContext = ContextProvider.GetHttpContext(false, [], isAuthenticated: false); // isAuthenticated: false => Should fail without dev mode
        var authHandler = new DevAccessAuthenticator.AuthHandler();


        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        DevAccessAuthenticator.ResourceFilter resourceFilter = new();


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        DevAccessAuthenticator.ActionFilter actionFilter = new();

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

    [Fact]
    public async Task TestShouldForbid_WhenAuthenticated()
    {

        // Arrange
        var httpContext = ContextProvider.GetHttpContext(false, [], isAuthenticated: true); // isAuthenticated: false => Should fail without dev mode
        var authHandler = new DevAccessAuthenticator.AuthHandler();


        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        DevAccessAuthenticator.ResourceFilter resourceFilter = new();


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        DevAccessAuthenticator.ActionFilter actionFilter = new();

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

}//Cls
