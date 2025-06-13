using ClArch.ValueObjects;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.FeatureFlags;

public class FeatureFlagRepoTests : RepoTestBase, IAsyncLifetime
{
    private FeatureFlagRepo _repo = null!;

    private readonly FeatureFlag _featureFlag1 = FeatureFlagDataFactory.Create(name: "Feature1");
    private readonly FeatureFlag _featureFlag2 = FeatureFlagDataFactory.Create(name: "Feature2");

    //-----------------------------//

    public async Task InitializeAsync()
    {
        _repo = CreateRepository<FeatureFlagRepo>();
        await SeedDatabaseAsync();
    }

    //-----------------------------//

    public Task DisposeAsync() => Task.CompletedTask;

    //-----------------------------//

    protected override async Task SeedDatabaseAsync()
    {
        // Add test data

        await DbContext.FeatureFlags.AddRangeAsync(_featureFlag1, _featureFlag2);
        await SaveAndDetachAllAsync();
    }

    #region CRUD Operations Tests

    [Fact]
    public async Task AddAsync_ShouldAddFeatureFlag()
    {
        // Arrange
        var newFeature = FeatureFlagDataFactory.Create(name: "NewFeature");

        // Act
        var result = await _repo.AddAsync(newFeature);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("NewFeature");

        var fromDb = await _repo.FirstOrDefaultByIdAsync(result.Id);
        fromDb.ShouldNotBeNull();
        fromDb!.Name.ShouldBe("NewFeature");
    }

    //-----------------------------//

    [Fact]
    public async Task UpdateAsync_ShouldUpdateFeatureFlag()
    {
        // Arrange
        var spec = new GetFeatureFlagByNameSpec(_featureFlag1.Name);
        var feature = await _repo.FirstOrDefaultAsync(spec);
        feature.ShouldNotBeNull();

        // Act
        feature.Update(
            Name.Create("UpdatedFeature"),
            Description.Create("Updated description"));

        await _repo.UpdateAsync(feature);
        await SaveAndDetachAllAsync();

        // Assert
        var updated = await _repo.FirstOrDefaultByIdAsync(feature.Id);
        updated.ShouldNotBeNull();
        updated!.Name.ShouldBe("UpdatedFeature");
    }

    //-----------------------------//

    [Fact]
    public async Task ListAllAsync_ShouldReturnAllFeatureFlags()
    {
        // Act
        var features = await _repo.ListAllAsync();

        // Assert
        features.Count.ShouldBe(2);
        features.ShouldContain(f => f.Name == _featureFlag1.Name);
        features.ShouldContain(f => f.Name == _featureFlag2.Name);
    }

    //-----------------------------//

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var spec = new GetFeatureFlagByNameSpec(_featureFlag1.Name);
        var feature = await _repo.FirstOrDefaultAsync(spec);

        // Act
        var exists = await _repo.ExistsAsync(feature!.Id);

        // Assert
        exists.ShouldBeTrue();
    }

    //-----------------------------//

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Act
        var exists = await _repo.ExistsAsync(Guid.NewGuid());

        // Assert
        exists.ShouldBeFalse();
    }

    #endregion

    #region Delete Tests (Business Logic)

    [Fact]
    public async Task DeleteAsync_WithNoConnectedPlans_ShouldSucceed()
    {
        // Arrange
        var spec = new GetFeatureFlagByNameSpec(_featureFlag1.Name);
        var feature = await _repo.FirstOrDefaultAsync(spec);
        feature.ShouldNotBeNull();

        // Act & Assert
        var deleteAction = async () => await _repo.DeleteAsync(feature!);
        await deleteAction.ShouldNotThrowAsync();

        await SaveAndDetachAllAsync();

        // Verify deletion
        var deleted = await _repo.FirstOrDefaultByIdAsync(feature!.Id);
        deleted.ShouldBeNull();
    }

    //-----------------------------//

    [Fact]
    public async Task DeleteAsync_WithConnectedPlans_ShouldThrowCantDeleteException()
    {
        // Arrange
        var spec = new GetFeatureFlagByNameSpec(_featureFlag1.Name);
        var feature = await _repo.FirstOrDefaultAsync(spec);

        // Create a subscription plan with this feature
        List<FeatureFlag> features = [];
        if (feature != null)
            features.Add(feature);
        var plan = SubscriptionPlanDataFactory.Create(name: "TestPlan", featureFlags: features);
        await DbContext.SubscriptionPlans.AddAsync(plan);
        await SaveAndDetachAllAsync();

        // Act & Assert
        feature.ShouldNotBeNull(); // Ensure feature is not null before using
        var deleteAction = async () => await _repo.DeleteAsync(feature);
        await deleteAction.ShouldThrowAsync<CantDeleteException>();
    }

    //-----------------------------//

    [Fact]
    public async Task CanDeleteAsync_WithConnectedPlans_ShouldReturnFailure()
    {
        // Arrange
        var spec = new GetFeatureFlagByNameSpec(_featureFlag1.Name);
        var feature = await _repo.FirstOrDefaultAsync(spec);

        // Create a subscription plan with this feature
        List<FeatureFlag> features = [];
        if (feature != null)
            features.Add(feature);
        var plan = SubscriptionPlanDataFactory.Create(name: "TestPlan", featureFlags: features);

        await DbContext.SubscriptionPlans.AddAsync(plan);
        await SaveAndDetachAllAsync();

        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(FeatureFlagRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [feature])!;

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("connected to Feature");
    }

    #endregion

    #region Specification Tests

    [Fact]
    public async Task FirstOrDefaultAsync_WithSpecification_ShouldReturnCorrectEntity()
    {
        // Arrange
        var spec = new GetFeatureFlagByNameContainsSpec(_featureFlag1.Name);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result!.Name.ShouldBe(_featureFlag1.Name);
    }

    //-----------------------------//

    [Fact]
    public async Task ListAllAsync_WithSpecification_ShouldReturnFilteredResults()
    {
        // Arrange
        var spec = new GetFeatureFlagByNameStartsWithSpec("Feature");

        // Act
        var results = await _repo.ListAllAsync(spec);

        // Assert
        results.Count.ShouldBe(2);
        results.ShouldAllBe(f => f.Name.StartsWith("Feature"));
    }

    #endregion

    #region Pagination Tests

    [Fact]
    public async Task PageAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        var request = new PagedRequest
        {
            PageNumber = 1,
            PageSize = 1,
            SortList = [new SortRequest { Field = "name", SortDescending = false }]
        };

        // Act
        var page = await _repo.PageAsync(request);

        // Assert
        page.ShouldNotBeNull();
        page.Data.Count.ShouldBe(1);
        page.TotalItems.ShouldBe(2);
        page.Number.ShouldBe(1);
        page.Size.ShouldBe(1);
    }

    #endregion
}
