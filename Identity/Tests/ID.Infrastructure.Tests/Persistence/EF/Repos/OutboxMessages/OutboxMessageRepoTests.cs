using ID.Domain.Entities.OutboxMessages;

namespace ID.Infrastructure.Tests.Persistence.EF.Repos.OutboxMessages;

public class OutboxMessageRepoTests : RepoTestBase, IAsyncLifetime
{
    private OutboxMessageRepo _repo = null!;

    private readonly IdOutboxMessage _unprocessedMessage = OutboxMessageDataFactory.Create(type: "UnprocessedEvent", processedOn: null);
    private readonly IdOutboxMessage _processedMessage = OutboxMessageDataFactory.Create(type: "ProcessedEvent");

    private readonly List<IdOutboxMessage> _idOutboxMessages;

    //-----------------------------//
    public OutboxMessageRepoTests()
    {
        _idOutboxMessages = [_unprocessedMessage, _processedMessage];
    }

    //-----------------------------//

    public async Task InitializeAsync()
    {
        _repo = CreateRepository<OutboxMessageRepo>();
        await SeedDatabaseAsync();
    }

    //-----------------------------//

    public Task DisposeAsync() => Task.CompletedTask;

    //-----------------------------//

    protected override async Task SeedDatabaseAsync()
    {
        // Ensure processed message is actually processed
        _processedMessage.SetProcessed();
        
        await DbContext.OutboxMessages.AddRangeAsync(_idOutboxMessages);
        await SaveAndDetachAllAsync();
    }

    //-----------------------------//

    #region CRUD Operations Tests

    [Fact]
    public async Task AddAsync_ShouldAddOutboxMessage()
    {
        // Arrange
        var newMessage = OutboxMessageDataFactory.Create(type: "NewTestEvent");

        // Act
        var result = await _repo.AddAsync(newMessage);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Type.ShouldBe("NewTestEvent");

        var fromDb = await _repo.FirstOrDefaultByIdAsync(result.Id);
        fromDb.ShouldNotBeNull();
        fromDb!.Type.ShouldBe("NewTestEvent");
    }

    //-----------------------------//

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOutboxMessage()
    {
        // Arrange
        var spec = new GetOutboxMessageByTypeSpec(_unprocessedMessage.Type);
        var message = await _repo.FirstOrDefaultAsync(spec);
        message.ShouldNotBeNull();

        // Act
        message.SetProcessed();
        await _repo.UpdateAsync(message);
        await SaveAndDetachAllAsync();

        // Assert
        var updated = await _repo.FirstOrDefaultByIdAsync(message.Id);
        updated.ShouldNotBeNull();
        updated!.ProcessedOnUtc.ShouldNotBeNull();
    }

    //-----------------------------//

    [Fact]
    public async Task ListAllAsync_ShouldReturnAllOutboxMessages()
    {
        // Act
        var messages = await _repo.ListAllAsync();

        // Assert
        messages.Count.ShouldBe(2);
        messages.ShouldContain(m => m.Type == _unprocessedMessage.Type);
        messages.ShouldContain(m => m.Type == _processedMessage.Type);
    }

    //-----------------------------//

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var spec = new GetOutboxMessageByTypeSpec(_unprocessedMessage.Type);
        var message = await _repo.FirstOrDefaultAsync(spec);

        // Act
        var exists = await _repo.ExistsAsync(message!.Id);

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
    public async Task DeleteAsync_WithUnprocessedMessage_ShouldThrowCantDeleteException()
    {
        // Arrange
        var spec = new GetOutboxMessageByTypeSpec(_unprocessedMessage.Type);
        var message = await _repo.FirstOrDefaultAsync(spec);
        message.ShouldNotBeNull();

        // Act & Assert
        var deleteAction = async () => await _repo.DeleteAsync(message!);
        await deleteAction.ShouldThrowAsync<CantDeleteException>();
    }

    //-----------------------------//

    [Fact]
    public async Task DeleteAsync_WithProcessedMessage_ShouldSucceed()
    {
        // Arrange
        var spec = new GetOutboxMessageByTypeSpec(_processedMessage.Type);
        var message = await _repo.FirstOrDefaultAsync(spec);
        message.ShouldNotBeNull();

        // Act & Assert
        var deleteAction = async () => await _repo.DeleteAsync(message!);
        await deleteAction.ShouldNotThrowAsync();

        await SaveAndDetachAllAsync();

        // Verify deletion
        var deleted = await _repo.FirstOrDefaultByIdAsync(message!.Id);
        deleted.ShouldBeNull();
    }

    //-----------------------------//

