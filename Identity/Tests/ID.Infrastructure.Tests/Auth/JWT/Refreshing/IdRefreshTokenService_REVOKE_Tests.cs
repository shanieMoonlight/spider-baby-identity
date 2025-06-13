using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.AppUsers;
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

public class IdRefreshTokenService_REVOKE_Tests
{
    private readonly Mock<IIdUnitOfWork> _uowMock;
    private readonly Mock<IIdentityRefreshTokenRepo> _refreshTokenRepoMock;
    private readonly Mock<IOptions<JwtOptions>> _optionsProviderMock;
    private readonly JwtRefreshTokenService<AppUser> _sut;

    //------------------------------//  

    public IdRefreshTokenService_REVOKE_Tests()
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
    public async Task RevokeTokens_ShouldRemoveAllUserTokens_AndSaveChanges()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var cancellationToken = new CancellationToken();

        var tkn = RefreshTokenDataFactory.Create(user: user);
        var userIdSpec = RefreshTokenByUserIdSpec.Create(user);

        // Act
        await _sut.RevokeTokensAsync(user, cancellationToken);

        // Assert
        _refreshTokenRepoMock.Verify(
            repo => repo.RemoveRangeAsync(It.Is<RefreshTokenByUserIdSpec>(spec =>
                spec.TESTING_GetCriteria().Compile().Invoke(tkn) == userIdSpec.TESTING_GetCriteria().Compile().Invoke(tkn)
            )),
            Times.Once);
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


}//Cls
