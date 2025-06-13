using ID.Application.Features.SubscriptionPlans;
using ID.Tests.Data.Factories;
using ID.Tests.Data.Factories.Dtos;
using Moq;
using Shouldly;
using ID.Application.Features.FeatureFlags;
using ID.Application.Features.SubscriptionPlans.Cmd.Create;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Abstractions.Services.SubPlans;

namespace ID.Application.Tests.Features.SubscriptionPlans.Cmd.Create;

public class CreateSubscriptionPlanCmdHandlerTests
{
    private readonly Mock<IIdentitySubscriptionPlanService> _serviceMock;
    private readonly CreateSubscriptionPlanCmdHandler _handler;

    public CreateSubscriptionPlanCmdHandlerTests()
    {
        _serviceMock = new Mock<IIdentitySubscriptionPlanService>();
        _handler = new CreateSubscriptionPlanCmdHandler(_serviceMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_CreateSubscriptionPlan_When_FeatureFlagsIsEmpty_And_FeatureFlagIdsIsNot()
    {
        // Arrange
        var dto = SubscriptionPlanDtoDataFactory.Create(
            featureFlags: [],
            featureFlagIds: [Guid.NewGuid()]
        );

        var request = new CreateSubscriptionPlanCmd(dto);
        var cancellationToken = CancellationToken.None;
        var expectedPlan = SubscriptionPlanDataFactory.Create();
        _serviceMock.Setup(s => s.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<IEnumerable<Guid>>(), cancellationToken))
                    .ReturnsAsync(expectedPlan);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(expectedPlan.Id);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_CreateSubscriptionPlan_When_FeatureFlagIdsIsEmpty_And_FeatureFlagsIsNot()
    {
        // Arrange
        List<FeatureFlagDto> featureFlagDtos = [new FeatureFlagDto()];
        var dto = SubscriptionPlanDtoDataFactory.Create(
            featureFlags: featureFlagDtos,
            featureFlagIds: []
        );
        var request = new CreateSubscriptionPlanCmd(dto);
        var cancellationToken = CancellationToken.None;
        var expectedPlan = SubscriptionPlanDataFactory.Create();
        _serviceMock.Setup(s => s.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<IEnumerable<Guid>>(), cancellationToken))
                    .ReturnsAsync(expectedPlan);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(expectedPlan.Id);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_CreateSubscriptionPlan_When_BothFeatureFlagsAndFeatureFlagIdsAreEmpty()
    {
        // Arrange
        var dto = SubscriptionPlanDtoDataFactory.Create(
            featureFlags: [],
            featureFlagIds: []
        );

        var request = new CreateSubscriptionPlanCmd(dto);
        var cancellationToken = CancellationToken.None;
        var expectedPlan = SubscriptionPlanDataFactory.Create();
        _serviceMock.Setup(s => s.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<IEnumerable<Guid>>(), cancellationToken))
                    .ReturnsAsync(expectedPlan);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(expectedPlan.Id);
    }

    //------------------------------------//

}//Cls
