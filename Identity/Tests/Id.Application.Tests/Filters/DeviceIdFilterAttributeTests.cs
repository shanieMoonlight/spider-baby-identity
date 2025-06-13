using ID.Application.Authenticators;
using ID.Tests.Data.Factories;
using ID.Tests.Data.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Shouldly;

namespace ID.Application.Tests.Filters;

public class DeviceIdFilterAttributeTests
{

    [Fact]
    public async Task OnActionExecuting_DeviceIdIsNullOrWhiteSpace_ShouldReturnForbidResultAsync()
    {
        // Arrange
        var subscriptionName = "testSubscription";
        var subClaimData =SubscriptionClaimDataFactory.Create(name: subscriptionName, deviceId: string.Empty);
        var filter = new ValidDeviceAuthenticator.ResourceFilter(subscriptionName);
        var context = ContextProvider.GetActionContextWithDeviceIdClaim([subClaimData]);
        var next = new Mock<ResourceExecutionDelegate>();

        // Act
        await filter.OnResourceExecutionAsync(context, next.Object);

        // Assert
        context.Result.ShouldBeOfType<ForbidResult>();
        next.Verify(n => n(), Times.Never);
    }

    //-------------------------------------//

    [Fact]
    public async Task OnActionExecuting_DeviceIdIsNotNullOrWhiteSpace_ShouldCallBaseOnActionExecutingAsync()
    {
        // Arrange
        var subscriptionName = "testSubscription";
        var deviceId = "validDevcId";
        var subClaimData = SubscriptionClaimDataFactory.Create(name: subscriptionName, deviceId: deviceId);
        var filter = new ValidDeviceAuthenticator.ResourceFilter(subscriptionName);
        var context = ContextProvider.GetActionContextWithDeviceIdClaim([subClaimData]);
        var next = new Mock<ResourceExecutionDelegate>();

        // Act
        await filter.OnResourceExecutionAsync(context, next.Object);

        // Assert
        context.Result.ShouldBeNull();
    }

    //-------------------------------------//

}