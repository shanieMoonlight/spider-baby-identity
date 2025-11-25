using ID.Application.Features.Account.Cmd.LoginRefresh;
using ID.Application.JWT;
using ID.Domain.Entities.Refreshing;
using ID.GlobalSettings.Setup.Options;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Options;

namespace ID.Application.Tests.Features.Account.Cmd.LoginRefresh;

public class RefreshTknHandlerTests
{
    private readonly Mock<IJwtRefreshTokenService<AppUser>> _mockTokenService;
    private readonly Mock<IJwtPackageProvider> _mockJwtPackageProvider;
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions_refreshEnabled;
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions_refreshDisabled;
    private readonly LoginRefreshHandler _handler_RefreshEnabled;
    private readonly LoginRefreshHandler _handler_RefreshDisabled;

    private readonly IdGlobalOptions _globalOptions_RefreshEnabled = GlobalOptionsUtils.InitiallyValidOptions(
            refreshTokensEnabled: true);

    private readonly IdGlobalOptions _globalOptions_RefreshDisabled = GlobalOptionsUtils.InitiallyValidOptions(
            refreshTokensEnabled: false);

    public RefreshTknHandlerTests()
    {
        _mockTokenService = new Mock<IJwtRefreshTokenService<AppUser>>();
        _mockJwtPackageProvider = new Mock<IJwtPackageProvider>();

        _mockGlobalOptions_refreshEnabled = new Mock<IOptions<IdGlobalOptions>>();
        _mockGlobalOptions_refreshEnabled.Setup(x => x.Value).Returns(_globalOptions_RefreshEnabled);
        _handler_RefreshEnabled = new LoginRefreshHandler(
            _mockTokenService.Object,
            _mockJwtPackageProvider.Object,
            _mockGlobalOptions_refreshEnabled.Object);

        _mockGlobalOptions_refreshDisabled = new Mock<IOptions<IdGlobalOptions>>();
        _mockGlobalOptions_refreshDisabled.Setup(x => x.Value).Returns(_globalOptions_RefreshDisabled);
        _handler_RefreshDisabled = new LoginRefreshHandler(
            _mockTokenService.Object,
            _mockJwtPackageProvider.Object,
            _mockGlobalOptions_refreshDisabled.Object);


    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenRefreshTokensDisabled()
    {
        // Arrange
        var dto = new LoginRefreshDto()
        {
            RefreshToken = "some-token",
            DeviceFingerprint = "device-123"
        };
        var command = new LoginRefreshCmd(dto);

        // Configure mock instead of static property;

        // Act
        var result = await _handler_RefreshDisabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.REFRESH_TOKEN_DISABLED);

    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenRefreshTokenNotFound()
    {
        // Arrange
        var dto = new LoginRefreshDto()
        {
            RefreshToken = "some-token",
            DeviceFingerprint = "device-123"
        };
        var command = new LoginRefreshCmd(dto);

        // Configure mock instead of static property

        _mockTokenService
            .Setup(s => s.FindTokenWithUserAndDeviceAndTeamAsync(command.Dto.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdRefreshToken)null!);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH);

    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenRefreshTokenIsExpired()
    {
        // Arrange
        var dto = new LoginRefreshDto()
        {
            RefreshToken = "some-token",
            DeviceFingerprint = "device-123"
        };
        var command = new LoginRefreshCmd(dto);
        var user = AppUserDataFactory.Create();
        var expiredToken = RefreshTokenDataFactory.Create(user: user, expiresOnUtc: DateTime.Now.AddDays(-10));


        _mockTokenService
            .Setup(s => s.FindTokenWithUserAndDeviceAndTeamAsync(command.Dto.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredToken);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNull()
    {
        // Arrange
        var dto = new LoginRefreshDto()
        {
            RefreshToken = "some-token",
            DeviceFingerprint = "device-123"
        };
        var command = new LoginRefreshCmd(dto);
        var tokenWithoutUser = RefreshTokenDataFactory.Create(user: null, expiresOnUtc: DateTime.Now.AddDays(10));


        _mockTokenService
            .Setup(s => s.FindTokenWithUserAndDeviceAndTeamAsync(command.Dto.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenWithoutUser);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH_EXPIRED_TOKEN);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenTeamIsNull()
    {
        // Arrange
        var dto = new LoginRefreshDto()
        {
            RefreshToken = "some-token",
            DeviceFingerprint = "device-123"
        };
        var command = new LoginRefreshCmd(dto);
        var userWithoutTeam = AppUserDataFactory.Create();
        var tokenWithoutTeam = RefreshTokenDataFactory.Create(user: userWithoutTeam, expiresOnUtc: DateTime.Now.AddDays(10));


        _mockTokenService
            .Setup(s => s.FindTokenWithUserAndDeviceAndTeamAsync(command.Dto.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenWithoutTeam);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnJwtPackage_WhenAllValidationsPass()
    {
        // Arrange
        var trustedDevice = TrustedDeviceDataFactory.Create();
        var dto = new LoginRefreshDto()
        {
            RefreshToken = "some-token",
            DeviceFingerprint = trustedDevice.Fingerprint
        };
        var command = new LoginRefreshCmd(dto);
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var validToken = RefreshTokenDataFactory.Create(user: user, expiresOnUtc: DateTime.Now.AddDays(10), trustedDevice: trustedDevice);
        var updatedToken = Mock.Of<IdRefreshToken>();
        var jwtPackage = JwtPackageDataFactory.Create(accessToken: "new-access-token");


        _mockTokenService
            .Setup(s => s.FindTokenWithUserAndDeviceAndTeamAsync(command.Dto.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validToken);

        //_mockTokenService
        //    .Setup(s => s.UpdateTokenPayloadAsync(validToken, It.IsAny<CancellationToken>()))
        //    .ReturnsAsync(updatedToken);

        _mockJwtPackageProvider
            .Setup(p => p.RefreshJwtPackageAsync(validToken, user, team, command.Dto.DeviceFingerprint))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);

    }


    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenDeviceFingerprintMismatch()
    {
        // Arrange
        var dto = new LoginRefreshDto()
        {
            RefreshToken = "some-token",
            DeviceFingerprint = "device-123"
        };
        var command = new LoginRefreshCmd(dto);
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var differentTrustedDevice = TrustedDeviceDataFactory.Create(deviceFingerprint: "other-device-fingerprint");
        var tokenWithDifferentFingerprint = RefreshTokenDataFactory.Create(user: user, expiresOnUtc: DateTime.Now.AddDays(10), trustedDevice: differentTrustedDevice);


        _mockTokenService
            .Setup(s => s.FindTokenWithUserAndDeviceAndTeamAsync(command.Dto.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenWithDifferentFingerprint);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnJwtPackage_WhenDeviceFingerprintIsNullAndDtoHasNoFingerprint()
    {
        // Arrange
        var dto = new LoginRefreshDto()
        {
            RefreshToken = "some-token",
            DeviceFingerprint = null
        };
        var command = new LoginRefreshCmd(dto);
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        //var trustedDevice = TrustedDeviceDataFactory.Create(deviceFingerprint: "other-device-fingerprint");
        var validToken = RefreshTokenDataFactory.Create(user: user, expiresOnUtc: DateTime.Now.AddDays(10));
        var jwtPackage = JwtPackageDataFactory.Create(accessToken: "new-access-token");


        _mockTokenService
            .Setup(s => s.FindTokenWithUserAndDeviceAndTeamAsync(command.Dto.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validToken);

        _mockJwtPackageProvider
            .Setup(p => p.RefreshJwtPackageAsync(validToken, user, team, command.Dto.DeviceFingerprint))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);

    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnJwtPackage_WhenDeviceFingerprintIsTrimmedMatch()
    {
        // Arrange
        var trustedDevice = TrustedDeviceDataFactory.Create();
        // give fingerprint with padding
        var paddedFingerprint = "  " + trustedDevice.Fingerprint + "  ";
        var dto = new LoginRefreshDto()
        {
            RefreshToken = "some-token",
            DeviceFingerprint = trustedDevice.Fingerprint
        };
        var command = new LoginRefreshCmd(dto);
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        // create token whose trusted device fingerprint has padding
        var tokenDevice = TrustedDeviceDataFactory.Create(user: user, deviceFingerprint: paddedFingerprint);
        var validToken = RefreshTokenDataFactory.Create(user: user, expiresOnUtc: DateTime.Now.AddDays(10), trustedDevice: tokenDevice);
        var jwtPackage = JwtPackageDataFactory.Create();


        _mockTokenService
            .Setup(s => s.FindTokenWithUserAndDeviceAndTeamAsync(command.Dto.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validToken);

        _mockJwtPackageProvider
            .Setup(p => p.RefreshJwtPackageAsync(validToken, user, team, command.Dto.DeviceFingerprint))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);

    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldProceed_WhenBothFingerprintsNull()
    {
        // Arrange
        var dto = new LoginRefreshDto()
        {
            RefreshToken = "some-token",
            DeviceFingerprint = null
        };
        var command = new LoginRefreshCmd(dto);
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var validToken = RefreshTokenDataFactory.Create(user: user, expiresOnUtc: DateTime.Now.AddDays(10), trustedDevice: (TrustedDevice?)null);
        var jwtPackage = JwtPackageDataFactory.Create();

        _mockTokenService
            .Setup(s => s.FindTokenWithUserAndDeviceAndTeamAsync(command.Dto.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validToken);

        _mockJwtPackageProvider
            .Setup(p => p.RefreshJwtPackageAsync(validToken, user, team, command.Dto.DeviceFingerprint))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenTokenFingerprintNull_AndRequestProvided()
    {
        // Arrange
        var dto = new LoginRefreshDto()
        {
            RefreshToken = "some-token",
            DeviceFingerprint = "device-123"
        };
        var command = new LoginRefreshCmd(dto);
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var validToken = RefreshTokenDataFactory.Create(user: user, expiresOnUtc: DateTime.Now.AddDays(10), trustedDevice: (TrustedDevice?)null);

        _mockTokenService
            .Setup(s => s.FindTokenWithUserAndDeviceAndTeamAsync(command.Dto.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validToken);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldTrimAndMatchFingerprints()
    {
        // Arrange
        var trustedDevice = TrustedDeviceDataFactory.Create();
        // give fingerprint with padding
        var paddedFingerprint = "  " + trustedDevice.Fingerprint + "  ";
        var dto = new LoginRefreshDto()
        {
            RefreshToken = "some-token",
            DeviceFingerprint = trustedDevice.Fingerprint
        };
        var command = new LoginRefreshCmd(dto);
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        // create token whose trusted device fingerprint has padding
        var tokenDevice = TrustedDeviceDataFactory.Create(user: user, deviceFingerprint: paddedFingerprint);
        var validToken = RefreshTokenDataFactory.Create(user: user, expiresOnUtc: DateTime.Now.AddDays(10), trustedDevice: tokenDevice);
        var jwtPackage = JwtPackageDataFactory.Create();


        _mockTokenService
            .Setup(s => s.FindTokenWithUserAndDeviceAndTeamAsync(command.Dto.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validToken);

        _mockJwtPackageProvider
            .Setup(p => p.RefreshJwtPackageAsync(validToken, user, team, command.Dto.DeviceFingerprint))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);
    }

    //------------------------------//

}//Cls