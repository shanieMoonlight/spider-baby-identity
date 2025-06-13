using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.SignIn;
using ID.Application.Features.Account.Cmd.Login;
using ID.Application.JWT;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.GlobalSettings.Setup.Options;
using ID.Tests.Data.Factories;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Login;

public class LoginHandlerTests_2
{
    private readonly Mock<IPreSignInService<AppUser>> _mockPreSignInService;
    private readonly Mock<IJwtPackageProvider> _mockPackageProvider;
    private readonly Mock<IJwtRefreshTokenService<AppUser>> _mockRefreshProvider;
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions;
    private readonly LoginHandler _handler;

    public LoginHandlerTests_2()
    {
        _mockPreSignInService = new Mock<IPreSignInService<AppUser>>();
        _mockPackageProvider = new Mock<IJwtPackageProvider>();
        _mockRefreshProvider = new Mock<IJwtRefreshTokenService<AppUser>>();
        
        _mockGlobalOptions = new Mock<IOptions<IdGlobalOptions>>();
        _mockGlobalOptions.Setup(x => x.Value).Returns(GlobalOptionsUtils.ValidOptions);

        _handler = new LoginHandler(
            _mockPreSignInService.Object,
            _mockPackageProvider.Object);

    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFoundResult_WhenUserNotFound()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password" };
        var command = new LoginCmd(loginDto);
        var signInResult = MyIdSignInResult.NotFoundResult();

        _mockPreSignInService
            .Setup(s => s.Authenticate(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(signInResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(signInResult.Message);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnUnauthorizedResult_WhenUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password" };
        var command = new LoginCmd(loginDto);
        var signInResult = MyIdSignInResult.UnauthorizedResult();

        _mockPreSignInService
            .Setup(s => s.Authenticate(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(signInResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.Info.ShouldBe(signInResult.Message);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnPreconditionRequiredResult_WhenEmailConfirmationRequired()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password" };
        var command = new LoginCmd(loginDto);
        var signInResult = MyIdSignInResult.EmailConfirmedRequiredResult("Email confirmation required");

        _mockPreSignInService
            .Setup(s => s.Authenticate(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(signInResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.PreconditionRequired.ShouldBeTrue();
        result.Info.ShouldBe(signInResult.Message);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_TwoFactorRequired_ShouldReturnTwoFactorRequiredResult()
    {
        // Arrange
        var loginCmd = new LoginCmd(new LoginDto { Username = "username", Password = "password" });
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(teamId: team.Id);
        var signInResult = MyIdSignInResult.TwoFactorRequiredResult(MfaResultData.Create(TwoFactorProvider.Email), user, team);
        _mockPreSignInService.Setup(x => x.Authenticate(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(signInResult);
        _mockPackageProvider.Setup(x => x.CreateJwtPackageWithTwoFactorRequiredAsync(
            It.IsAny<AppUser>(), 
            It.IsAny<Team>(), 
            It.IsAny<TwoFactorProvider>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(JwtPackageDataFactory.Create(twoStepVerificationRequired: true, twoFactorProvider: TwoFactorProvider.Email));

        // Act
        var result = await _handler.Handle(loginCmd, CancellationToken.None);

        // Assert
        result.Value?.TwoStepVerificationRequired.ShouldBeTrue();
        _mockPackageProvider.Verify(x => x.CreateJwtPackageWithTwoFactorRequiredAsync(
                It.IsAny<AppUser>(), 
                It.IsAny<Team>(), 
                It.IsAny<TwoFactorProvider>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnJwtPackage_WhenAuthenticationSucceeds()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password", DeviceId = "device-123" };
        var command = new LoginCmd(loginDto);
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var refreshToken = RefreshTokenDataFactory.Create(user: user);
        var signInResult = MyIdSignInResult.Success(user, team);
        var jwtPackage = JwtPackageDataFactory.Create(accessToken: "access-token");

        // Set up the mock to return true for RefreshTokensEnabled
        var refreshEnabledOptions = GlobalOptionsUtils.InitiallyValidOptions(
            refreshTokensEnabled: true);
        _mockGlobalOptions.Setup(x => x.Value).Returns(refreshEnabledOptions);




        _mockPreSignInService
            .Setup(s => s.Authenticate(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(signInResult);

        _mockRefreshProvider
            .Setup(r => r.GenerateTokenAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        _mockPackageProvider
            .Setup(p => p.CreateJwtPackageAsync(
                user,
                team,
                false,
                loginDto.DeviceId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);

        // Verify that the global settings were checked
        refreshEnabledOptions.JwtRefreshTokensEnabled.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnJwtPackageWithoutRefreshToken_WhenRefreshTokensDisabled()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password", DeviceId = "device-123" };
        var command = new LoginCmd(loginDto);
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var signInResult = MyIdSignInResult.Success(user, team);
        var jwtPackage = JwtPackageDataFactory.Create(accessToken: "access-token");

        // Set up the mock to return false for RefreshTokensEnabled
        var refreshDisabledOptions = GlobalOptionsUtils.InitiallyValidOptions(
            refreshTokensEnabled: false);
        _mockGlobalOptions.Setup(x => x.Value).Returns(refreshDisabledOptions);


        _mockPreSignInService
            .Setup(s => s.Authenticate(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(signInResult);

        _mockPackageProvider
            .Setup(p => p.CreateJwtPackageAsync(
                user,
                team,
                false,
                loginDto.DeviceId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);


        LoginHandler handler = new(
            _mockPreSignInService.Object,
            _mockPackageProvider.Object);


        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);
        _mockRefreshProvider.Verify(
            r => r.GenerateTokenAsync(It.IsAny<AppUser>(), It.IsAny<CancellationToken>()),
            Times.Never);

        // Verify that the global settings were checked;
        refreshDisabledOptions.JwtRefreshTokensEnabled.ShouldBeFalse();
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldPropagateFailure_WhenSignInResultFails()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "password" };
        var command = new LoginCmd(loginDto);
        var errorMessage = "Some other error occurred";
        var signInResult = MyIdSignInResult.Failure(errorMessage);

        _mockPreSignInService
            .Setup(s => s.Authenticate(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(signInResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(errorMessage);
    }

    //------------------------------//
}
