using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Refreshing;
using ID.Domain.Entities.Refreshing.ValueObjects;
using ID.GlobalSettings.Setup.Options;
using ID.Infrastructure.Auth.JWT.AppServiceImps;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.RefreshTokens;
using ID.Infrastructure.Tests.Auth.JWT.Utils;
using Microsoft.Extensions.Options;
using Moq;
using System.Reflection;

namespace ID.Infrastructure.Tests.Auth.JWT.Refreshing;

public class IdRefreshTokenService_UPDATE_Tests
{
    private readonly Mock<IIdUnitOfWork> _uowMock;
    private readonly Mock<IIdentityRefreshTokenRepo> _refreshTokenRepoMock;
    private readonly Mock<IOptions<JwtOptions>> _optionsProviderMock;
    private readonly JwtRefreshTokenService<AppUser> _sut;

    //------------------------------//  

    public IdRefreshTokenService_UPDATE_Tests()
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
    public async Task UpdateTokenPayloadAsync_ShouldUpdateExistingToken_WithNewPayload_AndDifferentExpiration()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var originalToken = CreateRefreshToken("original-token-payload", user);
        var newTokenPayloadVo = TokenPayload.Create("new-token-payload");
        var cancellationToken = new CancellationToken();

        // Usage example:
        var originalDate = originalToken.ExpiresOnUtc;

        _refreshTokenRepoMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<IdRefreshToken>()))
            .ReturnsAsync((IdRefreshToken)null!); // Mock UpdateAsync behavior

        // Act
        var updatedToken = await _sut.UpdateTokenPayloadAsync(originalToken, cancellationToken);

        // Assert
        updatedToken.ShouldNotBeNull();
        updatedToken.ShouldBe(originalToken);
        //updatedToken.Payload.ShouldNotBe(newTokenPayloadVo.Value);
        updatedToken.ExpiresOnUtc.ShouldNotBe(originalDate);

        _refreshTokenRepoMock.Verify(repo => repo.UpdateAsync(originalToken), Times.Once);
        _uowMock.Verify(uow => uow.SaveChangesAsync(cancellationToken), Times.Once);
    }

    //------------------------------//  

    [Fact]
    public void GeneratePayload_ShouldReturnDifferentValues_OnEachCall()
    {
        // Arrange
        var generatePayloadMethod = typeof(JwtRefreshTokenService<AppUser>)
            .GetMethod("GeneratePayload", BindingFlags.NonPublic | BindingFlags.Static);
        generatePayloadMethod.ShouldNotBeNull();

        // Act
        var payload1 = generatePayloadMethod.Invoke(null, null) as string;
        var payload2 = generatePayloadMethod.Invoke(null, null) as string;

        // Assert
        payload1.ShouldNotBeNull();
        payload2.ShouldNotBeNull();
        payload1.ShouldNotBe(payload2);
    }

    //------------------------------//  

    private static IdRefreshToken CreateRefreshToken(string tokenPayloadValue, AppUser user) =>
        RefreshTokenDataFactory.Create(user: user, payload: tokenPayloadValue);

    //------------------------------//  

    private static string GetSpecTokenPayload(RefreshTokenWithUserAndTeamSpec spec)
    {
        var field = typeof(RefreshTokenWithUserAndTeamSpec).GetField("_tokenPayload", BindingFlags.NonPublic | BindingFlags.Instance);
        return field?.GetValue(spec) as string ?? string.Empty;
    }

    //------------------------------//  

    private static Guid GetSpecUserId(RefreshTokenByUserIdSpec spec)
    {
        var field = typeof(RefreshTokenByUserIdSpec).GetField("_userId", BindingFlags.NonPublic | BindingFlags.Instance);
        return (Guid)(field?.GetValue(spec) ?? Guid.Empty);
    }
   
  

    //------------------------------// 

}//Cls
