using ID.Application.Authenticators;
using ID.Domain.Claims;
using ID.Tests.Data.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Shouldly;
using Xunit.Abstractions;

namespace ID.Application.Tests.Authenticators;

public class MfaVerifiedAuthenticatorTests(ITestOutputHelper _output)
{
    //------------------------------//

    [Fact]
    public async Task ShouldPassWhen_Verified()
    {
        var httpContext = ContextProvider.GetHttpContext([TwoFactorClaims.TwoFactorVerified], true);

        var authHandler = new MfaVerifiedAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        MfaVerifiedAuthenticator.ResourceFilter resourceFilter = new();


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        MfaVerifiedAuthenticator.ActionFilter actionFilter = new();


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
    public async Task TestShouldUnauthorized_WhenNotVerified()
    {

        // Arrange
        var httpContext = ContextProvider.GetHttpContext([], true);
        var authHandler = new MfaVerifiedAuthenticator.AuthHandler();


        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        MfaVerifiedAuthenticator.ResourceFilter resourceFilter = new();


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        MfaVerifiedAuthenticator.ActionFilter actionFilter = new();

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
