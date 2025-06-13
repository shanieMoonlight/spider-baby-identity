using ID.Application.Features.Account.Cmd.RefreshTokenRevoke;
using ID.Application.JWT;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.RefreshRevoke;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
public class RefreshTokenRevokeHandlerTests
{
    private readonly Mock<IJwtRefreshTokenService<AppUser>> _mockRefreshProvider;
    private readonly RefreshTokenRevokeHandler _handler;

    //- - - - - - - - - - - - - - - //

    public RefreshTokenRevokeHandlerTests()
    {
        _mockRefreshProvider = new Mock<IJwtRefreshTokenService<AppUser>>();
        _handler = new RefreshTokenRevokeHandler(_mockRefreshProvider.Object);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldCallRevokeTokensWithCorrectUser()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var command = new RefreshTokenRevokeCmd { PrincipalUser = user };

        _mockRefreshProvider
            .Setup(x => x.RevokeTokensAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRefreshProvider.Verify(x => x.RevokeTokensAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var command = new RefreshTokenRevokeCmd { PrincipalUser = user };

        _mockRefreshProvider
            .Setup(x => x.RevokeTokensAsync(user, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldContain(user.FriendlyName);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_WhenExceptionThrown_ShouldPropagateException()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var command = new RefreshTokenRevokeCmd { PrincipalUser = user };
        var expectedException = new InvalidOperationException("Test exception");

        _mockRefreshProvider
            .Setup(x => x.RevokeTokensAsync(user, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _handler.Handle(command, CancellationToken.None));
        
        exception.ShouldBe(expectedException);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_WithNullUser_ShouldThrowArgumentNullException()
    {
        // Arrange
        var command = new RefreshTokenRevokeCmd { PrincipalUser = null };

        // Act & Assert
        await Should.ThrowAsync<NullReferenceException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }

    //------------------------------//

}//
