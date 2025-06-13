using ID.Application.JWT;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Refreshing;
using ID.Domain.Models;
using ID.GlobalSettings.Setup.Options;
using ID.Infrastructure.Auth.JWT.AppServiceImps;
using ID.Infrastructure.Auth.JWT.LocalServices.Abs;
using ID.Infrastructure.Auth.JWT.Setup;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Options;
using Moq;
using TestingHelpers;
using Xunit.Abstractions;

namespace ID.Infrastructure.Tests.Auth.JWT;


public class JwtPackageProviderTests
{
    private readonly Mock<IJwtBuilder> _jwtBuilderMock = new();
    private readonly Mock<IJwtRefreshTokenService<AppUser>> _refreshTokenServiceMock = new();
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptionsProvider = new();
    private readonly ITestOutputHelper _output;

    public JwtPackageProviderTests(ITestOutputHelper output)
    {
        _output = output;
        _mockGlobalOptionsProvider.Setup(x => x.Value).Returns(GlobalOptionsUtils.ValidOptions);
    }

    //------------------------------------//

    [Fact]
    public async Task CreateJwtPackageWithTwoFactorRequiredAsync_Should_Return_JwtPackage_With_TwoFactorRequired()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(teamId);
        var team = TeamDataFactory.Create(id: teamId);
        var provider = TwoFactorProvider.Email;
        var extraInfo = "Test extra info";
        var deviceId = "device123";
        var encodedToken = "encoded.jwt.token";

        _jwtBuilderMock
            .Setup(x => x.CreateJwtWithTwoFactorRequiredAsync(user, team, deviceId))
            .ReturnsAsync(encodedToken);

        var jwtOptions = CreateJwtOptions();
        var provider_sut = new JwtPackageProvider(
            _jwtBuilderMock.Object,
            _refreshTokenServiceMock.Object,
            jwtOptions,
            _mockGlobalOptionsProvider.Object);

        // Act
        var result = await provider_sut.CreateJwtPackageWithTwoFactorRequiredAsync(
            user, team, provider, extraInfo, deviceId);

        // Assert
        result.ShouldNotBeNull();
        result.AccessToken.ShouldBe(encodedToken);
        result.TwoStepVerificationRequired.ShouldBeTrue();
        result.TwoFactorProvider.ShouldBe(provider);
        result.RefreshToken.ShouldBeNull();

