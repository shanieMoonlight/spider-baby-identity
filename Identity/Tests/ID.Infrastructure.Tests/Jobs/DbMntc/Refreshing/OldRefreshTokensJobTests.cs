using Hangfire;
using Id.Tests.Utility.Exceptions;
using ID.Application.Utility;
using ID.Infrastructure.Jobs.Imps.DbMntc.Jobs;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.RefreshTokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace ID.Infrastructure.Tests.Jobs.DbMntc.Refreshing;

public class OldRefreshTokensJobTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IServiceScope> _mockServiceScope;
    private readonly Mock<IServiceScopeFactory> _mockServiceScopeFactory;
    private readonly Mock<IIdUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IIdentityRefreshTokenRepo> _mockRefreshTokenRepo;
    private readonly Mock<ILogger<OldRefreshTokensJob>> _mockLogger;
    private readonly OldRefreshTokensJob _sut;

    public OldRefreshTokensJobTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceScope = new Mock<IServiceScope>();
        _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        _mockUnitOfWork = new Mock<IIdUnitOfWork>();
        _mockRefreshTokenRepo = new Mock<IIdentityRefreshTokenRepo>();
        _mockLogger = new Mock<ILogger<OldRefreshTokensJob>>();

        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
            .Returns(_mockServiceScopeFactory.Object);
        _mockServiceScopeFactory.Setup(f => f.CreateScope())
            .Returns(_mockServiceScope.Object);
        _mockServiceScope.Setup(s => s.ServiceProvider)
            .Returns(_mockServiceProvider.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IIdUnitOfWork)))
            .Returns(_mockUnitOfWork.Object);
        _mockUnitOfWork.Setup(uow => uow.RefreshTokenRepo)
            .Returns(_mockRefreshTokenRepo.Object);

        _sut = new OldRefreshTokensJob(_mockServiceProvider.Object, _mockLogger.Object);
    }

    //------------------------------//

    [Fact]
    public async Task HandleAsync_ShouldUseExpiredRefreshTokensSpec()
    {
        // Arrange
        _mockRefreshTokenRepo.Setup(repo => repo.RemoveRangeAsync(
                It.IsAny<RefreshTokenExpiredSpec>()));

        // Act
        await _sut.HandleAsync(CancellationToken.None);

        // Assert
        _mockRefreshTokenRepo.Verify(repo => repo.RemoveRangeAsync(
            It.Is<RefreshTokenExpiredSpec>(spec => spec != null)),
            Times.Once);
    }

    //------------------------------//

    [Fact]
    public async Task HandleAsync_ShouldLogException_WhenErrorOccurs()
    {
        // Arrange
        var expectedException = new Exception("Test exception");

        _mockRefreshTokenRepo.Setup(repo => repo.RemoveRangeAsync(
                It.IsAny<RefreshTokenExpiredSpec>()))
            .ThrowsAsync(expectedException);

        // Act
        await _sut.HandleAsync(CancellationToken.None);

        // Assert
        ExceptionUtils.VerifyExceptionLogging(_mockLogger, MyIdLoggingEvents.JOBS.DB_MNTC, expectedException);

    }

    //------------------------------//

    [Fact]
    public async Task HandleAsync_ShouldNotCallSaveChanges_WhenExceptionOccurs()
    {
        // Arrange
        _mockRefreshTokenRepo.Setup(repo => repo.RemoveRangeAsync(
                It.IsAny<RefreshTokenExpiredSpec>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        await _sut.HandleAsync(CancellationToken.None);

        // Assert
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------//

    [Fact]
    public void OldRefreshTokensJob_ShouldHaveDisableConcurrentExecutionAttribute()
    {
        // Arrange & Act
        var methodInfo = typeof(OldRefreshTokensJob).GetMethod("HandleAsync");
        var attribute = methodInfo?.GetCustomAttributes(typeof(DisableConcurrentExecutionAttribute), false).FirstOrDefault() as DisableConcurrentExecutionAttribute;

        // Assert
        attribute.ShouldNotBeNull();
        attribute.TimeoutSec.ShouldBe(300);
    }

    //------------------------------//
}
