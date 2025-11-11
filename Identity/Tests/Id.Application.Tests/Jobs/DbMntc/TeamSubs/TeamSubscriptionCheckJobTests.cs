using ID.Application.Jobs.Abstractions;
using ID.Domain.Repos.Transactions;
using ID.Tests.Utility.ServiceProvider;

namespace ID.Application.Tests.Jobs.DbMntc.TeamSubs;

public class TeamSubscriptionCheckJobTests : ServiceProviderTestBase
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamMgrMock;
    private readonly Mock<IIdentityTransactionService> _transactionServiceMock;
    private readonly Mock<IIdExecutionStrategy> _executionStrategyMock;
    private readonly Mock<ILogger<TeamSubscriptionCheckJob>> _loggerMock;
    private readonly TeamSubscriptionCheckJob _job;

    //- - - - - - - - - - - - - - - - - - //

    public TeamSubscriptionCheckJobTests()
    {
        _teamMgrMock = new Mock<IIdentityTeamManager<AppUser>>();
        _executionStrategyMock = new Mock<IIdExecutionStrategy>();
        _transactionServiceMock = new Mock<IIdentityTransactionService>();
        _loggerMock = new Mock<ILogger<TeamSubscriptionCheckJob>>();

        MockServiceProvider.Setup(sp => sp.GetService(typeof(IIdentityTeamManager<AppUser>)))
            .Returns(_teamMgrMock.Object);
        MockServiceProvider.Setup(sp => sp.GetService(typeof(IIdentityTransactionService)))
            .Returns(_transactionServiceMock.Object);

        _job = new TeamSubscriptionCheckJob(MockServiceProvider.Object, _loggerMock.Object);
    }


    //------------------------------------//


    [Fact]
    public async Task HandleAsync_Should_Call_GetAllTeamsWithExpiredSubscriptions()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var transactionMock = new Mock<IIdTransaction>();

        // Ensure transaction service returns the execution strategy and that the strategy executes the provided delegate
        _transactionServiceMock.Setup(m => m.CreateExecutionStrategyAsync())
            .ReturnsAsync(_executionStrategyMock.Object);

        _executionStrategyMock
            .Setup(s => s.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<CancellationToken, Task> op, CancellationToken ct) => op(ct));

        _transactionServiceMock.Setup(m => m.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionMock.Object);

        _teamMgrMock.Setup(m => m.GetAllTeamsWithExpiredSubscriptions(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _job.HandleAsync(cancellationToken);

        // Assert
        _teamMgrMock.Verify(m => m.GetAllTeamsWithExpiredSubscriptions(cancellationToken), Times.Once);
        transactionMock.Verify(t => t.CommitAsync(cancellationToken), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleAsync_Should_LogException_And_RollbackTransaction_On_Exception()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var transactionMock = new Mock<IIdTransaction>();

        _transactionServiceMock.Setup(m => m.CreateExecutionStrategyAsync())
            .ReturnsAsync(_executionStrategyMock.Object);

        _executionStrategyMock
            .Setup(s => s.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<CancellationToken, Task> op, CancellationToken ct) => op(ct));

        _transactionServiceMock.Setup(m => m.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionMock.Object);
        _teamMgrMock.Setup(m => m.GetAllTeamsWithExpiredSubscriptions(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        await _job.HandleAsync(cancellationToken);

        // Assert
        transactionMock.Verify(t => t.RollbackAsync(cancellationToken), Times.Once);
        _loggerMock.Verify(l => l.Log(
            It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
    }

    //------------------------------//

    [Fact]
    public void OldRefreshTokensJob_ShouldHaveDisableConcurrentExecutionAttribute()
    {
        // Arrange & Act
        var methodInfo = typeof(TeamSubscriptionCheckJob).GetMethod("HandleAsync");
        var attribute = methodInfo?.GetCustomAttributes(typeof(MyIdDisableConcurrentExecutionAttribute), false).FirstOrDefault() as MyIdDisableConcurrentExecutionAttribute;

        // Assert
        attribute.ShouldNotBeNull();
        attribute.TimeoutSec.ShouldBe(300);
    }



}//Cls
