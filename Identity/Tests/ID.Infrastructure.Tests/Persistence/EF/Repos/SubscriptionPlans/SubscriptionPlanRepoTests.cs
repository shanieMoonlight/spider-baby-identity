using ClArch.ValueObjects;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.SubscriptionPlans;

public class SubscriptionPlanRepoTests : RepoTestBase, IAsyncLifetime
{
    private SubscriptionPlanRepo _repo = null!;

    private readonly SubscriptionPlan _basicPlan = SubscriptionPlanDataFactory.Create(
        name: "Basic Plan",
        price: 9.99,
        renewalType: SubscriptionRenewalTypes.Monthly,
        deviceLimit: 5,
        trialMonths: 1);

    private readonly SubscriptionPlan _premiumPlan = SubscriptionPlanDataFactory.Create(
        name: "Premium Plan",
        price: 19.99,
        renewalType: SubscriptionRenewalTypes.Monthly,
        deviceLimit: 10,
        trialMonths: 2);

    private readonly SubscriptionPlan _enterprisePlan = SubscriptionPlanDataFactory.Create(
        name: "Enterprise Plan",
        price: 99.99,
        renewalType: SubscriptionRenewalTypes.Annual,
        deviceLimit: 0, // Unlimited
        trialMonths: 3);

    private readonly List<SubscriptionPlan> _subscriptionPlans;

    //-----------------------------//

    public SubscriptionPlanRepoTests()
    {
        _subscriptionPlans = [_basicPlan, _premiumPlan, _enterprisePlan];
    }

    //-----------------------------//

    public async Task InitializeAsync()
    {
        _repo = CreateRepository<SubscriptionPlanRepo>();
        await SeedDatabaseAsync();
    }

    //-----------------------------//

    public Task DisposeAsync() => Task.CompletedTask;

    //-----------------------------//

    protected override async Task SeedDatabaseAsync()
    {
        await DbContext.SubscriptionPlans.AddRangeAsync(_subscriptionPlans);
        await SaveAndDetachAllAsync();
    }

    //-----------------------------//

    #region CRUD Operations Tests

    [Fact]
    public async Task AddAsync_ShouldAddSubscriptionPlan()
    {
        // Arrange
        var newPlan = SubscriptionPlanDataFactory.Create(name: "Test Plan");

        // Act
        var result = await _repo.AddAsync(newPlan);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(newPlan.Id);
        result.Name.ShouldBe("Test Plan");
    }

    //-----------------------------//

    [Fact]
    public async Task FirstOrDefaultByIdAsync_ShouldReturnCorrectSubscriptionPlan()
    {
        // Act
        var result = await _repo.FirstOrDefaultByIdAsync(_basicPlan.Id);

        // Assert
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(_basicPlan.Id);
        result.Name.ShouldBe(_basicPlan.Name);
    }

    //-----------------------------//

    [Fact]
    public async Task FirstOrDefaultByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _repo.FirstOrDefaultByIdAsync(invalidId);

