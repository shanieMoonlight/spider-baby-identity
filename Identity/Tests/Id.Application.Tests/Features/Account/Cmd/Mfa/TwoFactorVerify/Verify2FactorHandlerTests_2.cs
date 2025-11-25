using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorVerify;
using ID.Application.JWT;
using ID.Application.MFA;
using ID.Domain.Claims.AuthMethods;
using ID.GlobalSettings.Setup.Options;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Options;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorVerify;

public class Verify2FactorHandlerTests
{
    private readonly Mock<IJwtPackageProvider> _mockPackageProvider;
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _mock2FactorService;
    private readonly Mock<IFindUserService<AppUser>> _mockFindUserService;
    private readonly Mock<ITwofactorUserIdCacheService> _mock2FactorUserIdCache;
    private readonly Mock<IDeviceTrustService<AppUser>> _deviceTrustServiceMock;


    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions_refreshEnabled;
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions_refreshDisabled;
    private readonly Verify2FactorHandler _handler_RefreshEnabled;
    private readonly Verify2FactorHandler _handler_RefreshDisabled;

    private readonly IdGlobalOptions _globalOptions_RefreshEnabled = GlobalOptionsUtils.InitiallyValidOptions(
            refreshTokensEnabled: true);
    private readonly IdGlobalOptions _globalOptions_RefreshDisabled = GlobalOptionsUtils.InitiallyValidOptions(
            refreshTokensEnabled: false);




    public Verify2FactorHandlerTests()
    {
        _mockPackageProvider = new Mock<IJwtPackageProvider>();
        _mock2FactorService = new Mock<ITwoFactorVerificationService<AppUser>>();
        _mockFindUserService = new Mock<IFindUserService<AppUser>>();
        _mock2FactorUserIdCache = new Mock<ITwofactorUserIdCacheService>();
        _deviceTrustServiceMock = new Mock<IDeviceTrustService<AppUser>>();

        _mockGlobalOptions_refreshEnabled = new Mock<IOptions<IdGlobalOptions>>();
        _mockGlobalOptions_refreshEnabled.Setup(x => x.Value).Returns(_globalOptions_RefreshEnabled);
        _handler_RefreshEnabled = new Verify2FactorHandler(
            _mockPackageProvider.Object,
            _mockFindUserService.Object,
            _mock2FactorUserIdCache.Object,
            _mock2FactorService.Object,
            _deviceTrustServiceMock.Object
            );



        _mockGlobalOptions_refreshDisabled = new Mock<IOptions<IdGlobalOptions>>();
        _mockGlobalOptions_refreshDisabled.Setup(x => x.Value).Returns(_globalOptions_RefreshDisabled);
        _handler_RefreshDisabled = new Verify2FactorHandler(
            _mockPackageProvider.Object,
            _mockFindUserService.Object,
            _mock2FactorUserIdCache.Object,
            _mock2FactorService.Object,
            _deviceTrustServiceMock.Object);

    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenVerificationFails()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var token = "invalid-token";
        var deviceId = "device-123";

        var dto = new Verify2FactorDto { Code = token, DeviceId = deviceId };
        var command = new Verify2FactorCmd(dto)
        {
        };

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        _mock2FactorService
            .Setup(s => s.VerifyTwoFactorTokenAsync(team, user, token))
            .ReturnsAsync(false);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_CODE);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnJwtPackage_WhenVerificationSucceedsWithRefreshToken()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var token = "valid-token";
        var deviceId = "device-123";
        var refreshToken = RefreshTokenDataFactory.Create(user: user);
        var jwtPackage = JwtPackageDataFactory.Create(accessToken: "new-access-token");

        var dto = new Verify2FactorDto { Code = token, DeviceId = deviceId };
        var command = new Verify2FactorCmd(dto)
        {
        };

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        _mock2FactorService
            .Setup(s => s.VerifyTwoFactorTokenAsync(team, user, token))
            .ReturnsAsync(true);