    [Fact]
    public async Task CanDeleteAsync_WithUnprocessedMessage_ShouldReturnFailure()
    {
        // Arrange
        var spec = new GetOutboxMessageByTypeSpec(_unprocessedMessage.Type);
        var message = await _repo.FirstOrDefaultAsync(spec);

        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(OutboxMessageRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        
        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [message])!;

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("has not been processsed yet");
    }

    //-----------------------------//

    [Fact]
    public async Task CanDeleteAsync_WithProcessedMessage_ShouldReturnSuccess()
    {
        // Arrange
        var spec = new GetOutboxMessageByTypeSpec(_processedMessage.Type);
        var message = await _repo.FirstOrDefaultAsync(spec);

        // Use reflection to access protected method for testing
        var canDeleteMethod = typeof(OutboxMessageRepo)
            .GetMethod("CanDeleteAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        
        // Act
        var result = await (Task<BasicResult>)canDeleteMethod!.Invoke(_repo, [message])!;

        // Assert
        result.Succeeded.ShouldBeTrue();
    }

    #endregion

    #region Specification Tests

    [Fact]
    public async Task FirstOrDefaultAsync_WithTypeSpecification_ShouldReturnCorrectEntity()
    {
        // Arrange
        var spec = new GetOutboxMessageByTypeSpec(_unprocessedMessage.Type);

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result!.Type.ShouldBe(_unprocessedMessage.Type);
    }

    //-----------------------------//

    [Fact]
    public async Task FirstOrDefaultAsync_WithTypeContainsSpecification_ShouldReturnCorrectEntity()
    {
        // Arrange
        var spec = new GetOutboxMessageByTypeContainsSpec("Event");

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result!.Type.ShouldContain("Event");
    }

    //-----------------------------//

    [Fact]
    public async Task FirstOrDefaultAsync_WithTypeStartsWithSpecification_ShouldReturnCorrectEntity()
    {
        // Arrange
        var spec = new GetOutboxMessageByTypeStartsWithSpec("Unprocessed");

        // Act
        var result = await _repo.FirstOrDefaultAsync(spec);

        // Assert
        result.ShouldNotBeNull();
        result!.Type.ShouldStartWith("Unprocessed");
    }

    //-----------------------------//

    [Fact]
    public async Task ListAllAsync_WithProcessedMessagesSpec_ShouldReturnFilteredResults()
    {
        // Arrange
        var spec = new GetProcessedOutboxMessagesSpec();

        // Act
        var results = await _repo.ListAllAsync(spec);

        // Assert
        results.Count.ShouldBe(1);
        results.ShouldAllBe(m => m.ProcessedOnUtc != null);
    }

    //-----------------------------//

    [Fact]
    public async Task ListAllAsync_WithUnprocessedMessagesSpec_ShouldReturnFilteredResults()
    {
        // Arrange
        var spec = new GetUnprocessedOutboxMessagesSpec();

        // Act
        var results = await _repo.ListAllAsync(spec);

        // Assert
        results.Count.ShouldBe(1);
        results.ShouldAllBe(m => m.ProcessedOnUtc == null);
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
            SortList = [new SortRequest { Field = "type", SortDescending = false }]
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

    #region OutboxMessage-Specific Tests

    [Fact]
    public async Task CreateFromDomainEvent_ShouldCreateMessageCorrectly()
    {
        // Arrange - Create a simple test message
        var testMessage = OutboxMessageDataFactory.Create(type: "TestDomainEvent");

        // Act
        var result = await _repo.AddAsync(testMessage);
        await SaveAndDetachAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Type.ShouldBe("TestDomainEvent");
        result.ContentJson.ShouldNotBeNullOrEmpty();
        result.ProcessedOnUtc.ShouldBeNull();
    }

    //-----------------------------//

    [Fact]
    public async Task SetProcessed_ShouldUpdateProcessedOnUtc()
    {
        // Arrange
        var message = OutboxMessageDataFactory.Create(type: "TestProcessing", processedOn: null);
        await _repo.AddAsync(message);
        await SaveAndDetachAllAsync();

        // Act
        message.SetProcessed();
        await _repo.UpdateAsync(message);
        await SaveAndDetachAllAsync();

        // Assert
        var updated = await _repo.FirstOrDefaultByIdAsync(message.Id);
        updated!.ProcessedOnUtc.ShouldNotBeNull();
    }

    //-----------------------------//

    [Fact]
    public async Task OutboxMessage_WithError_ShouldStoreErrorCorrectly()
    {
        // Arrange
        var message = OutboxMessageDataFactory.Create(
            type: "ErrorEvent", 
            error: "Test error occurred during processing");

        // Act
        var result = await _repo.AddAsync(message);
        await SaveAndDetachAllAsync();

        // Assert
        var fromDb = await _repo.FirstOrDefaultByIdAsync(result.Id);
        fromDb!.Error.ShouldBe("Test error occurred during processing");
    }

    #endregion
}