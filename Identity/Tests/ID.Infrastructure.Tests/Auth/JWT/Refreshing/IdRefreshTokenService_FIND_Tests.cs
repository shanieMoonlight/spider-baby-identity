using ID.Domain.Repos.Specs.RefreshTokens;
using TestingHelpers.RandomData;
using Microsoft.AspNetCore.Identity;

namespace ID.Infrastructure.Tests.Auth.JWT.Refreshing;

public class IdRefreshTokenService_FINDING_Tests
{
    private readonly Mock<IIdUnitOfWork> _uowMock = new();
    private readonly Mock<IIdentityRefreshTokenRepo> _repoMock = new();
    private readonly Mock<IPasswordHasher<AppUser>> _pwdHasher = new();
    private JwtRefreshTokenService<AppUser> _sut;

    //------------------------------//  

    public IdRefreshTokenService_FINDING_Tests()
    {
        _uowMock.Setup(u => u.RefreshTokenRepo).Returns(_repoMock.Object);
        _pwdHasher.Setup(h => h.VerifyHashedPassword(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>())).Returns(PasswordVerificationResult.Success);
        var jwtOptions = Options.Create(new JwtOptions { RefreshTokenTimeSpan = TimeSpan.FromDays(7) });
        _sut = new JwtRefreshTokenService<AppUser>(_uowMock.Object, _pwdHasher.Object, jwtOptions);
    }

    //------------------------------//  

    [Fact]
    public async Task FindTokenWithUserAndTeamAsync_ShouldReturnToken_WhenTokenExists()
    {
        // Arrange
        var selector = "sample-selector";
        var validator = "sampleValidator";
        var clientToken = selector + "." + validator;

        var user = AppUserDataFactory.Create();
        var existingToken = CreateRefreshToken("hashed-value", user);
        existingToken.Selector = selector;

        var selectorSpec = RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec.Create(selector);

        _repoMock
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingToken); //Assume FirstOrDefaultAsync works

        // Act
        var result = await _sut.FindTokenWithUserAndDeviceAndTeamAsync(clientToken, It.IsAny<CancellationToken>());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(existingToken);

        _repoMock.Verify(
            repo => repo.FirstOrDefaultAsync(
                It.Is<RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec>(spec => spec.TESTING_CompareCriteria(selectorSpec, existingToken)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    //------------------------------//  

    [Fact]
    public async Task FindTokenWithUserAndTeamAsync_ShouldReturnNull_WhenTokenDoesNotExist()
    {
        // Arrange
        var selector = "missing-selector";
        var clientToken = selector + "." + "ignored";

        var userTeamSpec = RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec.Create(selector);
        var testToken = RefreshTokenDataFactory.Create(payload: "somehash");

        _repoMock
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdRefreshToken?)null);

        // Act
        var result = await _sut.FindTokenWithUserAndDeviceAndTeamAsync(clientToken, It.IsAny<CancellationToken>());

        // Assert
        result.ShouldBeNull();

        _repoMock.Verify(
            repo => repo.FirstOrDefaultAsync(
                It.Is<RefreshTokenBySelectorWithUserAndDeviceAndTeamSpec>(spec =>
                    spec.TESTING_GetCriteria().Compile().Invoke(testToken) == userTeamSpec.TESTING_GetCriteria().Compile().Invoke(testToken)),
                It.IsAny<CancellationToken>()),
            Times.Once);

    }

    //------------------------------//    

    private static IdRefreshToken CreateRefreshToken(string tokenPayloadValue, AppUser user) =>
        RefreshTokenDataFactory.Create(user: user, payload: tokenPayloadValue);

    //------------------------------//  
    private static IOptions<JwtOptions> CreateJwtOptions(
        int tokenExpirationMinutes = 30,
        RefreshTokenUpdatePolicy refreshTokenUpdatePolicy = RefreshTokenUpdatePolicy.ThreeQuarterLife)
    {
        var options = new JwtOptions
        {
            TokenExpirationMinutes = tokenExpirationMinutes,
            RefreshTokenUpdatePolicy = refreshTokenUpdatePolicy,
            RefreshTokenTimeSpan = TimeSpan.FromDays(2),
            SymmetricTokenSigningKey = RandomStringGenerator.Generate(64),
            TokenIssuer = "TestIssuer",
            SecurityAlgorithm = "HS256"
        };

        return Options.Create(options);
    }

    //------------------------------------//

}//Cls
