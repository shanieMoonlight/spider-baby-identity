using ID.Application.Authenticators;
using ID.Application.JWT.Subscriptions;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using ID.Tests.Data.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Shouldly;
using Xunit.Abstractions;

namespace ID.Application.Tests.Authenticators;

public class ValidDeviceAuthenticatorTests(ITestOutputHelper _output)
{

    [Fact]
    public async Task ShouldNotPassWhen_ValidDeviced()
    {

        var subName = "TestSubPlan123426";
        var device = DeviceDataFactory.Create();
        var subPlan = SubscriptionPlanDataFactory.Create(name: subName);
        var teamSubscription = SubscriptionDataFactory.Create(plan: subPlan, devices: [device]);
        var claims = new List<TeamSubscription>() { teamSubscription }.ToClaims(device.UniqueId);
        var httpContext = ContextProvider.GetHttpContext(claims, true);


        var authHandler = new ValidDeviceAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        ValidDeviceAuthenticator.ResourceFilter resourceFilter = new(subName);


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        ValidDeviceAuthenticator.ActionFilter actionFilter = new(subName);


        // Act
        var handleResult = authHandler.IsAuthorized(httpContext, subName);
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
    public async Task TestShouldUnauthorized_WhenInitialized()
    {

        // Arrange
        var subName = "TestSubPlan123426";
        var httpContext = ContextProvider.GetHttpContext([], true);


        var authHandler = new ValidDeviceAuthenticator.AuthHandler();

        var resourceExecutingContext = ContextProvider.GetResourceExecutingContext(httpContext);
        var resourceFilterNext = new Mock<ResourceExecutionDelegate>();
        ValidDeviceAuthenticator.ResourceFilter resourceFilter = new(subName);


        var actionContext = ContextProvider.GetActionExecutingContext(httpContext);
        ValidDeviceAuthenticator.ActionFilter actionFilter = new(subName);


        // Act
        var handleResult = authHandler.IsAuthorized(httpContext, subName);
        actionFilter.OnActionExecuting(actionContext);
        await resourceFilter.OnResourceExecutionAsync(resourceExecutingContext, resourceFilterNext.Object);
        // Assert
        handleResult.ShouldBeFalse();
        resourceExecutingContext.Result.ShouldBeOfType<ForbidResult>();
        actionContext.Result.ShouldBeOfType<ForbidResult>();
        _output.WriteLine("Result: " + resourceExecutingContext.Result.ToString());

    }

    //------------------------------//

}//Cls
