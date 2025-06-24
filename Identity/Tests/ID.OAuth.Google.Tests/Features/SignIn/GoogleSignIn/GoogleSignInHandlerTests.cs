namespace ID.OAuth.Google.Tests.Features.SignIn.GoogleSignIn;

public class GoogleSignInHandlerTests
{
    private readonly Mock<IFindOrCreateService<AppUser>> _mockFindOrCreate;
    private readonly Mock<IJwtPackageProvider> _mockJwtPackageProvider;
    private readonly Mock<IGoogleTokenVerifier> _mockVerifier;
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _mockTwoFactorService;
    private readonly Mock<ITwoFactorMsgService> _mockTwoFactorMsgService;
    private readonly GoogleSignInHandler _handler;

    public GoogleSignInHandlerTests()
    {
        _mockFindOrCreate = new Mock<IFindOrCreateService<AppUser>>();
        _mockJwtPackageProvider = new Mock<IJwtPackageProvider>();
        _mockVerifier = new Mock<IGoogleTokenVerifier>();
        _mockTwoFactorService = new Mock<ITwoFactorVerificationService<AppUser>>();
        _mockTwoFactorMsgService = new Mock<ITwoFactorMsgService>();

        _handler = new GoogleSignInHandler(
            _mockFindOrCreate.Object,
            _mockJwtPackageProvider.Object,
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
        var dto = new GoogleSignInDto { IdToken = idToken };
        var command = new GoogleSignInCmd(dto);

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
            f => f.FindOrCreateUserAsync(It.IsAny<GoogleVerifiedPayload>(), It.IsAny<GoogleSignInDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenFindOrCreateUserFails_ShouldReturnFailureResult()
    {
        // Arrange
        var idToken = "valid-token";
        var errorMessage = "User creation failed";
        var dto = new GoogleSignInDto { IdToken = idToken };
        var command = new GoogleSignInCmd(dto);
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload()){ Email = "test@example.com" };

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
    public async Task Handle_WhenTwoFactorEnabledAndSendingOtpSucceeds_ShouldReturnJwtPackageWithTwoFactorRequired()
    {
        // Arrange
        var idToken = "valid-token";
        var deviceId = "device-123";
        var dto = new GoogleSignInDto { IdToken = idToken, DeviceId = deviceId };
        var command = new GoogleSignInCmd(dto);
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload()){ Email = "test@example.com" };
        
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        
        var mfaData = MfaResultData.Create(TwoFactorProvider.Email);
        var jwtPackage = JwtPackageDataFactory.Create(accessToken: "jwt-token-with-2fa-required");

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

        _mockJwtPackageProvider
            .Setup(p => p.CreateJwtPackageWithTwoFactorRequiredAsync(
                user,
                mfaData.TwoFactorProvider,
                mfaData.ExtraInfo,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);
        
        // Verify the standard JWT package creation was not called
        _mockJwtPackageProvider.Verify(
            p => p.CreateJwtPackageAsync(It.IsAny<AppUser>(), It.IsAny<Team>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTwoFactorEnabledButSendingOtpFails_ShouldReturnFailureResult()
    {
        // Arrange
        var idToken = "valid-token";
        var errorMessage = "Failed to send OTP";
        var dto = new GoogleSignInDto { IdToken = idToken };
        var command = new GoogleSignInCmd(dto);
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload()){ Email = "test@example.com" };
        
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
        
        // Verify JWT package creation was not called
        _mockJwtPackageProvider.Verify(
            p => p.CreateJwtPackageWithTwoFactorRequiredAsync(
                It.IsAny<AppUser>(), 
                It.IsAny<TwoFactorProvider>(), 
                It.IsAny<string>(), 
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenTwoFactorNotEnabled_ShouldReturnStandardJwtPackage()
    {
        // Arrange
        var idToken = "valid-token";
        var deviceId = "device-123";
        var dto = new GoogleSignInDto { IdToken = idToken, DeviceId = deviceId };
        var command = new GoogleSignInCmd(dto);
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload()){ Email = "test@example.com" };
        
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var jwtPackage = JwtPackageDataFactory.Create(accessToken: "standard-jwt-token");

        _mockVerifier
            .Setup(v => v.VerifyTokenAsync(idToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Success(payload));

        _mockFindOrCreate
            .Setup(f => f.FindOrCreateUserAsync(payload, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(user));

        _mockTwoFactorService
            .Setup(s => s.IsTwoFactorEnabledAsync(user))
            .ReturnsAsync(false);

        _mockJwtPackageProvider
            .Setup(p => p.CreateJwtPackageAsync(
                user,
                team,
                deviceId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);
        
        // Verify the two-factor JWT package creation was not called
        _mockJwtPackageProvider.Verify(
            p => p.CreateJwtPackageWithTwoFactorRequiredAsync(
                It.IsAny<AppUser>(), 
                It.IsAny<TwoFactorProvider>(), 
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        
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
        var dto = new GoogleSignInDto { IdToken = idToken };
        var command = new GoogleSignInCmd(dto);
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload()){ Email = "test@example.com" };
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
        var dto = new GoogleSignInDto { IdToken = idToken };
        var command = new GoogleSignInCmd(dto);
        
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