using ID.Domain.Entities.TrustedDevices;
using ID.Domain.Repos.Specs.TrustedDevices;
using ID.Domain.Repos.Transactions;
using ID.Tests.Utility.ServiceProvider;

namespace ID.Application.Tests.Jobs.DbMntc.TrustedDevices;

public class ExpiredTrustedDevicesJobTests : ServiceProviderTestBase
{
    private readonly Mock<IIdentityTrustedDeviceRepo> _repoMock;
    private readonly Mock<IIdentityTransactionService> _transactionServiceMock;
    private readonly Mock<IIdExecutionStrategy> _executionStrategyMock;
    private readonly Mock<IIdTransaction> _transactionMock;
    private readonly Mock<ILogger<ExpiredTrustedDevicesJob>> _loggerMock;
    private readonly ExpiredTrustedDevicesJob _job;

    public ExpiredTrustedDevicesJobTests()
    {
        _repoMock = new Mock<IIdentityTrustedDeviceRepo>();
        _transactionServiceMock = new Mock<IIdentityTransactionService>();
        _executionStrategyMock = new Mock<IIdExecutionStrategy>();
        _transactionMock = new Mock<IIdTransaction>();
        _loggerMock = new Mock<ILogger<ExpiredTrustedDevicesJob>>();

        // Setup service provider scope to return our mocks
        MockServiceProvider.Setup(sp => sp.GetService(typeof(IIdentityTrustedDeviceRepo))).Returns(_repoMock.Object);
        MockServiceProvider.Setup(sp => sp.GetService(typeof(IIdentityTransactionService))).Returns(_transactionServiceMock.Object);

        // Execution strategy
        _transactionServiceMock.Setup(t => t.CreateExecutionStrategyAsync()).ReturnsAsync(_executionStrategyMock.Object);
        _executionStrategyMock.Setup(es => es.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>(async (action, ct) => { await action(ct); });

        // Transaction
        _transactionServiceMock.Setup(t => t.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_transactionMock.Object);
        _transactionServiceMock.Setup(t => t.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Create job
        _job = new ExpiredTrustedDevicesJob(MockServiceProvider.Object, _loggerMock.Object);
    }

    //--------------------//

    [Fact]
    public async Task HandleAsync_WhenNoExpiredDevices_ShouldDoNothing()
    {
        // Arrange
        _repoMock.Setup(r => r.ListAllTrackedAsync(It.IsAny<TrustedDevicesExpiredSpec>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<TrustedDevice>());

        // Act
        await _job.HandleAsync();

        // Assert
        _repoMock.Verify(r => r.ListAllTrackedAsync(It.IsAny<TrustedDevicesExpiredSpec>(), It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.RemoveRangeAsync(It.IsAny<IEnumerable<TrustedDevice>>()), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    //--------------------//

    [Fact]
    public async Task HandleAsync_WhenExpiredDevicesExist_ShouldRemoveInBatchesAndCommit()
    {
        // Arrange
        var devices = new List<TrustedDevice>();
        for (int i = 0; i < 120; i++) devices.Add(TrustedDeviceDataFactory.Create());
        _repoMock.Setup(r => r.ListAllTrackedAsync(It.IsAny<TrustedDevicesExpiredSpec>(), It.IsAny<CancellationToken>())).ReturnsAsync(devices);

        // Act
        await _job.HandleAsync();

        // Assert - 120 / 50 => 3 batches
        _repoMock.Verify(r => r.RemoveRangeAsync(It.IsAny<IEnumerable<TrustedDevice>>()), Times.Exactly(3));
        _transactionServiceMock.Verify(t => t.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    //--------------------//

    [Fact]
    public async Task HandleAsync_WhenRepoThrows_ShouldRollbackAndLog()
    {
        // Arrange
        _repoMock.Setup(r => r.ListAllTrackedAsync(It.IsAny<TrustedDevicesExpiredSpec>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("boom"));

        // Act
        await _job.HandleAsync();

        // Assert
        _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), Times.AtLeastOnce);
    }
}
