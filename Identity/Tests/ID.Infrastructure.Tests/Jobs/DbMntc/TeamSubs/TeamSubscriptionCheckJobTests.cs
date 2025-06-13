using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Abstractions.Services.Transactions;
using ID.Domain.Entities.AppUsers;
using ID.Infrastructure.Jobs.Imps.DbMntc.Jobs;
using Microsoft.Extensions.Logging;
using Moq;

namespace ID.Infrastructure.Tests.Jobs.DbMntc.TeamSubs;

public class TeamSubscriptionCheckJobTests : ServiceProviderTestBase
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamMgrMock;
    private readonly Mock<IIdentityTransactionService> _transactionServiceMock;
    private readonly Mock<ILogger<TeamSubscriptionCheckJob>> _loggerMock;
    private readonly TeamSubscriptionCheckJob _job;

    //- - - - - - - - - - - - - - - - - - //

    public TeamSubscriptionCheckJobTests()
    {
        _teamMgrMock = new Mock<IIdentityTeamManager<AppUser>>();
        _transactionServiceMock = new Mock<IIdentityTransactionService>();
        _loggerMock = new Mock<ILogger<TeamSubscriptionCheckJob>>();

        MockServiceProvider.Setup(sp => sp.GetService(typeof(IIdentityTeamManager<AppUser>))).Returns(_teamMgrMock.Object);
        MockServiceProvider.Setup(sp => sp.GetService(typeof(IIdentityTransactionService))).Returns(_transactionServiceMock.Object);

        _job = new TeamSubscriptionCheckJob(MockServiceProvider.Object, _loggerMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task HandleAsync_Should_Call_GetAllTeamsWithExpiredSubscriptions()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var transactionMock = new Mock<IIdTransaction>();
        _transactionServiceMock.Setup(m => m.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(transactionMock.Object);
        _teamMgrMock.Setup(m => m.GetAllTeamsWithExpiredSubscriptions(cancellationToken))
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
        _transactionServiceMock.Setup(m => m.BeginTransactionAsync(cancellationToken))
            .ReturnsAsync(transactionMock.Object);
        _teamMgrMock.Setup(m => m.GetAllTeamsWithExpiredSubscriptions(cancellationToken))
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

    //------------------------------------//

}
