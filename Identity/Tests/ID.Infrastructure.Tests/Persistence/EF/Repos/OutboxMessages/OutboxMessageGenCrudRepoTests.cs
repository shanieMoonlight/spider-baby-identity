using ClArch.ValueObjects;
using FluentAssertions;
using ID.Domain.Entities.OutboxMessages;
using ID.Infrastructure.Persistance.EF.Repos;
using ID.Infrastructure.Tests.Persistence.EF.TestHelpers;
using ID.Tests.Data.Factories;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.OutboxMessages;

/// <summary>
/// Tests for AGenCrudRepo functionality using OutboxMessageRepo as concrete implementation.
/// These tests verify the base CRUD operations work correctly.
/// </summary>
public class OutboxMessageGenCrudRepoTests : RepoTestBase
{
    private readonly OutboxMessageRepo _repo = null!;

    public OutboxMessageGenCrudRepoTests()
    {
        _repo = CreateRepository<OutboxMessageRepo>();
    }

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_WithValidEntity_ShouldAddToContext()
    {
        // Arrange
        var entity = OutboxMessageDataFactory.Create(type: "TestMessage");

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
            OutboxMessageDataFactory.Create(type: "Message1"),
            OutboxMessageDataFactory.Create(type: "Message2"),
            OutboxMessageDataFactory.Create(type: "Message3")
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
        var entities = Array.Empty<IdOutboxMessage>();

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
        var entity = OutboxMessageDataFactory.Create(type: "WorkflowTest");
        var added = await _repo.AddAsync(entity);
        await SaveAndDetachAllAsync();

        // Read
        var retrieved = await _repo.FirstOrDefaultByIdAsync(added.Id);
        retrieved.ShouldNotBeNull();
        retrieved!.Type.ShouldBe("WorkflowTest");

        // Delete - This should fail if message is unprocessed
        var deleteAction = async () => await _repo.DeleteAsync(retrieved);
        await deleteAction.ShouldThrowAsync<CantDeleteException>();


        // Update - Set as processed
        retrieved.SetProcessed();
        await _repo.UpdateAsync(retrieved);
        await SaveAndDetachAllAsync();

        // Verify Update
        var updated = await _repo.FirstOrDefaultByIdAsync(retrieved.Id);
        updated!.ProcessedOnUtc.ShouldNotBeNull();

        await _repo.DeleteAsync(updated);
        await SaveAndDetachAllAsync();
        var allEntities = await _repo.ListAllAsync();
        allEntities.Count.ShouldBe(0);



    }

    #endregion
}
