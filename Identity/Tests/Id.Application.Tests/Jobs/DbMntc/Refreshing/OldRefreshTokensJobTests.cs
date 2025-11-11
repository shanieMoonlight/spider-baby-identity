using ID.Application.Jobs.Abstractions;
using ID.Tests.Utility.ServiceProvider;

namespace ID.Application.Tests.Jobs.DbMntc.Refreshing;

public class OldRefreshTokensJobTests : ServiceProviderTestBase
{
    private readonly Mock<IIdUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IIdentityRefreshTokenRepo> _mockRefreshTokenRepo;
    private readonly Mock<ILogger<OldRefreshTokensJob>> _mockLogger;
    private readonly OldRefreshTokensJob _sut;

    public OldRefreshTokensJobTests()
    {
        _mockUnitOfWork = new Mock<IIdUnitOfWork>();
        _mockRefreshTokenRepo = new Mock<IIdentityRefreshTokenRepo>();
        _mockLogger = new Mock<ILogger<OldRefreshTokensJob>>();

        MockServiceProvider.Setup(sp => sp.GetService(typeof(IIdUnitOfWork)))
            .Returns(_mockUnitOfWork.Object);
        _mockUnitOfWork.Setup(uow => uow.RefreshTokenRepo)
            .Returns(_mockRefreshTokenRepo.Object);

        _sut = new OldRefreshTokensJob(MockServiceProvider.Object, _mockLogger.Object);
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
        ExceptionUtils.VerifyExceptionLogging(_mockLogger, IdErrorEvents.Jobs.DbMntc, expectedException);

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
        var attribute = methodInfo?.GetCustomAttributes(typeof(MyIdDisableConcurrentExecutionAttribute), false).FirstOrDefault() as MyIdDisableConcurrentExecutionAttribute;

        // Assert
        attribute.ShouldNotBeNull();
        attribute.TimeoutSec.ShouldBe(300);
    }

}
