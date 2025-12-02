namespace ID.Application.Tests.Features.Account.Cmd.Login;

public class LoginHandlerTests_2
{
    private readonly Mock<IPreSignInService<AppUser>> _mockPreSignInService;
    private readonly Mock<IJwtPackageProvider> _mockPackageProvider;
    private readonly LoginHandler _handler;

    public LoginHandlerTests_2()
    {
        _mockPreSignInService = new Mock<IPreSignInService<AppUser>>();
        _mockPackageProvider = new Mock<IJwtPackageProvider>();

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
    public async Task Handle_TwoFactorRequired_ShouldReturnPreconditionRequired_WithTwoFactorJwt()
    {
        // Arrange
        var loginCmd = new LoginCmd(new LoginDto { Username = "username", Password = "password" });
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var mfa = MfaResultData.Create(TwoFactorProvider.Email, "extra");
        var signInResult = MyIdSignInResult.TwoFactorRequiredResult(mfa, user, team);

        _mockPreSignInService.Setup(x => x.Authenticate(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(signInResult);

        var tfPackage = JwtPackageDataFactory.Create(twoStepVerificationRequired: true, twoFactorProvider: TwoFactorProvider.Email);
        _mockPackageProvider.Setup(x => x.CreateJwtPackageWithTwoFactorRequiredAsync(
            It.IsAny<AppUser>(),
            It.IsAny<TwoFactorProvider>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(tfPackage);

        // Act
        var result = await _handler.Handle(loginCmd, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.PreconditionRequired.ShouldBeTrue();
        result.Value.ShouldBe(tfPackage);

        _mockPackageProvider.Verify(x => x.CreateJwtPackageWithTwoFactorRequiredAsync(
            It.IsAny<AppUser>(),
            It.IsAny<TwoFactorProvider>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
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
        var signInResult = MyIdSignInResult.Success(user, team, []);
        var jwtPackage = JwtPackageDataFactory.Create(accessToken: "access-token");

        _mockPreSignInService
            .Setup(s => s.Authenticate(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(signInResult);

        _mockPackageProvider
            .Setup(p => p.CreateJwtPackageAsync(
                user,
                team,
                It.IsAny<IEnumerable<AuthMethodRef>>(),
                loginDto.DeviceId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);
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
