using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Infrastructure.DomainServices.SubPlans;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using Moq;

namespace ID.Infrastructure.Tests.DmnSevcs.SubPlans;

public class IdentitySubscriptionPlanService_AddFlagIdsToPlan_Tests
{
    private readonly Mock<IIdentitySubscriptionPlanRepo> _repoMock;
    private readonly Mock<IIdentityFeatureFlagRepo> _featuresRepoMock;
    private readonly Mock<IIdUnitOfWork> _uowMock;
    private readonly IdentitySubscriptionPlanService _service;

    public IdentitySubscriptionPlanService_AddFlagIdsToPlan_Tests()
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

    [Fact]
    public async Task AddFeaturesToPlanAsync_WithEmptyFeatureFlags_ShouldReturnPlanUnchanged()
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create();
        var featureFlags = new List<FeatureFlag>();

        // Act
        var result = await _service.AddFeaturesToPlanAsync(plan, featureFlags);

        // Assert
        result.ShouldNotBeNull();
        result.FeatureFlags.Count.ShouldBe(0); // Ensure no feature flags were added
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<SubscriptionPlan>()), Times.Never);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never); // Verify SaveChangesAsync was not called
    }

    //------------------------------------//

    [Fact]
    public async Task AddFeaturesToPlanAsync_WithFeatureFlagIds_ShouldAddFeatureFlagsToPlan()
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create();
        var featureFlagIds = FeatureFlagDataFactory.CreateMany(5).Select(f => f.Id);
        var featureFlags = FeatureFlagDataFactory.CreateMany(5);

        _featuresRepoMock.Setup(r => r.ListByIdsAsync(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(featureFlags);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<SubscriptionPlan>())).ReturnsAsync(plan);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddFeaturesToPlanAsync(plan, featureFlagIds);

        // Assert
        result.ShouldNotBeNull();
        result.FeatureFlags.Count.ShouldBe(featureFlags.Count); // Ensure feature flags were added
        foreach (var flag in featureFlags)
        {
            result.FeatureFlags.ShouldContain(f => f.Id == flag.Id); // Ensure feature flags were added            
        }
        _repoMock.Verify(r => r.UpdateAsync(It.Is<SubscriptionPlan>(p => p.FeatureFlags.Count == featureFlags.Count)), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); // Verify SaveChangesAsync was called once
    }

    //------------------------------------//


}//Cls
