using ID.Tests.Data.Factories;
using Moq;
using Shouldly;
using ID.Application.Features.SubscriptionPlans.Cmd.FeatureFlags.RemoveFeature;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Abstractions.Services.SubPlans;

namespace ID.Application.Tests.Features.SubscriptionPlans.Cmd.FeatureFlags.RemoveFeature;

public class RemoveFeaturesFromPlanCmdHandlerTests
{
    private readonly Mock<IIdentitySubscriptionPlanService> _serviceMock;
    private readonly RemoveFeaturesFromSubscriptionPlanCmdHandler _handler;

    public RemoveFeaturesFromPlanCmdHandlerTests()
    {
        _serviceMock = new Mock<IIdentitySubscriptionPlanService>();
        _handler = new RemoveFeaturesFromSubscriptionPlanCmdHandler(_serviceMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenSubscriptionPlanDoesNotExist()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var featureIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var request = new RemoveFeaturesFromSubscriptionPlanCmd(new RemoveFeaturesFromSubscriptionPlanDto(planId, featureIds));

        _serviceMock.Setup(s => s.GetByIdWithFeatureFlagsAsync(planId))
            .ReturnsAsync((SubscriptionPlan?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<SubscriptionPlan>(planId));
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldRemoveFeaturesFromPlan_WhenSubscriptionPlanExists()
    {
        // Arrange
        var featureId1 = Guid.NewGuid();
        var featureId2 = Guid.NewGuid();
        var featureId3 = Guid.NewGuid();
        var featureIdToRemove1 = Guid.NewGuid();
        var featureIdToRemove2 = Guid.NewGuid();
        List<FeatureFlag> featureFlags = [];
        foreach (var id in new List<Guid> { featureId1, featureId2, featureId3, featureIdToRemove1, featureIdToRemove2 })
        {
            featureFlags.Add(FeatureFlagDataFactory.Create(id));
        }

        var featureIdsToRemove = new List<Guid> { featureIdToRemove1, featureIdToRemove2 };
        var plan = SubscriptionPlanDataFactory.Create(featureFlags: featureFlags);
        var request = new RemoveFeaturesFromSubscriptionPlanCmd(new RemoveFeaturesFromSubscriptionPlanDto(plan.Id, featureIdsToRemove));

        _serviceMock.Setup(s => s.GetByIdWithFeatureFlagsAsync(plan.Id))
            .ReturnsAsync(plan);

        _serviceMock.Setup(s => s.RemoveFeaturesFromPlanAsync(plan, featureIdsToRemove, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(plan.Id);

        // Verify that RemoveFeaturesFromPlanAsync was called with the correct parameters
        _serviceMock.Verify(s => s.RemoveFeaturesFromPlanAsync(plan, featureIdsToRemove, It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

}//Cls
