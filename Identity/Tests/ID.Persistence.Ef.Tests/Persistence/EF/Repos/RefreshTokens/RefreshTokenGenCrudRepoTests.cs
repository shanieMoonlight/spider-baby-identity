using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.Refreshing.ValueObjects;
using ID.Infrastructure.Persistance.EF.Repos;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.RefreshTokens;

public class RefreshTokenGenCrudRepoTests : RepoTestBase, IAsyncLifetime
{
    private RefreshTokenRepo _repo = null!;

    private readonly IdRefreshToken _testToken = RefreshTokenDataFactory.Create(
        userId: Guid.NewGuid(),
        payload: "test_token_payload",
        expiresOnUtc: DateTime.UtcNow.AddDays(7));

    //-----------------------------//

    public async Task InitializeAsync()
    {
        _repo = CreateRepository<RefreshTokenRepo>();
        await SeedDatabaseAsync();
    }

    //-----------------------------//

    public Task DisposeAsync() => Task.CompletedTask;

    //-----------------------------//

    protected override async Task SeedDatabaseAsync()
    {
        await DbContext.RefreshTokens.AddAsync(_testToken);
        await SaveAndDetachAllAsync();
    }

    //-----------------------------//

    #region Basic CRUD Tests

    [Fact]
    public async Task AddAsync_ShouldAddRefreshToken()
    {
        // Arrange
        var newToken = RefreshTokenDataFactory.Create(
            userId: Guid.NewGuid(),
            payload: "new_test_payload");

        // Act
        var result = await _repo.AddAsync(newToken);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(newToken.Id);
        result.UserId.ShouldBe(newToken.UserId);
        result.Payload.ShouldBe("new_test_payload");
    }

    //------------------------//  

    [Fact]
    public async Task FirstOrDefaultByIdAsync_ShouldReturnCorrectRefreshToken()
    {
        // Act
        var result = await _repo.FirstOrDefaultByIdAsync(_testToken.Id);

        // Assert
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(_testToken.Id);
        result.UserId.ShouldBe(_testToken.UserId);
        result.Payload.ShouldBe(_testToken.Payload);
        result.ExpiresOnUtc.ShouldBe(_testToken.ExpiresOnUtc);
    }

    //------------------------//  

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

    //------------------------//
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateRefreshToken()
    {
        // Arrange
        var token = await _repo.FirstOrDefaultByIdAsync(_testToken.Id);
        token.ShouldNotBeNull();

        var originalPayload = token!.Payload;
        var newPayload = "updated_test_payload";
        var newExpiryTimeSpan = TimeSpan.FromDays(14);

        token.Update(
          TokenPayload.Create(newPayload),
       TokenLifetime.Create(newExpiryTimeSpan)
            );

        // Act
        var result = await _repo.UpdateAsync(token);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Payload.ShouldBe(newPayload);
        result.ExpiresOnUtc.ShouldBeInRange(
            DateTime.UtcNow.AddDays(13.9), 
            DateTime.UtcNow.AddDays(14.1));
        result.Payload.ShouldNotBe(originalPayload);
    }

    //------------------------//  

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRefreshToken()
    {
        // Arrange
        var tokenToDelete = RefreshTokenDataFactory.Create(userId: Guid.NewGuid());
        await _repo.AddAsync(tokenToDelete);
        await SaveAndDetachAllAsync();

        // Act
        await _repo.DeleteAsync(tokenToDelete.Id);
        await SaveAndDetachAllAsync();

        // Assert
        var deletedToken = await _repo.FirstOrDefaultByIdAsync(tokenToDelete.Id);
        deletedToken.ShouldBeNull();
    }

