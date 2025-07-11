using ID.Infrastructure.DomainServices.SubPlans;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.Infrastructure.Tests.DmnSevcs.SubPlans;

public class IdentitySubscriptionPlanService_RemoveFlagIdsFromPlan_Tests
{
    private readonly Mock<IIdentitySubscriptionPlanRepo> _subscriptionPlanRepoMock;
    private readonly Mock<IIdentityFeatureFlagRepo> _featureFlagRepoMock;
    private readonly Mock<IIdUnitOfWork> _unitOfWorkMock;
    private readonly IdentitySubscriptionPlanService _service;

    public IdentitySubscriptionPlanService_RemoveFlagIdsFromPlan_Tests()
    {
        _subscriptionPlanRepoMock = new Mock<IIdentitySubscriptionPlanRepo>();
        _featureFlagRepoMock = new Mock<IIdentityFeatureFlagRepo>();
        _unitOfWorkMock = new Mock<IIdUnitOfWork>();

        _unitOfWorkMock.Setup(uow => uow.SubscriptionPlanRepo).Returns(_subscriptionPlanRepoMock.Object);
        _unitOfWorkMock.Setup(uow => uow.FeatureFlagRepo).Returns(_featureFlagRepoMock.Object);

        _service = new IdentitySubscriptionPlanService(_unitOfWorkMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task RemoveFeaturesFromPlanAsync_Should_Remove_FeatureFlags_By_Ids()
    {
        // Arrange
        var featureFlags = FeatureFlagDataFactory.CreateMany(3);
        var plan = SubscriptionPlanDataFactory.Create(featureFlags: featureFlags);
        var featureFlagIds = featureFlags.Select(ff => ff.Id).ToList();

        _featureFlagRepoMock.Setup(repo => repo.ListByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(featureFlags);

        //Make sure the plan has feature flags
        plan.FeatureFlags.ShouldNotBeEmpty();

        // Act
        var result = await _service.RemoveFeaturesFromPlanAsync(plan, featureFlagIds);

        // Assert
        result.ShouldNotBeNull();
        result.FeatureFlags.ShouldBeEmpty();
        _subscriptionPlanRepoMock.Verify(repo => repo.UpdateAsync(plan), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

}//Cls