        _jwtBuilderMock.Verify(x => x.CreateJwtWithTwoFactorRequiredAsync(user, team, deviceId), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task CreateJwtPackageAsync_Should_Return_JwtPackage_With_RefreshToken_When_Eligible()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(teamId);
        user.TwoFactorEnabled = false; // No 2FA required
        var team = TeamDataFactory.Create(id: teamId);
        var twoFactorVerified = false; // Not needed since 2FA disabled
        var deviceId = "device123";
        var encodedToken = "encoded.jwt.token";
        var refreshToken = RefreshTokenDataFactory.Create();

        _jwtBuilderMock
            .Setup(x => x.CreateJwtAsync(user, team, twoFactorVerified, deviceId))
            .ReturnsAsync(encodedToken);

        _refreshTokenServiceMock
            .Setup(x => x.GenerateTokenAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        var globalOptions = GlobalOptionsUtils.ValidOptions;
        globalOptions.JwtRefreshTokensEnabled = true;
        _mockGlobalOptionsProvider.Setup(x => x.Value).Returns(globalOptions);

        var jwtOptions = CreateJwtOptions();
        var provider_sut = new JwtPackageProvider(
            _jwtBuilderMock.Object,
            _refreshTokenServiceMock.Object,
            jwtOptions,
            _mockGlobalOptionsProvider.Object);

        // Act
        var result = await provider_sut.CreateJwtPackageAsync(user, team, twoFactorVerified, deviceId);

        // Assert
        result.ShouldNotBeNull();
        result.AccessToken.ShouldBe(encodedToken);
        result.TwoStepVerificationRequired.ShouldBeFalse();
        result.RefreshToken.ShouldBe(refreshToken.Payload);
        result.TwoFactorProvider.ShouldBe(user.TwoFactorProvider);

        _jwtBuilderMock.Verify(x => x.CreateJwtAsync(user, team, twoFactorVerified, deviceId), Times.Once);
        _refreshTokenServiceMock.Verify(x => x.GenerateTokenAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task CreateJwtPackageAsync_Should_Not_Generate_RefreshToken_When_TwoFactor_Not_Verified()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(teamId);
        user.TwoFactorEnabled = true; // 2FA enabled
        var team = TeamDataFactory.Create(id: teamId);
        var twoFactorVerified = false; // But not verified yet
        var deviceId = "device123";
        var encodedToken = "encoded.jwt.token";

        _jwtBuilderMock
            .Setup(x => x.CreateJwtAsync(user, team, twoFactorVerified, deviceId))
            .ReturnsAsync(encodedToken);

        var globalOptions = GlobalOptionsUtils.ValidOptions;
        globalOptions.JwtRefreshTokensEnabled = true;
        _mockGlobalOptionsProvider.Setup(x => x.Value).Returns(globalOptions);

        var jwtOptions = CreateJwtOptions();
        var provider_sut = new JwtPackageProvider(
            _jwtBuilderMock.Object,
            _refreshTokenServiceMock.Object,
            jwtOptions,
            _mockGlobalOptionsProvider.Object);

        // Act
        var result = await provider_sut.CreateJwtPackageAsync(user, team, twoFactorVerified, deviceId);

        // Assert
        result.ShouldNotBeNull();
        result.AccessToken.ShouldBe(encodedToken);
        result.RefreshToken.ShouldBeNull(); // No refresh token since 2FA not verified

        _jwtBuilderMock.Verify(x => x.CreateJwtAsync(user, team, twoFactorVerified, deviceId), Times.Once);
        _refreshTokenServiceMock.Verify(x => x.GenerateTokenAsync(It.IsAny<AppUser>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task CreateJwtPackageAsync_Should_Not_Generate_RefreshToken_When_RefreshTokens_Disabled_Globally()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(teamId);
        user.TwoFactorEnabled = false;
        var team = TeamDataFactory.Create(id: teamId);
        var twoFactorVerified = true;
        var deviceId = "device123";
        var encodedToken = "encoded.jwt.token";

        _jwtBuilderMock
            .Setup(x => x.CreateJwtAsync(user, team, twoFactorVerified, deviceId))
            .ReturnsAsync(encodedToken);

        var globalOptions = GlobalOptionsUtils.ValidOptions;
        globalOptions.JwtRefreshTokensEnabled = false; // Globally disabled
        _mockGlobalOptionsProvider.Setup(x => x.Value).Returns(globalOptions);

        var jwtOptions = CreateJwtOptions();
        var provider_sut = new JwtPackageProvider(
            _jwtBuilderMock.Object,
            _refreshTokenServiceMock.Object,
            jwtOptions,
            _mockGlobalOptionsProvider.Object);

        // Act
        var result = await provider_sut.CreateJwtPackageAsync(user, team, twoFactorVerified, deviceId);

        // Assert
        result.ShouldNotBeNull();
        result.AccessToken.ShouldBe(encodedToken);
        result.RefreshToken.ShouldBeNull(); // No refresh token since globally disabled

        _jwtBuilderMock.Verify(x => x.CreateJwtAsync(user, team, twoFactorVerified, deviceId), Times.Once);
        _refreshTokenServiceMock.Verify(x => x.GenerateTokenAsync(It.IsAny<AppUser>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//

    [Theory]
    [InlineData(RefreshTokenUpdatePolicy.Always, true)]
    [InlineData(RefreshTokenUpdatePolicy.Never, false)]
    public async Task RefreshJwtPackageAsync_Should_Update_RefreshToken_Based_On_Policy(
        RefreshTokenUpdatePolicy policy, bool shouldUpdate)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(teamId);
        var team = TeamDataFactory.Create(id: teamId);
        var existingToken = RefreshTokenDataFactory.Create();
        var updatedToken = RefreshTokenDataFactory.Create();
        var deviceId = "device123";
        var encodedToken = "encoded.jwt.token";

        _jwtBuilderMock
            .Setup(x => x.CreateJwtAsync(user, team, true, deviceId))
            .ReturnsAsync(encodedToken);

        if (shouldUpdate)
        {
            _refreshTokenServiceMock
                .Setup(x => x.UpdateTokenPayloadAsync(existingToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedToken);
        }

        var jwtOptions = CreateJwtOptions(refreshTokenUpdatePolicy: policy);
        var provider_sut = new JwtPackageProvider(
            _jwtBuilderMock.Object,
            _refreshTokenServiceMock.Object,
            jwtOptions,
            _mockGlobalOptionsProvider.Object);

        // Act
        var result = await provider_sut.RefreshJwtPackageAsync(existingToken, user, team, deviceId);

        // Assert
        result.ShouldNotBeNull();
        result.AccessToken.ShouldBe(encodedToken);
        
        if (shouldUpdate)
        {
            result.RefreshToken.ShouldBe(updatedToken.Payload);
            _refreshTokenServiceMock.Verify(x => x.UpdateTokenPayloadAsync(existingToken, It.IsAny<CancellationToken>()), Times.Once);
        }
        else
        {
            result.RefreshToken.ShouldBe(existingToken.Payload);
            _refreshTokenServiceMock.Verify(x => x.UpdateTokenPayloadAsync(It.IsAny<IdRefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        _jwtBuilderMock.Verify(x => x.CreateJwtAsync(user, team, true, deviceId), Times.Once);
    }

    //------------------------------------//

    [Theory]
    [InlineData(RefreshTokenUpdatePolicy.QuarterLife, 0.3, true)]  // 30% > 25% = should update
    [InlineData(RefreshTokenUpdatePolicy.QuarterLife, 0.2, false)] // 20% < 25% = should not update
    [InlineData(RefreshTokenUpdatePolicy.HalfLife, 0.6, true)]     // 60% > 50% = should update
    [InlineData(RefreshTokenUpdatePolicy.HalfLife, 0.4, false)]    // 40% < 50% = should not update
    [InlineData(RefreshTokenUpdatePolicy.ThreeQuarterLife, 0.8, true)]  // 80% > 75% = should update
    [InlineData(RefreshTokenUpdatePolicy.ThreeQuarterLife, 0.7, false)] // 70% < 75% = should not update
    public async Task RefreshJwtPackageAsync_Should_Update_RefreshToken_Based_On_Token_Age(
        RefreshTokenUpdatePolicy policy, double tokenAgePercentage, bool shouldUpdate)
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(teamId);
        var team = TeamDataFactory.Create(id: teamId);
        var deviceId = "device123";
        var encodedToken = "encoded.jwt.token";

        // Create a token with specific age based on test parameters
        var tokenLifetime = TimeSpan.FromDays(7); // 7-day token
        var tokenAge = TimeSpan.FromTicks((long)(tokenLifetime.Ticks * tokenAgePercentage));
        
        var existingToken = RefreshTokenDataFactory.Create();
        existingToken.CreatedUtc = DateTime.UtcNow - tokenAge;
        existingToken.ExpiresOnUtc = existingToken.CreatedUtc + tokenLifetime;

        var updatedToken = RefreshTokenDataFactory.Create();

        _jwtBuilderMock
            .Setup(x => x.CreateJwtAsync(user, team, true, deviceId))
            .ReturnsAsync(encodedToken);

        if (shouldUpdate)
        {
            _refreshTokenServiceMock
                .Setup(x => x.UpdateTokenPayloadAsync(existingToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedToken);
        }

        var jwtOptions = CreateJwtOptions(refreshTokenUpdatePolicy: policy);
        var provider_sut = new JwtPackageProvider(
            _jwtBuilderMock.Object,
            _refreshTokenServiceMock.Object,
            jwtOptions,
            _mockGlobalOptionsProvider.Object);

        // Act
        var result = await provider_sut.RefreshJwtPackageAsync(existingToken, user, team, deviceId);

        // Assert
        result.ShouldNotBeNull();
        result.AccessToken.ShouldBe(encodedToken);

        if (shouldUpdate)
        {
            result.RefreshToken.ShouldBe(updatedToken.Payload);
            _refreshTokenServiceMock.Verify(x => x.UpdateTokenPayloadAsync(existingToken, It.IsAny<CancellationToken>()), Times.Once);
        }
        else
        {
            result.RefreshToken.ShouldBe(existingToken.Payload);
            _refreshTokenServiceMock.Verify(x => x.UpdateTokenPayloadAsync(It.IsAny<IdRefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        _output.WriteLine($"Policy: {policy}, Token Age: {tokenAgePercentage:P}, Should Update: {shouldUpdate}");
    }

    //------------------------------------//

    [Fact]
    public async Task RefreshJwtPackageAsync_Should_Assume_TwoFactorVerified_True()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(teamId);
        var team = TeamDataFactory.Create(id: teamId);
        var existingToken = RefreshTokenDataFactory.Create();
        var deviceId = "device123";
        var encodedToken = "encoded.jwt.token";

        _jwtBuilderMock
            .Setup(x => x.CreateJwtAsync(user, team, true, deviceId)) // Should be called with twoFactorVerified = true
            .ReturnsAsync(encodedToken);

        var jwtOptions = CreateJwtOptions();
        var provider_sut = new JwtPackageProvider(
            _jwtBuilderMock.Object,
            _refreshTokenServiceMock.Object,
            jwtOptions,
            _mockGlobalOptionsProvider.Object);

        // Act
        var result = await provider_sut.RefreshJwtPackageAsync(existingToken, user, team, deviceId);

        // Assert
        result.ShouldNotBeNull();
        result.AccessToken.ShouldBe(encodedToken);

        // Verify that JWT was created with twoFactorVerified = true
        _jwtBuilderMock.Verify(x => x.CreateJwtAsync(user, team, true, deviceId), Times.Once);
    }

    //------------------------------------//
    // Helper Methods
    //------------------------------------//

    private static IOptions<JwtOptions> CreateJwtOptions(
        int tokenExpirationMinutes = 30,
        RefreshTokenUpdatePolicy refreshTokenUpdatePolicy = RefreshTokenUpdatePolicy.ThreeQuarterLife)
    {
        var options = new JwtOptions
        {
            TokenExpirationMinutes = tokenExpirationMinutes,
            RefreshTokenUpdatePolicy = refreshTokenUpdatePolicy,
            SymmetricTokenSigningKey = RandomStringGenerator.Generate(64),
            TokenIssuer = "TestIssuer",
            SecurityAlgorithm = "HS256"
        };

        return Options.Create(options);
    }

    //------------------------------------//

}//Cls