    //------------------------//
    //
    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrow()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act & Assert
        var deleteAction = async () => await _repo.DeleteAsync(invalidId);
        await deleteAction.ShouldNotThrowAsync();
    }

    //------------------------//  

    [Fact]
    public async Task ListAllAsync_ShouldReturnAllRefreshTokens()
    {
        // Arrange
        var additionalToken = RefreshTokenDataFactory.Create(
            userId: Guid.NewGuid(),
            payload: "additional_token");
        await _repo.AddAsync(additionalToken);
        await SaveAndDetachAllAsync();

        // Act
        var result = await _repo.ListAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);
        result.ShouldContain(t => t.Id == _testToken.Id);
        result.ShouldContain(t => t.Id == additionalToken.Id);
    }

    #endregion

    #region Property Validation Tests

    [Fact]
    public async Task AddAsync_WithValidProperties_ShouldPersistCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var payload = "comprehensive_test_payload";
        var expiryDate = DateTime.UtcNow.AddDays(30);

        var token = RefreshTokenDataFactory.Create(
            userId: userId,
            payload: payload,
            expiresOnUtc: expiryDate);

        // Act
        var result = await _repo.AddAsync(token);
        await SaveAndDetachAllAsync();

        // Retrieve from database to verify persistence
        var retrievedToken = await _repo.FirstOrDefaultByIdAsync(result.Id);

        // Assert
        retrievedToken.ShouldNotBeNull();
        retrievedToken!.UserId.ShouldBe(userId);
        retrievedToken.Payload.ShouldBe(payload);
        retrievedToken.ExpiresOnUtc.ShouldBe(expiryDate);
        var what = retrievedToken.DateCreated.Date;
        retrievedToken.DateCreated.Date.ShouldBe(DateTime.UtcNow.Date);
    }

    //------------------------//  

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(365)]
    public async Task AddAsync_WithDifferentExpirationPeriods_ShouldPersistCorrectly(int daysToAdd)
    {
        // Arrange
        var expiryDate = DateTime.UtcNow.AddDays(daysToAdd);
        var token = RefreshTokenDataFactory.Create(
            userId: Guid.NewGuid(),
            payload: $"token_expires_in_{daysToAdd}_days",
            expiresOnUtc: expiryDate);

        // Act
        var result = await _repo.AddAsync(token);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ExpiresOnUtc.ShouldBe(expiryDate);
        result.Payload.ShouldBe($"token_expires_in_{daysToAdd}_days");
    }

    //------------------------//  

    [Fact]
    public async Task AddAsync_WithLongPayload_ShouldPersistCorrectly()
    {
        // Arrange
        var longPayload = new string('x', 1000); // Very long payload
        var token = RefreshTokenDataFactory.Create(
            userId: Guid.NewGuid(),
            payload: longPayload);

        // Act
        var result = await _repo.AddAsync(token);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Payload.ShouldBe(longPayload);
        result.Payload.Length.ShouldBe(1000);    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task AddAsync_WithNullEntity_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var addAction = async () => await _repo.AddAsync(null!);
        await addAction.ShouldThrowAsync<ArgumentNullException>();
    }

    //------------------------//  

    [Fact]
    public async Task UpdateAsync_WithNullEntity_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var updateAction = async () => await _repo.UpdateAsync(null!);
        await updateAction.ShouldThrowAsync<ArgumentNullException>();
    }

    #endregion

    #region Collection Operations Tests

    [Fact]
    public async Task AddRangeAsync_ShouldAddMultipleRefreshTokens()
    {
        // Arrange
        var tokens = new List<IdRefreshToken>
        {
            RefreshTokenDataFactory.Create(userId: Guid.NewGuid(), payload: "Batch Token 1"),
            RefreshTokenDataFactory.Create(userId: Guid.NewGuid(), payload: "Batch Token 2"),
            RefreshTokenDataFactory.Create(userId: Guid.NewGuid(), payload: "Batch Token 3")
        };

        // Act
        await _repo.AddRangeAsync(tokens);
        await SaveAndDetachAllAsync();

        // Assert
        var allTokens = await _repo.ListAllAsync();
        allTokens.ShouldContain(t => t.Payload == "Batch Token 1");
        allTokens.ShouldContain(t => t.Payload == "Batch Token 2");
        allTokens.ShouldContain(t => t.Payload == "Batch Token 3");
    }

    //------------------------//  

    [Fact]
    public async Task ListByIdsAsync_ShouldReturnCorrectRefreshTokens()
    {
        // Arrange
        var token1 = RefreshTokenDataFactory.Create(userId: Guid.NewGuid(), payload: "Token 1");
        var token2 = RefreshTokenDataFactory.Create(userId: Guid.NewGuid(), payload: "Token 2");
        var token3 = RefreshTokenDataFactory.Create(userId: Guid.NewGuid(), payload: "Token 3");

        await _repo.AddRangeAsync([token1, token2, token3]);
        await SaveAndDetachAllAsync();

        var idsToRetrieve = new[] { token1.Id, token3.Id };

        // Act
        var result = await _repo.ListByIdsAsync(idsToRetrieve);

        // Assert
        result.Count.ShouldBe(2);
        result.ShouldContain(t => t.Id == token1.Id);
        result.ShouldContain(t => t.Id == token3.Id);
        result.ShouldNotContain(t => t.Id == token2.Id);
    }

    #endregion

    #region Full CRUD Workflow Test

    [Fact]
    public async Task FullCrudWorkflow_ShouldWorkCorrectly()
    {
        // Create
        var userId = Guid.NewGuid();
        var entity = RefreshTokenDataFactory.Create(
            userId: userId,
            payload: "WorkflowTest");
        var added = await _repo.AddAsync(entity);
        await SaveAndDetachAllAsync();

        // Read
        var retrieved = await _repo.FirstOrDefaultByIdAsync(added.Id);
        retrieved.ShouldNotBeNull();
        retrieved!.Payload.ShouldBe("WorkflowTest");        // Update
        var newTokenPayload = TokenPayload.Create("UpdatedWorkflow");
        var newTokenLifetime = TokenLifetime.Create(TimeSpan.FromDays(14));
        retrieved.Update(newTokenPayload, newTokenLifetime);
        await _repo.UpdateAsync(retrieved);
        await SaveAndDetachAllAsync();

        // Verify Update
        var updated = await _repo.FirstOrDefaultByIdAsync(retrieved.Id);
        updated!.Payload.ShouldBe("UpdatedWorkflow");

        // Delete
        await _repo.DeleteAsync(updated);
        await SaveAndDetachAllAsync();

        // Verify Delete
        var deleted = await _repo.FirstOrDefaultByIdAsync(updated.Id);
        deleted.ShouldBeNull();
    }

    #endregion

    #region Expiration Boundary Tests

    [Fact]
    public async Task RefreshToken_WithPastExpirationDate_ShouldStillPersist()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-10);
        var expiredToken = RefreshTokenDataFactory.Create(
            userId: Guid.NewGuid(),
            payload: "expired_token",
            expiresOnUtc: pastDate);

        // Act
        var result = await _repo.AddAsync(expiredToken);
        await SaveAndDetachAllAsync();

        // Assert - Repository should store expired tokens (business logic handles expiry)
        result.ShouldNotBeNull();
        result.ExpiresOnUtc.ShouldBe(pastDate);

        var retrieved = await _repo.FirstOrDefaultByIdAsync(result.Id);
        retrieved.ShouldNotBeNull();
        retrieved!.ExpiresOnUtc.ShouldBe(pastDate);
    }

    [Fact]
    public async Task RefreshToken_WithFutureExpirationDate_ShouldPersist()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddYears(1);
        var longLivedToken = RefreshTokenDataFactory.Create(
            userId: Guid.NewGuid(),
            payload: "long_lived_token",
            expiresOnUtc: futureDate);

        // Act
        var result = await _repo.AddAsync(longLivedToken);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ExpiresOnUtc.ShouldBe(futureDate);
    }

    #endregion
}
