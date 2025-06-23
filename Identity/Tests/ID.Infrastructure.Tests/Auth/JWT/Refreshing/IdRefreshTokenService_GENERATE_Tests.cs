using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Refreshing;
using ID.GlobalSettings.Setup.Options;
using ID.Infrastructure.Auth.JWT.AppServiceImps;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.RefreshTokens;
using ID.Infrastructure.Tests.Auth.JWT.Utils;
using Microsoft.Extensions.Options;
using Moq;

namespace ID.Infrastructure.Tests.Auth.JWT.Refreshing;

public class IdRefreshTokenService_GENERATE_Tests
{
    private readonly Mock<IIdUnitOfWork> _uowMock;
    private readonly Mock<IIdentityRefreshTokenRepo> _refreshTokenRepoMock;
    private readonly Mock<IOptions<JwtOptions>> _optionsProviderMock;
    private readonly JwtRefreshTokenService<AppUser> _sut;

    //------------------------------//  

    public IdRefreshTokenService_GENERATE_Tests()
    {
        _refreshTokenRepoMock = new Mock<IIdentityRefreshTokenRepo>();

        _uowMock = new Mock<IIdUnitOfWork>();
        _uowMock.Setup(uow => uow.RefreshTokenRepo).Returns(_refreshTokenRepoMock.Object);

        _optionsProviderMock = new Mock<IOptions<JwtOptions>>();
        _optionsProviderMock.Setup(o => o.Value).Returns(JwtOptionsUtils.ValidOptions);
        _sut = new JwtRefreshTokenService<AppUser>(_uowMock.Object, _optionsProviderMock.Object);
    }

    //------------------------------//  

    [Fact]
    public async Task GenerateToken_ShouldCreateNewToken_WithRandomPayload()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var cancellationToken = new CancellationToken();
        IdRefreshToken capturedToken = null!;

        _refreshTokenRepoMock
            .Setup(repo => repo.AddAsync(It.IsAny<IdRefreshToken>(), cancellationToken))
            .Callback<IdRefreshToken, CancellationToken>((token, _) => capturedToken = token)
            .ReturnsAsync(capturedToken);

        // Act
        var result = await _sut.GenerateTokenAsync(user, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.User.ShouldBe(user);
        result.Payload.ShouldNotBeEmpty();

        capturedToken.ShouldNotBeNull();
        capturedToken.ShouldBe(result);

        _refreshTokenRepoMock.Verify(repo => repo.AddAsync(result, cancellationToken), Times.Once);
        _uowMock.Verify(uow => uow.SaveChangesAsync(cancellationToken), Times.Once);
    }

    //------------------------------//  
}//Cls
