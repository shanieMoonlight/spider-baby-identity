using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.Refreshing.ValueObjects;
using ID.Infrastructure.Persistance.EF.Repos;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.RefreshTokens;

public class RefreshTokenRepoTests : RepoTestBase, IAsyncLifetime
{
    private RefreshTokenRepo _repo = null!;

    private readonly IdRefreshToken _activeToken = RefreshTokenDataFactory.Create(
        userId: Guid.NewGuid(),
        payload: "active_token_payload",
        expiresOnUtc: DateTime.UtcNow.AddDays(7));

    private readonly IdRefreshToken _expiredToken = RefreshTokenDataFactory.Create(
        userId: Guid.NewGuid(),
        payload: "expired_token_payload",
        expiresOnUtc: DateTime.UtcNow.AddDays(-1));

    private readonly List<IdRefreshToken> _refreshTokens;

    //-----------------------------//

    public RefreshTokenRepoTests()
    {
        _refreshTokens = [_activeToken, _expiredToken];
    }

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
        await DbContext.RefreshTokens.AddRangeAsync(_refreshTokens);
        await SaveAndDetachAllAsync();
    }

    //-----------------------------//

    #region CRUD Operations Tests

    [Fact]
    public async Task AddAsync_ShouldAddRefreshToken()
    {
        // Arrange
        var newToken = RefreshTokenDataFactory.Create(
            userId: Guid.NewGuid(),
            payload: "new_token_payload");

        // Act
        var result = await _repo.AddAsync(newToken);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(newToken.Id);
        result.UserId.ShouldBe(newToken.UserId);
        result.Payload.ShouldBe("new_token_payload");
    }

    [Fact]
    public async Task FirstOrDefaultByIdAsync_ShouldReturnCorrectRefreshToken()
    {
        // Act
        var result = await _repo.FirstOrDefaultByIdAsync(_activeToken.Id);

        // Assert
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(_activeToken.Id);
        result.UserId.ShouldBe(_activeToken.UserId);
        result.Payload.ShouldBe(_activeToken.Payload);
    }

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
    public async Task UpdateAsync_ShouldUpdateRefreshToken()
    {
        // Arrange
        var token = await _repo.FirstOrDefaultByIdAsync(_activeToken.Id);
        token.ShouldNotBeNull(); var originalPayload = token!.Payload;
        var newPayload = TokenPayload.Create("updated_token_payload");
        var newTokenLifetime = TokenLifetime.Create(TimeSpan.FromDays(14));

        token.Update(newPayload, newTokenLifetime);

        // Act
        var result = await _repo.UpdateAsync(token);
        await SaveAndDetachAllAsync();        // Assert
        result.ShouldNotBeNull();
        result.Payload.ShouldBe(newPayload.Value);
        result.ExpiresOnUtc.ShouldBeInRange(DateTime.UtcNow.AddDays(13.9), DateTime.UtcNow.AddDays(14.1));
        result.Payload.ShouldNotBe(originalPayload);
    }

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

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrow()
    {
        // Arrange
        var invalidId = Guid.NewGuid();        // Act & Assert
        await Should.NotThrowAsync(() => _repo.DeleteAsync(invalidId));
    }

    [Fact]
    public async Task ListAllAsync_ShouldReturnAllRefreshTokens()
    {
        // Act
        var result = await _repo.ListAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(_refreshTokens.Count);
        result.ShouldContain(t => t.Id == _activeToken.Id);
        result.ShouldContain(t => t.Id == _expiredToken.Id);
    }

    #endregion

    #region UpsertRefreshTokenAsync Tests    
    [Fact(Skip = "UpsertRefreshTokenAsync uses SQL Server MERGE which is not supported by in-memory database")]
    public async Task UpsertRefreshTokenAsync_WithNewToken_ShouldInsertToken()
    {
        // Arrange
        var newUserId = Guid.NewGuid();
        var newToken = RefreshTokenDataFactory.Create(
            userId: newUserId,
            payload: "upsert_new_payload");

        // Act
        await _repo.UpsertRefreshTokenAsync(newToken);
        await SaveAndDetachAllAsync();

        // Assert
        var retrievedToken = await _repo.FirstOrDefaultByIdAsync(newToken.Id);
        retrievedToken.ShouldNotBeNull();
        retrievedToken!.UserId.ShouldBe(newUserId);
        retrievedToken.Payload.ShouldBe("upsert_new_payload");
    }

    [Fact(Skip = "UpsertRefreshTokenAsync uses SQL Server MERGE which is not supported by in-memory database")]
    public async Task UpsertRefreshTokenAsync_WithExistingUserId_ShouldUpdateToken()
    {
        // Arrange
        var existingUserId = _activeToken.UserId;
        var updatedToken = RefreshTokenDataFactory.Create(
            userId: existingUserId,
            payload: "updated_via_upsert",
            expiresOnUtc: DateTime.UtcNow.AddDays(30));

        // Act
        await _repo.UpsertRefreshTokenAsync(updatedToken);
        await SaveAndDetachAllAsync();

        // Assert
        var tokens = await _repo.ListAllAsync();
        var userTokens = tokens.Where(t => t.UserId == existingUserId).ToList();

        // Should still have only one token for this user
        userTokens.Count.ShouldBe(1);
        userTokens.First().Payload.ShouldBe("updated_via_upsert");
        userTokens.First().ExpiresOnUtc.Date.ShouldBe(DateTime.UtcNow.AddDays(30).Date);
    }

    [Fact(Skip = "UpsertRefreshTokenAsync uses SQL Server MERGE which is not supported by in-memory database")]
    public async Task UpsertRefreshTokenAsync_MultipleCalls_ShouldMaintainOneTokenPerUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var firstToken = RefreshTokenDataFactory.Create(userId: userId, payload: "first_payload");
        var secondToken = RefreshTokenDataFactory.Create(userId: userId, payload: "second_payload");
        var thirdToken = RefreshTokenDataFactory.Create(userId: userId, payload: "third_payload");

        // Act
        await _repo.UpsertRefreshTokenAsync(firstToken);
        await SaveAndDetachAllAsync();

        await _repo.UpsertRefreshTokenAsync(secondToken);
        await SaveAndDetachAllAsync();

        await _repo.UpsertRefreshTokenAsync(thirdToken);
        await SaveAndDetachAllAsync();

        // Assert
        var allTokens = await _repo.ListAllAsync();
        var userTokens = allTokens.Where(t => t.UserId == userId).ToList();

        userTokens.Count.ShouldBe(1);
        userTokens.First().Payload.ShouldBe("third_payload");
    }

    #endregion

    #region Expiration Tests

    [Fact]
    public async Task RefreshToken_ShouldHandleExpirationDates()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddYears(1);
        var pastDate = DateTime.UtcNow.AddYears(-1);

        var futureToken = RefreshTokenDataFactory.Create(
            userId: Guid.NewGuid(),
            expiresOnUtc: futureDate);

        var pastToken = RefreshTokenDataFactory.Create(
            userId: Guid.NewGuid(),
            expiresOnUtc: pastDate);

        // Act
        await _repo.AddAsync(futureToken);
        await _repo.AddAsync(pastToken);
        await SaveAndDetachAllAsync();

        // Assert
        var retrievedFuture = await _repo.FirstOrDefaultByIdAsync(futureToken.Id);
        var retrievedPast = await _repo.FirstOrDefaultByIdAsync(pastToken.Id);

        retrievedFuture.ShouldNotBeNull();
        retrievedFuture!.ExpiresOnUtc.ShouldBe(futureDate);

        retrievedPast.ShouldNotBeNull();
        retrievedPast!.ExpiresOnUtc.ShouldBe(pastDate);
    }

    #endregion

    #region Pagination Tests

    [Fact]
    public async Task PageAsync_WithValidPageRequest_ShouldReturnCorrectPage()
    {
        // Arrange
        var pageRequest = new Pagination.PagedRequest { PageNumber = 1, PageSize = 1 };

        // Act
        var result = await _repo.PageAsync(pageRequest);

        // Assert
        result.ShouldNotBeNull();
        result.Data.Count.ShouldBe(1);
        result.TotalItems.ShouldBe(_refreshTokens.Count);
        result.Number.ShouldBe(1);
        result.Size.ShouldBe(1);
        result.TotalPages.ShouldBe(_refreshTokens.Count);
    }

    [Fact]
    public async Task PageAsync_WithLargePage_ShouldReturnAllItems()
    {
        // Arrange
        var pageRequest = new Pagination.PagedRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _repo.PageAsync(pageRequest);

        // Assert
        result.ShouldNotBeNull();
        result.Data.Count.ShouldBe(_refreshTokens.Count);
        result.TotalItems.ShouldBe(_refreshTokens.Count);
        result.Number.ShouldBe(1);
        result.Size.ShouldBe(10);
        result.TotalPages.ShouldBe(1);
    }

    [Fact]
    public async Task PageAsync_WithSecondPage_ShouldReturnRemainingItems()
    {
        // Arrange
        var pageRequest = new Pagination.PagedRequest { PageNumber = 2, PageSize = 1 };

        // Act
        var result = await _repo.PageAsync(pageRequest);

        // Assert
        result.ShouldNotBeNull();
        result.Data.Count.ShouldBe(1);
        result.TotalItems.ShouldBe(_refreshTokens.Count);
        result.Number.ShouldBe(2);
        result.Size.ShouldBe(1);
    }

    [Fact]
    public async Task PageAsync_WithEmptyPage_ShouldReturnEmptyResult()
    {
        // Arrange
        var pageRequest = new Pagination.PagedRequest { PageNumber = 10, PageSize = 1 };

        // Act
        var result = await _repo.PageAsync(pageRequest);

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldBeEmpty();
        result.TotalItems.ShouldBe(_refreshTokens.Count);
        result.Number.ShouldBe(10);
        result.Size.ShouldBe(1);
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

    #region Error Handling Tests

    [Fact]
    public async Task AddAsync_WithNullEntity_ShouldThrowArgumentNullException()
    {        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(() => _repo.AddAsync(null!));
    }

    [Fact]
    public async Task UpdateAsync_WithNullEntity_ShouldThrowArgumentNullException()
    {        // Act & Assert        
        await Should.ThrowAsync<ArgumentNullException>(() => _repo.UpdateAsync(null!));
    }

    [Fact(Skip = "UpsertRefreshTokenAsync uses SQL Server MERGE which is not supported by in-memory database")]
    public async Task UpsertRefreshTokenAsync_WithNullEntity_ShouldThrowException()
    {
        // Act & Assert
        var upsertAction = async () => await _repo.UpsertRefreshTokenAsync(null!);
        await upsertAction.ShouldThrowAsync<Exception>();
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task ListAllAsync_WithEmptyDatabase_ShouldReturnEmptyList()
    {
        // Arrange - Clear existing data
        var existingTokens = await _repo.ListAllAsync();
        foreach (var token in existingTokens)
        {
            await _repo.DeleteAsync(token.Id);
        }
        await SaveAndDetachAllAsync();

        // Act
        var result = await _repo.ListAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        // Act
        var count = await _repo.CountAsync();

        // Assert
        count.ShouldBe(_refreshTokens.Count);
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Act
        var exists = await _repo.ExistsAsync(_activeToken.Id);

        // Assert
        exists.ShouldBeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var exists = await _repo.ExistsAsync(invalidId);

        // Assert
        exists.ShouldBeFalse();
    }

    #endregion

    #region Consistency Tests

    [Fact]
    public async Task ConcurrentOperations_ShouldMaintainDataIntegrity()
    {
        // Arrange
        var token1 = RefreshTokenDataFactory.Create(userId: Guid.NewGuid(), payload: "Concurrent Token 1");
        var token2 = RefreshTokenDataFactory.Create(userId: Guid.NewGuid(), payload: "Concurrent Token 2");

        // Act - Simulate concurrent additions
        var task1 = _repo.AddAsync(token1);
        var task2 = _repo.AddAsync(token2);

        await Task.WhenAll(task1, task2);
        await SaveAndDetachAllAsync();

        // Assert
        var allTokens = await _repo.ListAllAsync();
        allTokens.ShouldContain(t => t.Payload == "Concurrent Token 1");
        allTokens.ShouldContain(t => t.Payload == "Concurrent Token 2");
    }

    [Fact]
    public async Task UpdateAfterDelete_ShouldNotAffectDeletedEntity()
    {
        // Arrange
        var token = RefreshTokenDataFactory.Create(userId: Guid.NewGuid(), payload: "Token to Delete");
        await _repo.AddAsync(token);
        await SaveAndDetachAllAsync();

        // Act
        await _repo.DeleteAsync(token.Id);
        await SaveAndDetachAllAsync();        // Attempt to update deleted entity
        token.Update(TokenPayload.Create("Updated Payload"), TokenLifetime.Create(TimeSpan.FromDays(1)));

        // Assert - The update should not affect the database since the entity is deleted
        var deletedToken = await _repo.FirstOrDefaultByIdAsync(token.Id);
        deletedToken.ShouldBeNull();
    }

    #endregion

    #region Business Logic Tests

    [Fact]
    public async Task RefreshToken_WithSameUserIdButDifferentTokenId_ShouldCoexistBeforeUpsert()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token1 = RefreshTokenDataFactory.Create(userId: userId, payload: "First Token");
        var token2 = RefreshTokenDataFactory.Create(userId: userId, payload: "Second Token");

        // Act - Add both tokens using regular AddAsync (not upsert)
        await _repo.AddAsync(token1);
        await _repo.AddAsync(token2);
        await SaveAndDetachAllAsync();

        // Assert - Both should exist since we used AddAsync, not UpsertAsync
        var allTokens = await _repo.ListAllAsync();
        var userTokens = allTokens.Where(t => t.UserId == userId).ToList();

        userTokens.Count.ShouldBe(2);
        userTokens.ShouldContain(t => t.Payload == "First Token");
        userTokens.ShouldContain(t => t.Payload == "Second Token");
    }

    #endregion
}