        // Assert
        result.ShouldBeNull();
    }

    //-----------------------------//

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSubscriptionPlan()
    {
        // Arrange
        var plan = await _repo.FirstOrDefaultByIdAsync(_basicPlan.Id);
        plan.ShouldNotBeNull();

        var originalName = plan!.Name;
        var newName = "Updated Basic Plan";
        plan.Update(
            Name.Create(newName),
            Description.Create(plan.Description),
            Price.Create(plan.Price),
            plan.RenewalType,
            null,
            null);

        // Act
        var result = await _repo.UpdateAsync(plan);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(newName);
        result.Name.ShouldNotBe(originalName);
    }

    //-----------------------------//

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSubscriptionPlan()
    {
        // Arrange
        var planToDelete = SubscriptionPlanDataFactory.Create();
        await _repo.AddAsync(planToDelete);
        await SaveAndDetachAllAsync();

        // Act
        await _repo.DeleteAsync(planToDelete.Id);
        await SaveAndDetachAllAsync();

        // Assert
        var deletedPlan = await _repo.FirstOrDefaultByIdAsync(planToDelete.Id);
        deletedPlan.ShouldBeNull();
    }

    //-----------------------------//

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrow()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act & Assert
        var deleteAction = async () => await _repo.DeleteAsync(invalidId);
        await deleteAction.ShouldNotThrowAsync();
    }

    //-----------------------------//

    [Fact]
    public async Task ListAllAsync_ShouldReturnAllSubscriptionPlans()
    {
        // Act
        var result = await _repo.ListAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(_subscriptionPlans.Count);
        result.ShouldContain(p => p.Name == _basicPlan.Name);
        result.ShouldContain(p => p.Name == _premiumPlan.Name);
        result.ShouldContain(p => p.Name == _enterprisePlan.Name);
    }

    #endregion

    #region Business Rule Tests

    [Fact]
    public async Task CanDeleteAsync_WithoutSubscriptions_ShouldAllowDeletion()
    {
        // Arrange
        var planWithoutSubscriptions = SubscriptionPlanDataFactory.Create();
        await _repo.AddAsync(planWithoutSubscriptions);
        await SaveAndDetachAllAsync();

        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(SubscriptionPlanRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [planWithoutSubscriptions])!;

        // Assert
        result.Succeeded.ShouldBeTrue();
    }

    //-----------------------------//

    [Fact]
    public async Task CanDeleteAsync_WithNullPlan_ShouldAllowDeletion()
    {
        // Act
        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(SubscriptionPlanRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [null])!;

        // Assert
        result.Succeeded.ShouldBeTrue();
    }

    //-----------------------------//

    [Theory]
    [InlineData(5, 9.99)]
    [InlineData(10, 19.99)]
    [InlineData(0, 99.99)]
    public async Task AddAsync_WithValidPlanData_ShouldCreatePlanWithCorrectProperties(int deviceLimit, double price)
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create(
            deviceLimit: deviceLimit,
            price: price);

        // Act
        var result = await _repo.AddAsync(plan);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.DeviceLimit.ShouldBe(deviceLimit);
        result.Price.ShouldBe(price);
    }

    #endregion

    #region Specification Tests

    [Fact]
    public async Task ListAllAsync_WithGetSubscriptionPlanByNameSpec_ShouldReturnMatchingPlan()
    {
        // Arrange
        var spec = new GetSubscriptionPlanByNameSpec(_basicPlan.Name);

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe(_basicPlan.Name);
    }

    [Fact]
    public async Task ListAllAsync_WithGetSubscriptionPlanByNameContainsSpec_ShouldReturnMatchingPlans()
    {
        // Arrange
        var spec = new GetSubscriptionPlanByNameContainsSpec("Plan");

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.Count.ShouldBe(3); // All plans contain "Plan"
        result.ShouldContain(p => p.Name == _basicPlan.Name);
        result.ShouldContain(p => p.Name == _premiumPlan.Name);
        result.ShouldContain(p => p.Name == _enterprisePlan.Name);
    }

    [Fact]
    public async Task ListAllAsync_WithGetSubscriptionPlansByDeviceLimitSpec_ShouldReturnMatchingPlans()
    {
        // Arrange
        var spec = new GetSubscriptionPlansByDeviceLimitSpec(5);

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe(_basicPlan.Name);
        result[0].DeviceLimit.ShouldBe(5);
    }

    [Fact]
    public async Task ListAllAsync_WithGetSubscriptionPlansByRenewalTypeSpec_ShouldReturnMatchingPlans()
    {
        // Arrange
        var spec = new GetSubscriptionPlansByRenewalTypeSpec(SubscriptionRenewalTypes.Monthly);

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.Count.ShouldBe(2); // Basic and Premium are monthly
        result.ShouldContain(p => p.Name == _basicPlan.Name);
        result.ShouldContain(p => p.Name == _premiumPlan.Name);
        result.ShouldNotContain(p => p.Name == _enterprisePlan.Name);
    }

    [Fact]
    public async Task ListAllAsync_WithGetSubscriptionPlansByPriceRangeSpec_ShouldReturnMatchingPlans()
    {
        // Arrange
        var spec = new GetSubscriptionPlansByPriceRangeSpec(10.0, 50.0);

        // Act
        var result = await _repo.ListAllAsync(spec);

        // Assert
        result.Count.ShouldBe(1); // Only Premium plan (19.99) is in this range
        result[0].Name.ShouldBe(_premiumPlan.Name);
        result[0].Price.ShouldBe(19.99);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithGetSubscriptionPlanByNameSpec_ShouldReturnCorrectPlan()
    {
        // Arrange
        var spec = new GetSubscriptionPlanByNameSpec(_enterprisePlan.Name);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result!.Name.ShouldBe(_enterprisePlan.Name);
        result.Price.ShouldBe(99.99);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithNonMatchingSpec_ShouldReturnNull()
    {
        // Arrange
        var spec = new GetSubscriptionPlanByNameSpec("Non-existent Plan");

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldBeNull();
    }

    #endregion

    #region Complex Specification Tests

    [Fact]
    public async Task ListAllAsync_WithMultipleFilters_ShouldReturnCorrectResults()
    {
        // Arrange - Create plans with specific characteristics for testing
        var monthlyLowPrice = SubscriptionPlanDataFactory.Create(
            name: "Monthly Low",
            price: 5.0,
            renewalType: SubscriptionRenewalTypes.Monthly,
            deviceLimit: 3);

        var monthlyHighPrice = SubscriptionPlanDataFactory.Create(
            name: "Monthly High",
            price: 50.0,
            renewalType: SubscriptionRenewalTypes.Monthly,
            deviceLimit: 15);

        await _repo.AddAsync(monthlyLowPrice);
        await _repo.AddAsync(monthlyHighPrice);
        await SaveAndDetachAllAsync();

        // Act - Test price range filtering
        var priceRangeSpec = new GetSubscriptionPlansByPriceRangeSpec(0.0, 20.0);
        var priceRangeResults = await _repo.ListAllAsync(priceRangeSpec);

        // Assert
        priceRangeResults.Count.ShouldBe(3); // Basic (9.99), Premium (19.99), Monthly Low (5.0)
        priceRangeResults.ShouldContain(p => p.Name == _basicPlan.Name);
        priceRangeResults.ShouldContain(p => p.Name == _premiumPlan.Name);
        priceRangeResults.ShouldContain(p => p.Name == "Monthly Low");
        priceRangeResults.ShouldNotContain(p => p.Name == _enterprisePlan.Name);
        priceRangeResults.ShouldNotContain(p => p.Name == "Monthly High");
    }

    #endregion

    #region Pagination Tests

    [Fact]
    public async Task PageAsync_WithValidPageRequest_ShouldReturnCorrectPage()
    {
        // Arrange
        var pageRequest = new PagedRequest { PageNumber = 1, PageSize = 2 };

        // Act
        var result = await _repo.PageAsync(pageRequest);

        // Assert
        result.ShouldNotBeNull();
        result.Data.Count.ShouldBe(2);
        result.TotalItems.ShouldBe(_subscriptionPlans.Count);
        result.Number.ShouldBe(1);
        result.Size.ShouldBe(2);
        result.TotalPages.ShouldBe(2); // 3 items with page size 2 = 2 pages
    }

    //-----------------------//

    [Fact]
    public async Task PageAsync_WithLargePage_ShouldReturnAllItems()
    {
        // Arrange
        var pageRequest = new PagedRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _repo.PageAsync(pageRequest);

        // Assert
        result.ShouldNotBeNull();
        result.Data.Count.ShouldBe(_subscriptionPlans.Count);
        result.TotalItems.ShouldBe(_subscriptionPlans.Count);
        result.Number.ShouldBe(1);
        result.Size.ShouldBe(10);
        result.TotalPages.ShouldBe(1);
    }

    //-----------------------//

    [Fact]
    public async Task PageAsync_WithSecondPage_ShouldReturnRemainingItems()
    {
        // Arrange
        var pageRequest = new PagedRequest { PageNumber = 2, PageSize = 2 };

        // Act
        var result = await _repo.PageAsync(pageRequest);

        // Assert
        result.ShouldNotBeNull();
        result.Data.Count.ShouldBe(1); // Only 1 item left on page 2
        result.TotalItems.ShouldBe(_subscriptionPlans.Count);
        result.Number.ShouldBe(2);
        result.Size.ShouldBe(2);
    }

    [Fact]
    public async Task PageAsync_WithEmptyPage_ShouldReturnEmptyResult()
    {
        // Arrange
        var pageRequest = new PagedRequest { PageNumber = 5, PageSize = 2 };

        // Act
        var result = await _repo.PageAsync(pageRequest);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldBeEmpty();
        result.TotalItems.ShouldBe(_subscriptionPlans.Count);
        result.Number.ShouldBe(5);
        result.Size.ShouldBe(2);
    }

    #endregion

    #region Feature Flags Relationship Tests

    [Fact]
    public async Task AddAsync_WithFeatureFlags_ShouldMaintainRelationship()
    {
        // Arrange
        var featureFlags = new List<FeatureFlag>
        {
            FeatureFlagDataFactory.Create(name: "Test Feature 1"),
            FeatureFlagDataFactory.Create(name: "Test Feature 2")
        };

        await DbContext.FeatureFlags.AddRangeAsync(featureFlags);
        await SaveAndDetachAllAsync();

        var plan = SubscriptionPlanDataFactory.Create(
            name: "Plan with Features");

        // Act
        var result = await _repo.AddAsync(plan);
        await SaveAndDetachAllAsync();

        result.AddFeatureFlags(featureFlags);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.FeatureFlags.Count.ShouldBe(2);
        result.FeatureFlags.ShouldContain(f => f.Name == "Test Feature 1");
        result.FeatureFlags.ShouldContain(f => f.Name == "Test Feature 2");
    }


    #endregion

    #region Edge Case Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task PageAsync_WithInvalidPageSize_ShouldHandleGracefully(int pageSize)
    {
        // Arrange
        var pageRequest = new PagedRequest { PageNumber = 1, PageSize = pageSize };

        // Act & Assert - This should either throw or handle gracefully based on your implementation
        var result = await _repo.PageAsync(pageRequest);
        result.ShouldNotBeNull();
    }

    #endregion

    #region Consistency Tests

    [Fact]
    public async Task ConcurrentOperations_ShouldMaintainDataIntegrity()
    {
        // Arrange
        var plan1 = SubscriptionPlanDataFactory.Create(name: "Concurrent Plan 1");
        var plan2 = SubscriptionPlanDataFactory.Create(name: "Concurrent Plan 2");

        // Act - Simulate concurrent additions
        var task1 = _repo.AddAsync(plan1);
        var task2 = _repo.AddAsync(plan2);

        await Task.WhenAll(task1, task2);
        await SaveAndDetachAllAsync();

        // Assert
        var allPlans = await _repo.ListAllAsync();
        allPlans.ShouldContain(p => p.Name == "Concurrent Plan 1");
        allPlans.ShouldContain(p => p.Name == "Concurrent Plan 2");
    }

    //-----------------------//

    [Fact]
    public async Task UpdateAfterDelete_ShouldNotAffectDeletedEntity()
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create(name: "Plan to Delete");
        await _repo.AddAsync(plan);
        await SaveAndDetachAllAsync();

        // Act
        await _repo.DeleteAsync(plan.Id);
        await SaveAndDetachAllAsync();

        // Attempt to update deleted entity
        plan.Update(
            Name.Create("Updated Name"),
            Description.Create(plan.Description),
            Price.Create(plan.Price),
            plan.RenewalType,
            null,
            null);

        // Assert - The update should not affect the database since the entity is deleted
        var deletedPlan = await _repo.FirstOrDefaultByIdAsync(plan.Id);
        deletedPlan.ShouldBeNull();
    }

    #endregion
}
