using ID.Application.Jobs.OutboxMsgs;
using ID.Domain.Repos.Specs.NewFolder.OutboxMsgs;
using ID.Tests.Utility.Logging;
using ID.Tests.Utility.ServiceProvider;

namespace ID.Application.Tests.Jobs.Outbox;

public class ProcessOldOutboxMsgsTests : ServiceProviderTestBase
{
    private readonly Mock<IIdentityOutboxMessageRepo> _outboxRepoMock = new();
    private readonly Mock<IIdUnitOfWork> _uowMock = new();
    private readonly Mock<ILogger<Process_Old_MyIdOutboxMsgs>> _loggerMock = new();
    private readonly Process_Old_MyIdOutboxMsgs _handler;

    public ProcessOldOutboxMsgsTests()
    {
        _uowMock.SetupGet(u => u.OutboxMessageRepo).Returns(_outboxRepoMock.Object);

        MockServiceProvider.Setup(sp => sp.GetService(typeof(IIdUnitOfWork)))
            .Returns(_uowMock.Object);
        _uowMock.Setup(uow => uow.OutboxMessageRepo)
            .Returns(_outboxRepoMock.Object);


        _handler = new Process_Old_MyIdOutboxMsgs(MockServiceProvider.Object, _loggerMock.Object);
    }

    //--------------------------// 

    [Fact]
    public async Task HandleAsync_Should_RemoveCompletedAndCallSaveChanges()
    {
        // Arrange
        var completed = IdOutboxMessageDataFactory.CreateMany(2);

        _outboxRepoMock.Setup(r => r.RemoveRangeAsync(It.Is<OutboxMsgsCompletedOlderThanSpec>(s => s.Seed == 14)));

        // Act
        await _handler.HandleAsync();

        // Assert
        _outboxRepoMock.Verify(r => r.RemoveRangeAsync(It.Is<OutboxMsgsCompletedOlderThanSpec>(s => s.Seed == 14)), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.VerifyErrorLogging(times: () => Times.Never());
    }

    //--------------------------// 

    [Fact]
    public async Task HandleAsync_Should_LogException_If_Thrown()
    {
        // Arrange
        var ex = new Exception("Test exception");

        _outboxRepoMock.Setup(r => r.RemoveRangeAsync(It.Is<OutboxMsgsCompletedOlderThanSpec>(s => s.Seed == 14)))
            .ThrowsAsync(ex);

        // Act
        await _handler.HandleAsync();

        // Assert
        _loggerMock.VerifyExceptionLogging(IdErrorEvents.Jobs.OutboxProcessing, ex);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    //--------------------------// 

    [Fact]
    public async Task HandleAsync_Should_Not_LogError_If_No_Exception()
    {
        // Arrange
        _outboxRepoMock.Setup(r => r.ListAllAsync(It.Is<OutboxMsgsCompletedOlderThanSpec>(s => s.Seed == 14), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdOutboxMessageDataFactory.CreateMany(0));

        // Act
        await _handler.HandleAsync();

        // Assert
        _loggerMock.VerifyErrorLogging(times: () => Times.Never());
    }
}