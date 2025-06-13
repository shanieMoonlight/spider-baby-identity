using ClArch.ValueObjects;
using FluentAssertions;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.SubscriptionPlans;

public class SubscriptionPlanGenCrudRepoTests : RepoTestBase, IAsyncLifetime
{
    private SubscriptionPlanRepo _repo = null!;

    private readonly SubscriptionPlan _testPlan = SubscriptionPlanDataFactory.Create(
        name: "Test Subscription Plan",
        description: "A test subscription plan for CRUD operations",
        price: 29.99,
        renewalType: SubscriptionRenewalTypes.Monthly,
        deviceLimit: 5,
        trialMonths: 1);

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
        await DbContext.SubscriptionPlans.AddAsync(_testPlan);
        await SaveAndDetachAllAsync();
    }

    //-----------------------------//

    #region Basic CRUD Tests

    [Fact]
    public async Task AddAsync_ShouldAddSubscriptionPlan()
    {
        // Arrange
        var newPlan = SubscriptionPlanDataFactory.Create(name: "New Plan");

        // Act
        var result = await _repo.AddAsync(newPlan);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(newPlan.Id);
        result.Name.ShouldBe("New Plan");
    }

    //-----------------------//

    [Fact]
    public async Task FirstOrDefaultByIdAsync_ShouldReturnCorrectSubscriptionPlan()
    {
        // Act
        var result = await _repo.FirstOrDefaultByIdAsync(_testPlan.Id);

        // Assert
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(_testPlan.Id);
        result.Name.ShouldBe(_testPlan.Name);
        result.Description.ShouldBe(_testPlan.Description);
    }

    //-----------------------//

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

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSubscriptionPlan()
    {
        // Arrange
        var plan = await _repo.FirstOrDefaultByIdAsync(_testPlan.Id);
        plan.ShouldNotBeNull();

        var originalName = plan!.Name;
        var newName = "Updated Test Plan";
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
        result.Name.Should().NotBe(originalName);
    }

    //-----------------------//

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

    //-----------------------//

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrow()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act & Assert
        await _repo.Invoking(r => r.DeleteAsync(invalidId))
            .Should().NotThrowAsync();
    }

    //-----------------------//

    [Fact]
    public async Task ListAllAsync_ShouldReturnAllSubscriptionPlans()
    {
        // Arrange
        var additionalPlan = SubscriptionPlanDataFactory.Create(name: "Additional Plan");
        await _repo.AddAsync(additionalPlan);
        await SaveAndDetachAllAsync();

        // Act
        var result = await _repo.ListAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);
        result.ShouldContain(p => p.Id == _testPlan.Id);
        result.ShouldContain(p => p.Id == additionalPlan.Id);
    }

    #endregion

    #region Property Validation Tests

    [Fact]
    public async Task AddAsync_WithValidProperties_ShouldPersistCorrectly()
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create(
            name: "Property Test Plan",
            description: "Testing property persistence",
            price: 49.99,
            renewalType: SubscriptionRenewalTypes.Annual,
            deviceLimit: 20,
            trialMonths: 6);

        // Act
        var result = await _repo.AddAsync(plan);
        await SaveAndDetachAllAsync();

        // Retrieve from database to verify persistence
        var retrievedPlan = await _repo.FirstOrDefaultByIdAsync(result.Id);

        // Assert
        retrievedPlan.ShouldNotBeNull();
        retrievedPlan!.Name.ShouldBe("Property Test Plan");
        retrievedPlan.Description.ShouldBe("Testing property persistence");
        retrievedPlan.Price.ShouldBe(49.99);
        retrievedPlan.RenewalType.ShouldBe(SubscriptionRenewalTypes.Annual);
        retrievedPlan.DeviceLimit.ShouldBe(20);
        retrievedPlan.TrialMonths.ShouldBe(6);
    }

    //-----------------------//

    [Theory]
    [InlineData(SubscriptionRenewalTypes.Daily)]
    [InlineData(SubscriptionRenewalTypes.Weekly)]
    [InlineData(SubscriptionRenewalTypes.Monthly)]
    [InlineData(SubscriptionRenewalTypes.Quarterly)]
    [InlineData(SubscriptionRenewalTypes.Biannual)]
    [InlineData(SubscriptionRenewalTypes.Annual)]
    [InlineData(SubscriptionRenewalTypes.Lifetime)]
    public async Task AddAsync_WithDifferentRenewalTypes_ShouldPersistCorrectly(SubscriptionRenewalTypes renewalType)
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create(
            name: $"Plan {renewalType}",
            renewalType: renewalType);

        // Act
        var result = await _repo.AddAsync(plan);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.RenewalType.ShouldBe(renewalType);
    }

    //-----------------------//

    [Theory]
    [InlineData(0)] // Unlimited
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task AddAsync_WithDifferentDeviceLimits_ShouldPersistCorrectly(int deviceLimit)
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create(
            name: $"Plan {deviceLimit} devices",
            deviceLimit: deviceLimit);

        // Act
        var result = await _repo.AddAsync(plan);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.DeviceLimit.ShouldBe(deviceLimit);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task AddAsync_WithNullEntity_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await _repo.Invoking(r => r.AddAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    //-----------------------//

    [Fact]
    public async Task UpdateAsync_WithNullEntity_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await _repo.Invoking(r => r.UpdateAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion

    #region Collection Operations Tests

    [Fact]
    public async Task AddRangeAsync_ShouldAddMultipleSubscriptionPlans()
    {
        // Arrange
        var plans = new List<SubscriptionPlan>
        {
            SubscriptionPlanDataFactory.Create(name: "Batch Plan 1"),
            SubscriptionPlanDataFactory.Create(name: "Batch Plan 2"),
            SubscriptionPlanDataFactory.Create(name: "Batch Plan 3")
        };

        // Act
        await _repo.AddRangeAsync(plans);
        await SaveAndDetachAllAsync();

        // Assert
        var allPlans = await _repo.ListAllAsync();
        allPlans.ShouldContain(p => p.Name == "Batch Plan 1");
        allPlans.ShouldContain(p => p.Name == "Batch Plan 2");
        allPlans.ShouldContain(p => p.Name == "Batch Plan 3");
    }

    //-----------------------//

    [Fact]
    public async Task ListByIdsAsync_ShouldReturnCorrectSubscriptionPlans()
    {
        // Arrange
        var plan1 = SubscriptionPlanDataFactory.Create(name: "Plan 1");
        var plan2 = SubscriptionPlanDataFactory.Create(name: "Plan 2");
        var plan3 = SubscriptionPlanDataFactory.Create(name: "Plan 3");

        await _repo.AddRangeAsync([plan1, plan2, plan3]);
        await SaveAndDetachAllAsync();

        var idsToRetrieve = new[] { plan1.Id, plan3.Id };

        // Act
        var result = await _repo.ListByIdsAsync(idsToRetrieve);

        // Assert
        result.Should().HaveCount(2);
        result.ShouldContain(p => p.Id == plan1.Id);
        result.ShouldContain(p => p.Id == plan3.Id);
        result.Should().NotContain(p => p.Id == plan2.Id);
    }

    #endregion
}
