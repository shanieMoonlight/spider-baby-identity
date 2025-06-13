using ID.Domain.Abstractions.Services.SubPlans;
using ID.Infrastructure.DomainServices.SubPlans;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using Moq;

namespace ID.Infrastructure.Tests.DmnSevcs.SubPlans;

public class IdentitySubscriptionPlanService_Add0_Tests
{
    private readonly Mock<IIdUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IIdentitySubscriptionPlanRepo> _mockSubscriptionPlanRepo;
    private readonly Mock<IIdentityFeatureFlagRepo> _mockFeatureFlagRepo;
    private readonly IIdentitySubscriptionPlanService _service;

    public IdentitySubscriptionPlanService_Add0_Tests()
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
        var featureFlagIds = featureFlags.Select(ff => ff.Id);

        _mockSubscriptionPlanRepo.Setup(repo => repo.AddAsync(plan, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);
        _mockFeatureFlagRepo.Setup(repo => repo.ListByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(featureFlags);

        // Act
        var result = await _service.AddAsync(plan, featureFlagIds);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(plan);
        result.FeatureFlags.ShouldBe(featureFlags);
        _mockSubscriptionPlanRepo.Verify(repo => repo.AddAsync(plan, It.IsAny<CancellationToken>()), Times.Once);
        _mockFeatureFlagRepo.Verify(repo => repo.ListByIdsAsync(It.IsAny<IEnumerable<Guid>>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    //------------------------------------//

    [Fact]
    public async Task AddAsync_ShouldCallGetRangeByIdsAsyncWithoutDuplicates_WhenDuplicateFeatureFlagIdsArePassed()
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create();
        var featureFlagIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        featureFlagIds.Add(featureFlagIds[0]); // Add duplicate
        featureFlagIds.Add(featureFlagIds[1]); // Add duplicate

        _mockSubscriptionPlanRepo.Setup(repo => repo.AddAsync(plan, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);
        _mockFeatureFlagRepo.Setup(repo => repo.ListByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync([]);

        // Act
        await _service.AddAsync(plan, featureFlagIds, CancellationToken.None);

        // Assert
        _mockFeatureFlagRepo.Verify(repo => repo.ListByIdsAsync(It.Is<IEnumerable<Guid>>(ids => ids.Distinct().Count() == ids.Count())), Times.Once);
    }

    //------------------------------------//

}//Cls