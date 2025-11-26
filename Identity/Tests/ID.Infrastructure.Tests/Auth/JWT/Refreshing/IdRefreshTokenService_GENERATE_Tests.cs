using ID.Domain.Claims.AuthMethods;
using Microsoft.AspNetCore.Identity;

namespace ID.Infrastructure.Tests.Auth.JWT.Refreshing;

public class IdRefreshTokenService_GENERATE_Tests
{
    private readonly Mock<IIdUnitOfWork> _uowMock;
    private readonly Mock<IIdentityRefreshTokenRepo> _refreshTokenRepoMock;
    private readonly Mock<IOptions<JwtOptions>> _optionsProviderMock;
    private readonly Mock<IPasswordHasher<AppUser>> _pwdHasherMock;
    private readonly JwtRefreshTokenService<AppUser> _sut;

    //------------------------------//  

    public IdRefreshTokenService_GENERATE_Tests()
    {
        _refreshTokenRepoMock = new Mock<IIdentityRefreshTokenRepo>();

        _uowMock = new Mock<IIdUnitOfWork>();
        _uowMock.Setup(uow => uow.RefreshTokenRepo).Returns(_refreshTokenRepoMock.Object);

        _optionsProviderMock = new Mock<IOptions<JwtOptions>>();
        _optionsProviderMock.Setup(o => o.Value).Returns(JwtOptionsUtils.ValidOptions);

        _pwdHasherMock = new Mock<IPasswordHasher<AppUser>>();
        _pwdHasherMock.Setup(h => h.HashPassword(It.IsAny<AppUser>(), It.IsAny<string>())).Returns("hashed");

        _sut = new JwtRefreshTokenService<AppUser>(_uowMock.Object, _pwdHasherMock.Object, _optionsProviderMock.Object);
    }

    //------------------------------//  

    [Fact]
    public async Task GenerateToken_ShouldCreateNewToken_WithRandomPayload()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var cancellationToken = new CancellationToken();
        IdRefreshToken capturedToken = null!;
        var authMethods = new List<AuthMethodRef> { AuthMethodRef.pwd, AuthMethodRef.mfa };

        _refreshTokenRepoMock
            .Setup(repo => repo.AddAsync(It.IsAny<IdRefreshToken>(), cancellationToken))
            .Callback<IdRefreshToken, CancellationToken>((token, _) => capturedToken = token)
            .ReturnsAsync((IdRefreshToken t, CancellationToken ct) => t);

        // Act
        var result = await _sut.GenerateAndStoreTokenAsync(user, authMethods, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.RefreshToken.ShouldNotBeNull();
        result.RefreshToken.User.ShouldBe(user);
        result.ClientToken.ShouldNotBeNullOrEmpty();

        capturedToken.ShouldNotBeNull();
        capturedToken.ShouldBe(result.RefreshToken);

        _refreshTokenRepoMock.Verify(repo => repo.AddAsync(It.IsAny<IdRefreshToken>(), cancellationToken), Times.Once);
        _uowMock.Verify(uow => uow.SaveChangesAsync(cancellationToken), Times.Once);
    }

}//Cls
