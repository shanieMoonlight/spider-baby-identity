using ID.Domain.Repos.Specs.RefreshTokens;
using Microsoft.AspNetCore.Identity;

namespace ID.Infrastructure.Tests.Auth.JWT.Refreshing;

public class IdRefreshTokenService_REVOKE_Tests
{
    private readonly Mock<IIdUnitOfWork> _uowMock = new();
    private readonly Mock<IIdentityRefreshTokenRepo> _repoMock = new();
    private readonly Mock<IPasswordHasher<AppUser>> _pwdHasher = new();
    private readonly Mock<IOptions<JwtOptions>> _optionsProviderMock = new();
    private JwtRefreshTokenService<AppUser> _sut;

    //------------------------------//  

    public IdRefreshTokenService_REVOKE_Tests()
    {
        _uowMock.Setup(u => u.RefreshTokenRepo).Returns(_repoMock.Object);
        _pwdHasher.Setup(h => h.VerifyHashedPassword(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>())).Returns(PasswordVerificationResult.Success);
        _optionsProviderMock.Setup(o => o.Value).Returns(JwtOptionsUtils.ValidOptions);
        _sut = new JwtRefreshTokenService<AppUser>(_uowMock.Object, _pwdHasher.Object, _optionsProviderMock.Object);
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
        _repoMock.Verify(
            repo => repo.RemoveRangeAsync(It.Is<RefreshTokenByUserIdSpec>(spec =>
                spec.TESTING_GetCriteria().Compile().Invoke(tkn) == userIdSpec.TESTING_GetCriteria().Compile().Invoke(tkn)
            )),
            Times.Once);
        _uowMock.Verify(uow => uow.SaveChangesAsync(cancellationToken), Times.Once);
    }

    //------------------------------//  


}//Cls
