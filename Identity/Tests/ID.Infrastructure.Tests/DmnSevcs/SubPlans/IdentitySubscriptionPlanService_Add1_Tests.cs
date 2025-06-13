using ID.Domain.Abstractions.Services.SubPlans;
using ID.Infrastructure.DomainServices.SubPlans;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using Moq;

namespace ID.Infrastructure.Tests.DmnSevcs.SubPlans;

public class IdentitySubscriptionPlanService_Add1_Tests
{
    private readonly Mock<IIdUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IIdentitySubscriptionPlanRepo> _mockSubscriptionPlanRepo;
    private readonly Mock<IIdentityFeatureFlagRepo> _mockFeatureFlagRepo;
    private readonly IIdentitySubscriptionPlanService _service;

    public IdentitySubscriptionPlanService_Add1_Tests()
    {
        _mockUnitOfWork = new Mock<IIdUnitOfWork>();
        _mockSubscriptionPlanRepo = new Mock<IIdentitySubscriptionPlanRepo>();
        _mockFeatureFlagRepo = new Mock<IIdentityFeatureFlagRepo>();

        _mockUnitOfWork.Setup(uow => uow.SubscriptionPlanRepo).Returns(_mockSubscriptionPlanRepo.Object);
        _mockUnitOfWork.Setup(uow => uow.FeatureFlagRepo).Returns(_mockFeatureFlagRepo.Object);

        _service = new IdentitySubscriptionPlanService(_mockUnitOfWork.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task AddAsync_ShouldAddSubscriptionPlanAndFeatureFlags()
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create();
        var featureFlags = FeatureFlagDataFactory.CreateMany(3);

        _mockSubscriptionPlanRepo.Setup(repo => repo.AddAsync(plan, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);
        _mockFeatureFlagRepo.Setup(repo => repo.ListByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(featureFlags);

        // Act
        var result = await _service.AddAsync(plan, featureFlags, default);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(plan);
        result.FeatureFlags.ShouldBe(featureFlags);
        _mockSubscriptionPlanRepo.Verify(repo => repo.AddAsync(plan, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    //------------------------------------//

}//Cls