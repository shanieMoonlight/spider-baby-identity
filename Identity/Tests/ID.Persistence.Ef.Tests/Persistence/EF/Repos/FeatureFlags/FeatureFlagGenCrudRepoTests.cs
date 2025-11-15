using ClArch.ValueObjects;
using FluentAssertions;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.FeatureFlags;

/// <summary>
/// Tests for AGenCrudRepo functionality using FeatureFlagRepo as concrete implementation.
/// These tests verify the base CRUD operations work correctly.
/// </summary>
public class FeatureFlagGenCrudRepoTests : RepoTestBase
{
    private readonly FeatureFlagRepo _repo = null!;

    public FeatureFlagGenCrudRepoTests()
    {
        _repo = CreateRepository<FeatureFlagRepo>();
    }

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_WithValidEntity_ShouldAddToContext()
    {
        // Arrange
        var entity = FeatureFlagDataFactory.Create(name: "TestFeature");

        // Act
        var result = await _repo.AddAsync(entity);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeSameAs(entity);

        // Verify it's tracked by EF
        DbContext.Entry(result).State.ShouldBe(EntityState.Added);
    }


    #endregion

    #region AddRangeAsync Tests

    [Fact]
    public async Task AddRangeAsync_WithMultipleEntities_ShouldAddAllToContext()
    {
        // Arrange
        var entities = new[]
        {
            FeatureFlagDataFactory.Create(name: "Feature1"),
            FeatureFlagDataFactory.Create(name: "Feature2"),
            FeatureFlagDataFactory.Create(name: "Feature3")
        };

        // Act
        await _repo.AddRangeAsync(entities);

        // Assert
        foreach (var entity in entities)
        {
            DbContext.Entry(entity).State.ShouldBe(EntityState.Added);
        }
    }

    //-----------------------//

    [Fact]
    public async Task AddRangeAsync_WithEmptyCollection_ShouldNotThrow()
    {
        // Arrange
        var entities = Array.Empty<FeatureFlag>();

        // Act & Assert
        var action = async () => await _repo.AddRangeAsync(entities);
        await action.ShouldNotThrowAsync();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task FullCrudWorkflow_ShouldWorkCorrectly()
    {
        // Create
        var entity = FeatureFlagDataFactory.Create(name: "WorkflowTest");
        var added = await _repo.AddAsync(entity);
        await SaveAndDetachAllAsync();

        // Read
        var retrieved = await _repo.FirstOrDefaultByIdAsync(added.Id);
        retrieved.ShouldNotBeNull();
        retrieved!.Name.ShouldBe("WorkflowTest");

        // Update
        retrieved.Update(
          Name.Create("UpdatedWorkflow"),
          Description.Create(entity.Description)
        );

        await _repo.UpdateAsync(retrieved);
        await SaveAndDetachAllAsync();

        // Verify Update
        var updated = await _repo.FirstOrDefaultByIdAsync(retrieved.Id);
        updated!.Name.ShouldBe("UpdatedWorkflow");

        // Delete
        await _repo.DeleteAsync(updated);
        await SaveAndDetachAllAsync();

        // Verify Delete
        var deleted = await _repo.FirstOrDefaultByIdAsync(updated.Id);
        deleted.ShouldBeNull();
    }

    #endregion
}
