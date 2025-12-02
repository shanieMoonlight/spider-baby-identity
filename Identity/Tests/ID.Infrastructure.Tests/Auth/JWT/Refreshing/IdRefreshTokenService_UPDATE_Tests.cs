using ID.Domain.Entities.Refreshing.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace ID.Infrastructure.Tests.Auth.JWT.Refreshing;

public class IdRefreshTokenService_UPDATE_Tests
{
    private readonly Mock<IIdUnitOfWork> _uowMock = new();
    private readonly Mock<IIdentityRefreshTokenRepo> _repoMock = new();
    private readonly Mock<IPasswordHasher<AppUser>> _pwdHasher = new();
    private readonly Mock<IOptions<JwtOptions>> _optionsProviderMock = new();
    private readonly JwtRefreshTokenService<AppUser> _sut;

    //------------------------------//  

    public IdRefreshTokenService_UPDATE_Tests()
    {
        var jwtOptions = new JwtOptions { RefreshTokenTimeSpan = TimeSpan.FromDays(7) };

        _uowMock.Setup(u => u.RefreshTokenRepo).Returns(_repoMock.Object);
        _optionsProviderMock.Setup(o => o.Value).Returns(jwtOptions);
        _pwdHasher.Setup(h => h.HashPassword(It.IsAny<AppUser>(), It.IsAny<string>())).Returns("hashed");
        _pwdHasher.Setup(h => h.VerifyHashedPassword(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>())).Returns(PasswordVerificationResult.Success);
        _sut = new JwtRefreshTokenService<AppUser>(_uowMock.Object, _pwdHasher.Object, _optionsProviderMock.Object);
    }

    //------------------------------//  

    [Fact]
    public async Task UpdateTokenPayloadAsync_ShouldUpdateExistingToken_WithNewPayload_AndDifferentExpiration()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var originalToken = CreateRefreshToken("original-token-payload", user);
        var newTokenPayloadVo = TokenPayloadHash.Create("new-token-payload");
        var cancellationToken = new CancellationToken();

        // Usage example:
        var originalDate = originalToken.ExpiresOnUtc;

        _repoMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<IdRefreshToken>()))
            .ReturnsAsync((IdRefreshToken)null!); // Mock UpdateAsync behavior

        // Act
        var updatedDto = await _sut.UpdateTokenPayloadAsync(originalToken, cancellationToken);

        // Assert
        updatedDto.ShouldNotBeNull();
        updatedDto.RefreshToken.ShouldBe(originalToken);
        updatedDto.ClientToken.ShouldNotBeNullOrEmpty();
        // Payload was renamed to PayloadHash
        updatedDto.RefreshToken.PayloadHash.ShouldNotBeNullOrEmpty();
        updatedDto.RefreshToken.ExpiresOnUtc.ShouldNotBe(originalDate);

        _repoMock.Verify(repo => repo.UpdateAsync(originalToken), Times.Once);
        _uowMock.Verify(uow => uow.SaveChangesAsync(cancellationToken), Times.Once);
    }

    //------------------------------//  

 

    private static IdRefreshToken CreateRefreshToken(string tokenPayloadValue, AppUser user) =>
        RefreshTokenDataFactory.Create(user: user, payload: tokenPayloadValue);

    //------------------------------//  
}//Cls
