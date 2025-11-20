using ID.Application.Jobs.Abstractions;
using ID.Application.Jobs.OutboxMsgs;
using ID.Domain.Repos.Specs.OutboxMsgs;
using ID.Tests.Utility.Logging;
using ID.Tests.Utility.ServiceProvider;


namespace ID.Application.Tests.Jobs.Outbox;

public class ProcessOutboxMsgJobTests : ServiceProviderTestBase
{
    private readonly Mock<IIdentityOutboxMessageRepo> _outboxRepoMock = new();
    private readonly Mock<IIdUnitOfWork> _uowMock = new();
    private readonly Mock<IPublisher> _publisherMock = new();
    private readonly Mock<ILogger<ProcessMyIdOutboxMsgJob>> _loggerMock = new();
    private readonly ProcessMyIdOutboxMsgJob _handler;

    private const int _chunkSize = 25;

    public ProcessOutboxMsgJobTests()
    {
        _publisherMock = new Mock<IPublisher>();

        _uowMock.SetupGet(u => u.OutboxMessageRepo)
            .Returns(_outboxRepoMock.Object);
        _uowMock.Setup(uow => uow.OutboxMessageRepo)
            .Returns(_outboxRepoMock.Object);
        MockServiceProvider.Setup(sp => sp.GetService(typeof(IIdUnitOfWork)))
            .Returns(_uowMock.Object);
        MockServiceProvider.Setup(sp => sp.GetService(typeof(IPublisher)))
            .Returns(_publisherMock.Object);

        _uowMock.SetupGet(u => u.OutboxMessageRepo).Returns(_outboxRepoMock.Object);
        _handler = new ProcessMyIdOutboxMsgJob(MockServiceProvider.Object, _loggerMock.Object);
    }

    //--------------------------// 

    [Fact]
    public async Task HandleAsync_Should_ProcessMessages_When_MessagesExist()
    {
        // Arrange
        var messages = IdOutboxMessageDataFactory.CreateMany(2);
        _outboxRepoMock.Setup(r => r.ListAllAsync(It.Is<UnprocessedOutboxMsgsSpec>(spc => spc.Seed == _chunkSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        // Act
        await _handler.HandleAsync();

        // Assert
        _outboxRepoMock.Verify(r => r.ListAllAsync(It.Is<UnprocessedOutboxMsgsSpec>(spc => spc.Seed == _chunkSize), It.IsAny<CancellationToken>()),
            Times.Once);
        // We can't directly verify ProcessAsync, but if no exception is thrown, the test passes for this scenario.
        _loggerMock.VerifyErrorLogging(times: () => Times.Never());

        foreach (var msg in messages)
        {
            msg.ProcessedOnUtc.ShouldNotBeNull();
        }
    }

    //--------------------------// 

    [Fact]
    public async Task HandleAsync_Should_Return_When_NoMessages()
    {
        // Arrange
        _outboxRepoMock.Setup(r => r.ListAllAsync(It.Is<UnprocessedOutboxMsgsSpec>(spc => spc.Seed == _chunkSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdOutboxMessageDataFactory.CreateMany(0));

        // Act
        await _handler.HandleAsync();

        // Assert
        _outboxRepoMock.Verify(r => r.ListAllAsync(It.Is<UnprocessedOutboxMsgsSpec>(spc => spc.Seed == _chunkSize), It.IsAny<CancellationToken>()),
            Times.Once);

        _loggerMock.VerifyErrorLogging(times: () => Times.Never());
    }

    //--------------------------// 

    [Fact]
    public async Task HandleAsync_Should_LogException_If_Thrown()
    {
        // Arrange
        var ex = new Exception("Test exception");
        _outboxRepoMock.Setup(r => r.ListAllAsync(It.Is<UnprocessedOutboxMsgsSpec>(spc => spc.Seed == _chunkSize), It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);

        // Act
        await _handler.HandleAsync();

        // Assert
        _loggerMock.VerifyExceptionLogging(IdErrorEvents.Jobs.OutboxProcessing, ex);
    }

    //------------------------------//

    [Fact]
    public void OldRefreshTokensJob_ShouldHaveDisableConcurrentExecutionAttribute()
    {
        // Arrange & Act
        var methodInfo = typeof(ProcessMyIdOutboxMsgJob).GetMethod("HandleAsync");
        var attribute = methodInfo?.GetCustomAttributes(typeof(MyIdDisableConcurrentExecutionAttribute), false).FirstOrDefault() as MyIdDisableConcurrentExecutionAttribute;

        // Assert
        attribute.ShouldNotBeNull();
        attribute.TimeoutSec.ShouldBe(300);
    }


}//Cls