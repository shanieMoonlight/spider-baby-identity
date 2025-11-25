using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppImps.SignIn;
using ID.Application.Features.Account.Cmd.Login;
using ID.Domain.Abstractions.Services.TrustedDevices;
using ID.Domain.Claims.AuthMethods;

namespace ID.Application.Tests.ApplicationImps.SignIn;

public class PreSignInServiceTests
{
    private readonly Mock<IIdUserMgmtService<AppUser>> _userMgrMock;
    private readonly Mock<IFindUserService<AppUser>> _findUserServiceMock;
    private readonly Mock<IEmailConfirmationBus> _emailConfirmationBusMock;
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _2FactorServiceMock;
    private readonly Mock<ITwoFactorMsgService> _twoFactorMsgServiceMock;
    private readonly Mock<ITrustedDeviceService<AppUser>> _trustedDeviceServiceMock;
    private readonly Mock<ILogger<PreSignInService<AppUser>>> _loggerMock;
    private readonly PreSignInService<AppUser> _preSignInService;

    //- - - - - - - - - - - - - - - - - - - -//

    public PreSignInServiceTests()
    {
        _userMgrMock = new Mock<IIdUserMgmtService<AppUser>>();
        _findUserServiceMock = new Mock<IFindUserService<AppUser>>();
        _emailConfirmationBusMock = new Mock<IEmailConfirmationBus>();
        _2FactorServiceMock = new Mock<ITwoFactorVerificationService<AppUser>>();
        _twoFactorMsgServiceMock = new Mock<ITwoFactorMsgService>();
        _loggerMock = new Mock<ILogger<PreSignInService<AppUser>>>();
        _trustedDeviceServiceMock = new Mock<ITrustedDeviceService<AppUser>>();


        _preSignInService = new PreSignInService<AppUser>(
            _userMgrMock.Object,
            _findUserServiceMock.Object,
            _emailConfirmationBusMock.Object,
            _2FactorServiceMock.Object,
            _twoFactorMsgServiceMock.Object,
            _trustedDeviceServiceMock.Object,
            _loggerMock.Object
        );
    }

    //-----------------------//

