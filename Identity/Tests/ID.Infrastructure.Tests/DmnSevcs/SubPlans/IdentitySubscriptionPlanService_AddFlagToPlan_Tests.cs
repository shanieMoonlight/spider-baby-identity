using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Infrastructure.DomainServices.SubPlans;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.Infrastructure.Tests.DmnSevcs.SubPlans;

public class IdentitySubscriptionPlanService_AddFlagToPlan_Tests
{
    private readonly Mock<IIdentitySubscriptionPlanRepo> _repoMock;
    private readonly Mock<IIdentityFeatureFlagRepo> _featuresRepoMock;
    private readonly Mock<IIdUnitOfWork> _uowMock;
    private readonly IdentitySubscriptionPlanService _service;

    public IdentitySubscriptionPlanService_AddFlagToPlan_Tests()
    {
        _repoMock = new Mock<IIdentitySubscriptionPlanRepo>();
        _featuresRepoMock = new Mock<IIdentityFeatureFlagRepo>();
        _uowMock = new Mock<IIdUnitOfWork>();

        _uowMock.Setup(u => u.SubscriptionPlanRepo).Returns(_repoMock.Object);
        _uowMock.Setup(u => u.FeatureFlagRepo).Returns(_featuresRepoMock.Object);

        _service = new IdentitySubscriptionPlanService(_uowMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task AddFeaturesToPlanAsync_ShouldNotAddDuplicateFeatureFlags()
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create();
        var featureFlags = FeatureFlagDataFactory.CreateMany(5);
        var duplicateFeatureFlags = new List<FeatureFlag>(featureFlags);
        duplicateFeatureFlags.AddRange(featureFlags); // Add duplicates

        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<SubscriptionPlan>())).ReturnsAsync(plan);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddFeaturesToPlanAsync(plan, duplicateFeatureFlags);

        // Assert
        result.ShouldNotBeNull();
        result.FeatureFlags.Count.ShouldBe(featureFlags.Count); // Ensure no duplicates
        _repoMock.Verify(r => r.UpdateAsync(It.Is<SubscriptionPlan>(p => p.FeatureFlags.Count == featureFlags.Count)), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); // Verify SaveChangesAsync was called once
    }

    //------------------------------------//

}//Cls