        _mockPackageProvider
            .Setup(p => p.CreateJwtPackageAsync(
                user,
                user.Team!,
                It.Is<IEnumerable<AuthMethodRef>>(x => x.Contains(AuthMethodRef.mfa)),
                deviceId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBeEquivalentTo(jwtPackage);

    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnJwtPackageWithoutRefreshToken_WhenRefreshTokensDisabled()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var token = "valid-token";
        var deviceId = "device-123";
        var jwtPackage = JwtPackageDataFactory.Create(accessToken: "new-access-token");

        var dto = new Verify2FactorDto { Code = token, DeviceId = deviceId };
        var command = new Verify2FactorCmd(dto)
        {
        };

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        _mock2FactorService
            .Setup(s => s.VerifyTwoFactorTokenAsync(team, user, token))
            .ReturnsAsync(true);

        _mockPackageProvider
            .Setup(p => p.CreateJwtPackageAsync(
                user,
                user.Team!,
                It.Is<IEnumerable<AuthMethodRef>>(x => x.Contains(AuthMethodRef.mfa)),
                deviceId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler_RefreshDisabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);
        //_mockRefreshProvider.Verify(
        //    r => r.GenerateTokenAsync(It.IsAny<AppUser>(), It.IsAny<CancellationToken>()),
        //    Times.Never);

    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenServiceThrows()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var token = "valid-token";
        var deviceId = "device-123";
        var expectedException = new InvalidOperationException("Service failure");

        var dto = new Verify2FactorDto { Code = token, DeviceId = deviceId };
        var command = new Verify2FactorCmd(dto)
        { };

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        _mock2FactorService
            .Setup(s => s.VerifyTwoFactorTokenAsync(team, user, token))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            () => _handler_RefreshEnabled.Handle(command, CancellationToken.None)
        );

        exception.Message.ShouldBe(expectedException.Message);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldHandleNullUser_WithAppropriateError()
    {
        // Arrange
        var token = "valid-token";
        var deviceId = "device-123";

        var dto = new Verify2FactorDto { Code = token, DeviceId = deviceId };
        var command = new Verify2FactorCmd(dto)
        {
        };

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldNotBeNull();
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldHandleNullTeam_WithAppropriateError()
    {
        // Arrange
        var token = "valid-token";
        var deviceId = "device-123";

        var dto = new Verify2FactorDto { Code = token, DeviceId = deviceId };
        var command = new Verify2FactorCmd(dto)
        {
        };

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldNotBeNull();
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldCallTrustService_WhenTrustDeviceTrue_AndFingerprintProvided()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var token = "valid-token";
        var deviceId = "device-123";
        var deviceFingerprint = "fp-abc";

        var dto = new Verify2FactorDto { Code = token, DeviceId = deviceId, TrustDevice = true, DeviceFingerprint = deviceFingerprint, DeviceName = "My device" };
        var command = new Verify2FactorCmd(dto) { };

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>())).ReturnsAsync(user);
        _mock2FactorService.Setup(s => s.VerifyTwoFactorTokenAsync(team, user, token)).ReturnsAsync(true);

        var trustedDevice = TrustedDeviceDataFactory.Create(user: user, deviceFingerprint: deviceFingerprint);
        _deviceTrustServiceMock
            .Setup(s => s.TrustAsync(user, deviceFingerprint, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<TrustedDevice>.Success(trustedDevice));

        var jwtPackage = JwtPackageDataFactory.Create(accessToken: "access");
        _mockPackageProvider
            .Setup(p => p.CreateJwtPackageAsync(user, user.Team!, It.Is<IEnumerable<AuthMethodRef>>(x => x.Contains(AuthMethodRef.mfa)), deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);

        _deviceTrustServiceMock.Verify(s => s.TrustAsync(user, deviceFingerprint, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldContinue_WhenTrustServiceFails()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var token = "valid-token";
        var deviceId = "device-123";
        var deviceFingerprint = "fp-abc";

        var dto = new Verify2FactorDto { Code = token, DeviceId = deviceId, TrustDevice = true, DeviceFingerprint = deviceFingerprint };
        var command = new Verify2FactorCmd(dto) { };

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>())).ReturnsAsync(user);
        _mock2FactorService.Setup(s => s.VerifyTwoFactorTokenAsync(team, user, token)).ReturnsAsync(true);

        _deviceTrustServiceMock
            .Setup(s => s.TrustAsync(user, deviceFingerprint, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<TrustedDevice>.BadRequestResult("fail"));

        var jwtPackage = JwtPackageDataFactory.Create(accessToken: "access");
        _mockPackageProvider
            .Setup(p => p.CreateJwtPackageAsync(user, user.Team!, It.Is<IEnumerable<AuthMethodRef>>(x => x.Contains(AuthMethodRef.mfa)), deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);

        _deviceTrustServiceMock.Verify(s => s.TrustAsync(user, deviceFingerprint, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------//
}