    [Fact]
    public async Task Authenticate_UserNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com" };
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _findUserServiceMock
            .Setup(s =>
                s.FindUserWithTeamDetailsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid?>()
                )
            )
            .ReturnsAsync((AppUser)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.NotFound.ShouldBeTrue();
    }

    //-----------------------//

    [Fact]
    public async Task Authenticate_EmailNotConfirmed_ReturnsEmailConfirmedRequiredResult()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var loginDto = new LoginDto { Email = user.Email };
        _findUserServiceMock
            .Setup(s =>
                s.FindUserWithTeamDetailsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid?>()
                )
            )
            .ReturnsAsync(user);
        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(false);

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.EmailConfirmationRequired.ShouldBeTrue();
    }

    //-----------------------//

    [Fact]
    public async Task Authenticate_InvalidPassword_ReturnsUnauthorizedResult()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var loginDto = new LoginDto { Email = user.Email, Password = "wrongpassword" };
        _findUserServiceMock
            .Setup(s =>
                s.FindUserWithTeamDetailsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid?>()
                )
            )
            .ReturnsAsync(user);
        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _userMgrMock.Setup(s => s.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.Unauthorized.ShouldBeTrue();
    }

    //-----------------------//

    [Fact]
    public async Task Authenticate_TwoFactorEnabled_MsgSuccess_ReturnsTwoFactorRequiredResult()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var loginDto = new LoginDto { Email = user.Email, Password = "correctpassword" };
        _findUserServiceMock
            .Setup(s =>
                s.FindUserWithTeamDetailsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid?>()
                )
            )
            .ReturnsAsync(user);
        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _userMgrMock.Setup(s => s.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
        _2FactorServiceMock.Setup(s => s.IsTwoFactorEnabledAsync(user)).ReturnsAsync(true);

        MfaResultData mfaResultData = MfaResultData.Create(
            user.TwoFactorProvider,
            "otp-extyra-info"
        );

        _twoFactorMsgServiceMock
            .Setup(s =>
                s.SendOTPFor2FactorAuth(
                    It.IsAny<Team>(),
                    It.IsAny<AppUser>(),
                    It.IsAny<TwoFactorProvider?>()
                )
            )
            .ReturnsAsync(
                GenResult<MfaResultData>.Success(
                    MfaResultData.Create(user.TwoFactorProvider, "otp-extyra-info")
                )
            );

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.TwoFactorRequired.ShouldBeTrue();
    }

    //-----------------------//

    [Fact]
    public async Task Authenticate_TwoFactorEnabled_MsgFailed_ReturnsTwoFactorRequiredResult()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var loginDto = new LoginDto { Email = user.Email, Password = "correctpassword" };
        _findUserServiceMock
            .Setup(s =>
                s.FindUserWithTeamDetailsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid?>()
                )
            )
            .ReturnsAsync(user);
        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _userMgrMock.Setup(s => s.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
        _2FactorServiceMock.Setup(s => s.IsTwoFactorEnabledAsync(user)).ReturnsAsync(true);

        _twoFactorMsgServiceMock
            .Setup(s =>
                s.SendOTPFor2FactorAuth(
                    It.IsAny<Team>(),
                    It.IsAny<AppUser>(),
                    It.IsAny<TwoFactorProvider?>()
                )
            )
            .ReturnsAsync(GenResult<MfaResultData>.Failure("Something went wrong"));

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.TwoFactorRequired.ShouldBeFalse();
        result.Succeeded.ShouldBeFalse();
    }

    //-----------------------//

    [Fact]
    public async Task Authenticate_Success_ReturnsSuccessResult()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var loginDto = new LoginDto { Email = user.Email, Password = "correctpassword" };
        _findUserServiceMock
            .Setup(s =>
                s.FindUserWithTeamDetailsAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid?>()
                )
            )
            .ReturnsAsync(user);
        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _userMgrMock.Setup(s => s.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
        _2FactorServiceMock.Setup(s => s.IsTwoFactorEnabledAsync(user)).ReturnsAsync(false);
        MfaResultData mfaResultData = MfaResultData.Create(
            user.TwoFactorProvider,
            "otp-extyra-info"
        );

        _twoFactorMsgServiceMock
            .Setup(s =>
                s.SendOTPFor2FactorAuth(
                    It.IsAny<Team>(),
                    It.IsAny<AppUser>(),
                    It.IsAny<TwoFactorProvider?>()
                )
            )
            .ReturnsAsync(GenResult<MfaResultData>.Success(mfaResultData));

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.User.ShouldBe(user);
        result.Team.ShouldBe(team);
    }

    //-----------------------//

    [Fact]
    public async Task Authenticate_TrustedDeviceValid_BypassesTwoFactor_ReturnsSuccessWithMfa()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var device = TrustedDeviceDataFactory.Create(user: user, trustedUntil: DateTime.UtcNow.AddDays(1));

        var loginDto = new LoginDto { Email = user.Email, Password = "correctpassword", DeviceId = device.Fingerprint };

        _findUserServiceMock
            .Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _userMgrMock.Setup(s => s.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);

        _trustedDeviceServiceMock
            .Setup(s => s.GetByFingerprintAsync(user, device.Fingerprint, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        _trustedDeviceServiceMock
            .Setup(s => s.UpdateLastUsedAsync(user, device, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.User.ShouldBe(user);
        result.Team.ShouldBe(team);
        result.AuthMethods.ShouldContain(AuthMethodRef.mfa);
    }

    //-----------------------//

    [Fact]
    public async Task Authenticate_TrustedDeviceExpired_DoesNotBypass_UsesTwoFactor()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var device = TrustedDeviceDataFactory.Create(user: user, trustedUntil: DateTime.UtcNow.AddDays(-1));

        var loginDto = new LoginDto { Email = user.Email, Password = "correctpassword", DeviceId = device.Fingerprint };

        _findUserServiceMock
            .Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _userMgrMock.Setup(s => s.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);

        _trustedDeviceServiceMock
            .Setup(s => s.GetByFingerprintAsync(user, device.Fingerprint, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        _2FactorServiceMock.Setup(s => s.IsTwoFactorEnabledAsync(user)).ReturnsAsync(true);
        _twoFactorMsgServiceMock
            .Setup(s => s.SendOTPFor2FactorAuth(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<TwoFactorProvider?>()))
            .ReturnsAsync(GenResult<MfaResultData>.Success(MfaResultData.Create(user.TwoFactorProvider, "otp")));

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.TwoFactorRequired.ShouldBeTrue();
    }

    //-----------------------//

    [Fact]
    public async Task Authenticate_TrustedDeviceLookupThrows_ContinuesWithTwoFactor()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var loginDto = new LoginDto { Email = user.Email, Password = "correctpassword", DeviceId = "some-device" };

        _findUserServiceMock
            .Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _userMgrMock.Setup(s => s.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);

        _trustedDeviceServiceMock
            .Setup(s => s.GetByFingerprintAsync(user, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Exception("boom"));

        _2FactorServiceMock.Setup(s => s.IsTwoFactorEnabledAsync(user)).ReturnsAsync(true);
        _twoFactorMsgServiceMock
            .Setup(s => s.SendOTPFor2FactorAuth(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<TwoFactorProvider?>()))
            .ReturnsAsync(GenResult<MfaResultData>.Success(MfaResultData.Create(user.TwoFactorProvider, "otp")));

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.TwoFactorRequired.ShouldBeTrue();
    }

    //-----------------------//
} //Cls
