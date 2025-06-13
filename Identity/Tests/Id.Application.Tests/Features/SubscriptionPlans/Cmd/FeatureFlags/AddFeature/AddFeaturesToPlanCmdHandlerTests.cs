using ID.Application.Features.SubscriptionPlans;
using ID.Application.Features.SubscriptionPlans.Cmd.FeatureFlags.AddFeature;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.SubscriptionPlans.Cmd.FeatureFlags.AddFeature;

public class AddFeaturesToPlanCmdHandlerTests
{
    private readonly Mock<IIdentitySubscriptionPlanService> _serviceMock;
    private readonly AddFeatureToSubscriptionPlanCmdHandler _handler;

    public AddFeaturesToPlanCmdHandlerTests()
    {
        _serviceMock = new Mock<IIdentitySubscriptionPlanService>();
        _handler = new AddFeatureToSubscriptionPlanCmdHandler(_serviceMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenPlanDoesNotExist()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var featureIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        var dto = new AddFeaturesToPlanDto(planId, featureIds);
        var request = new AddFeatureToSubscriptionPlanCmd(dto);

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _serviceMock.Setup(s => s.GetByIdWithFeatureFlagsAsync(planId))
            .ReturnsAsync((SubscriptionPlan)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<SubscriptionPlanDto>>();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenFeaturesAddedSuccessfully()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var featureIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        var dto = new AddFeaturesToPlanDto(planId, featureIds);
        var request = new AddFeatureToSubscriptionPlanCmd(dto);

        var subscriptionPlan = SubscriptionPlanDataFactory.Create();
        var updatedPlan = SubscriptionPlanDataFactory.Create(); 

        _serviceMock.Setup(s => s.GetByIdWithFeatureFlagsAsync(planId))
            .ReturnsAsync(subscriptionPlan);

        _serviceMock.Setup(s => s.AddFeaturesToPlanAsync(subscriptionPlan, featureIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedPlan);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<SubscriptionPlanDto>>();
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls