using ID.Application.AppAbs.SignIn;
using ID.Domain.Utility.Messages;
using ID.OAuth.Google.Features.SignIn.GoogleCookieSignIn;

namespace ID.OAuth.Google.Tests.Features.SignIn.GoogleCookieSignIn;

public class GoogleCookieSignInCmdHandlerTests
{
    private readonly Mock<IFindOrCreateService<AppUser>> _mockFindOrCreate;
    private readonly Mock<ICookieAuthService<AppUser>> _mockCookieSignInService;
    private readonly Mock<IGoogleTokenVerifier> _mockVerifier;
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _mockTwoFactorService;
    private readonly Mock<ITwoFactorMsgService> _mockTwoFactorMsgService;
    private readonly GoogleCookieSignInCmdHandler _handler;

    public GoogleCookieSignInCmdHandlerTests()
    {
        _mockFindOrCreate = new Mock<IFindOrCreateService<AppUser>>();
        _mockCookieSignInService = new Mock<ICookieAuthService<AppUser>>();
        _mockVerifier = new Mock<IGoogleTokenVerifier>();
        _mockTwoFactorService = new Mock<ITwoFactorVerificationService<AppUser>>();
        _mockTwoFactorMsgService = new Mock<ITwoFactorMsgService>();

        _handler = new GoogleCookieSignInCmdHandler(
            _mockFindOrCreate.Object,
            _mockCookieSignInService.Object,
            _mockVerifier.Object,
            _mockTwoFactorService.Object,
            _mockTwoFactorMsgService.Object);
    }

    [Fact]
    public async Task Handle_WhenTokenVerificationFails_ShouldReturnFailureResult()
    {
        // Arrange
        var idToken = "invalid-token";
        var errorMessage = "Invalid token";
        var dto = new GoogleCookieSignInDto { IdToken = idToken };
        var command = new GoogleCookieSignInCmd(dto);

        _mockVerifier
            .Setup(v => v.VerifyTokenAsync(idToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Failure(errorMessage));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(errorMessage);
        
        // Verify subsequent methods were not called
        _mockFindOrCreate.Verify(
            f => f.FindOrCreateUserAsync(It.IsAny<GoogleVerifiedPayload>(), It.IsAny<GoogleCookieSignInDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenFindOrCreateUserFails_ShouldReturnFailureResult()
    {
        // Arrange
        var idToken = "valid-token";
        var errorMessage = "User creation failed";
        var dto = new GoogleCookieSignInDto { IdToken = idToken };
        var command = new GoogleCookieSignInCmd(dto);
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload()) { Email = "test@example.com" };

        _mockVerifier
            .Setup(v => v.VerifyTokenAsync(idToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Success(payload));

        _mockFindOrCreate
            .Setup(f => f.FindOrCreateUserAsync(payload, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Failure(errorMessage));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(errorMessage);
        
        // Verify subsequent methods were not called
        _mockTwoFactorService.Verify(
            s => s.IsTwoFactorEnabledAsync(It.IsAny<AppUser>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTwoFactorEnabledAndSendingOtpSucceeds_ShouldReturnPreconditionRequiredResult()
    {
        // Arrange
        var idToken = "valid-token";
        var deviceId = "device-123";
        var rememberMe = true;
        var dto = new GoogleCookieSignInDto { IdToken = idToken, DeviceId = deviceId, RememberMe = rememberMe };
        var command = new GoogleCookieSignInCmd(dto);
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload()) { Email = "test@example.com" };
        
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        
        var mfaData = MfaResultData.Create(TwoFactorProvider.Email);
        var expectedMessage = IDMsgs.Error.Authorization.TWO_FACTOR_REQUIRED(TwoFactorProvider.Email);

        _mockVerifier
            .Setup(v => v.VerifyTokenAsync(idToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Success(payload));

        _mockFindOrCreate
            .Setup(f => f.FindOrCreateUserAsync(payload, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(user));

        _mockTwoFactorService
            .Setup(s => s.IsTwoFactorEnabledAsync(user))
            .ReturnsAsync(true);

        _mockTwoFactorMsgService
            .Setup(s => s.SendOTPFor2FactorAuth(team, user, It.IsAny<TwoFactorProvider?>()))
            .ReturnsAsync(GenResult<MfaResultData>.Success(mfaData));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.PreconditionRequired.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TwoFactorRequired.ShouldBeTrue();
        result.Value.TwoFactorProvider.ShouldBe(TwoFactorProvider.Email);
        result.Value.Message.ShouldBe(expectedMessage);
        
        // Verify the cookie was attached
        _mockCookieSignInService.Verify(
            s => s.CreateWithTwoFactorRequiredAsync(rememberMe, user, deviceId),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTwoFactorEnabledButSendingOtpFails_ShouldReturnFailureResult()
    {
        // Arrange
        var idToken = "valid-token";
        var errorMessage = "Failed to send OTP";
        var dto = new GoogleCookieSignInDto { IdToken = idToken };
        var command = new GoogleCookieSignInCmd(dto);
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload()) { Email = "test@example.com" };
        
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);

        _mockVerifier
            .Setup(v => v.VerifyTokenAsync(idToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Success(payload));

        _mockFindOrCreate
            .Setup(f => f.FindOrCreateUserAsync(payload, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(user));

        _mockTwoFactorService
            .Setup(s => s.IsTwoFactorEnabledAsync(user))
            .ReturnsAsync(true);

        _mockTwoFactorMsgService
            .Setup(s => s.SendOTPFor2FactorAuth(team, user, It.IsAny<TwoFactorProvider?>()))
            .ReturnsAsync(GenResult<MfaResultData>.Failure(errorMessage));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(errorMessage);
        
        // Verify cookie was not attached
        _mockCookieSignInService.Verify(
            s => s.SignInAsync(
                It.IsAny<bool>(), 
                It.IsAny<AppUser>(), 
                It.IsAny<Team>(), 
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTwoFactorNotEnabled_ShouldReturnSuccessResultAndAttachCookie()
    {
        // Arrange
        var idToken = "valid-token";
        var deviceId = "device-123";
        var rememberMe = true;
        var dto = new GoogleCookieSignInDto { IdToken = idToken, DeviceId = deviceId, RememberMe = rememberMe };
        var command = new GoogleCookieSignInCmd(dto);
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload()) { Email = "test@example.com" };
        
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);

        _mockVerifier
            .Setup(v => v.VerifyTokenAsync(idToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Success(payload));

        _mockFindOrCreate
            .Setup(f => f.FindOrCreateUserAsync(payload, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(user));

        _mockTwoFactorService
            .Setup(s => s.IsTwoFactorEnabledAsync(user))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Succeeded.ShouldBeTrue();
        
        // Verify the correct cookie sign-in method is called
        _mockCookieSignInService.Verify(
            s => s.SignInAsync(rememberMe, user, team, deviceId),
            Times.Once);
        
        // Verify OTP was not sent
        _mockTwoFactorMsgService.Verify(
            s => s.SendOTPFor2FactorAuth(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<TwoFactorProvider?>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCancellationRequested_ShouldRespectCancellation()
    {
        // Arrange
        var idToken = "valid-token";
        var dto = new GoogleCookieSignInDto { IdToken = idToken };
        var command = new GoogleCookieSignInCmd(dto);
        var cancellationToken = new CancellationToken(true);

        _mockVerifier
            .Setup(v => v.VerifyTokenAsync(idToken, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException(cancellationToken));

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () => 
            await _handler.Handle(command, cancellationToken)
        );
    }

    [Fact]
    public async Task Handle_WhenDependencyThrowsException_ShouldPropagateException()
    {
        // Arrange
        var idToken = "valid-token";
        var expectedException = new InvalidOperationException("Unexpected error");
        var dto = new GoogleCookieSignInDto { IdToken = idToken };
        var command = new GoogleCookieSignInCmd(dto);
        
        _mockVerifier
            .Setup(v => v.VerifyTokenAsync(idToken, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(async () => 
            await _handler.Handle(command, CancellationToken.None)
        );
        
        exception.Message.ShouldBe(expectedException.Message);
    }
}