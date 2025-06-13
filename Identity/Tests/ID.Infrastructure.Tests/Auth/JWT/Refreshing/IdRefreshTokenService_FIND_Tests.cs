using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Refreshing;
using ID.Infrastructure.Auth.JWT.AppServiceImps;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.RefreshTokens;
using Microsoft.Extensions.Options;
using Moq;
using TestingHelpers;

namespace ID.Infrastructure.Tests.Auth.JWT.Refreshing;

public class IdRefreshTokenService_FINDING_Tests
{
    private readonly Mock<IIdUnitOfWork> _uowMock;
    private readonly Mock<IIdentityRefreshTokenRepo> _refreshTokenRepoMock;
    //private readonly Mock<IOptions<JwtOptions>> _optionsProviderMock;
    private readonly JwtRefreshTokenService<AppUser> _sut;

    //------------------------------//  

    public IdRefreshTokenService_FINDING_Tests()
    {
        _refreshTokenRepoMock = new Mock<IIdentityRefreshTokenRepo>();

        _uowMock = new Mock<IIdUnitOfWork>();
        _uowMock.Setup(uow => uow.RefreshTokenRepo).Returns(_refreshTokenRepoMock.Object);


        var jwtOptions = CreateJwtOptions();
        //_optionsProviderMock = new Mock<IOptions<JwtOptions>>();
        //_optionsProviderMock.Setup(o => o.Value).Returns(jwtOptions);
        _sut = new JwtRefreshTokenService<AppUser>(_uowMock.Object, jwtOptions);
    }

    //------------------------------//  

    [Fact]
    public async Task FindTokenWithUserAndTeamAsync_ShouldReturnToken_WhenTokenExists()
    {
        // Arrange
        var tokenPayload = "test-token-payload";
        var user = AppUserDataFactory.Create();
        var existingToken = CreateRefreshToken(tokenPayload, user);

        var userTeamSpec = RefreshTokenWithUserAndTeamSpec.Create(tokenPayload);

        _refreshTokenRepoMock
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<RefreshTokenWithUserAndTeamSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingToken); //Assume FirstOrDefaultAsync works

        // Act
        var result = await _sut.FindTokenWithUserAndTeamAsync(tokenPayload, It.IsAny<CancellationToken>());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(existingToken);


        _refreshTokenRepoMock.Verify(
            repo => repo.FirstOrDefaultAsync(
                It.Is<RefreshTokenWithUserAndTeamSpec>(spec => spec.TESTING_CompareCriteria(userTeamSpec, existingToken)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    //------------------------------//  

    [Fact]
    public async Task FindTokenWithUserAndTeamAsync_ShouldReturnNull_WhenTokenDoesNotExist()
    {
        // Arrange
        var tokenPayload = "non-existent-token";
        var userTeamSpec = RefreshTokenWithUserAndTeamSpec.Create(tokenPayload);
        var testToken = RefreshTokenDataFactory.Create(payload: tokenPayload);

        _refreshTokenRepoMock
            .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<RefreshTokenWithUserAndTeamSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdRefreshToken?)null);

        // Act
        var result = await _sut.FindTokenWithUserAndTeamAsync(tokenPayload, It.IsAny<CancellationToken>());

        // Assert
        result.ShouldBeNull();

        _refreshTokenRepoMock.Verify(
            repo => repo.FirstOrDefaultAsync(
                It.Is<RefreshTokenWithUserAndTeamSpec>(spec =>
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
