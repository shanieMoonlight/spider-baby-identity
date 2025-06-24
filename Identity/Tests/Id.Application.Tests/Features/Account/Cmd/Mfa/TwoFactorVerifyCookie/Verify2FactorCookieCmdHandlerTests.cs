using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Features.Account.Cmd.Cookies.TwoFactorVerifyCookie;
using ID.Application.MFA;
using ID.Application.Tests.Utility;
using ID.Domain.Utility.Messages;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorVerifyCookie;

[Collection(TestingConstants.NonParallelCollection)]
public class Verify2FactorCookieCmdHandlerTests
{
    private readonly Mock<ICookieAuthService<AppUser>> _mockCookieAuthService;
    private readonly Mock<IFindUserService<AppUser>> _mockFindUserService;
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _mock2FactorService;
    private readonly Mock<ITwofactorUserIdCacheService> _mock2FactorUserIdCache;
    private readonly Verify2FactorCookieCmdHandler _handler;

    public Verify2FactorCookieCmdHandlerTests()
    {
        _mockCookieAuthService = new Mock<ICookieAuthService<AppUser>>();
        _mockFindUserService = new Mock<IFindUserService<AppUser>>();
        _mock2FactorService = new Mock<ITwoFactorVerificationService<AppUser>>();
        _mock2FactorUserIdCache = new Mock<ITwofactorUserIdCacheService>();

        _handler = new Verify2FactorCookieCmdHandler(
            _mockCookieAuthService.Object,
            _mockFindUserService.Object,
            _mock2FactorUserIdCache.Object,
            _mock2FactorService.Object);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = "valid-token";
        var deviceId = "device-123";
        var code = "123123";

        var dto = new Verify2FactorCookieDto
        {
            Code = code
        };
        
        var command = new Verify2FactorCookieCmd(dto);

        _mockCookieAuthService.Setup(s => s.TryGetTwoFactorToken()).Returns(token);
        _mockCookieAuthService.Setup(s => s.TryGetDeviceId()).Returns(deviceId);

        _mock2FactorUserIdCache.Setup(x => x.GetUserId(token)).Returns(userId);

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync((AppUser)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldNotBeNull();
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTeamIsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = AppUserDataFactory.CreateNoTeam(id: userId);

        var code = "123123";
        var token = "invalid-token";

        var dto = new Verify2FactorCookieDto
        {
            Code = code
        };
        var command = new Verify2FactorCookieCmd(dto);

        _mockCookieAuthService.Setup(s => s.TryGetTwoFactorToken()).Returns(token);

        _mock2FactorUserIdCache.Setup(x => x.GetUserId(token)).Returns(userId);
        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(userId)).ReturnsAsync(user);


        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldNotBeNull();
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenVerificationFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(id: userId, team: team);
        var token = "invalid-token";

        var code = "123123";

        var dto = new Verify2FactorCookieDto
        {
            Code = code
        };
        var command = new Verify2FactorCookieCmd(dto);

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        _mock2FactorService
            .Setup(s => s.VerifyTwoFactorTokenAsync(team, user, token))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_CODE);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenVerificationSucceeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(id: userId, team: team);
        var token = "valid-token";
        var deviceId = "device-123";
        var rememberMe = true;
        var code = "123123";


        var dto = new Verify2FactorCookieDto 
        { 
            Code = code
        };
        var command = new Verify2FactorCookieCmd(dto);

        _mockCookieAuthService.Setup(s => s.TryGetTwoFactorToken()).Returns(token);
        _mockCookieAuthService.Setup(s => s.GetRememberMe()).Returns(rememberMe);
        _mockCookieAuthService.Setup(s => s.TryGetDeviceId()).Returns(deviceId);

        _mock2FactorUserIdCache.Setup(x => x.GetUserId(token)).Returns(userId);
        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(userId)).ReturnsAsync(user);

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        _mock2FactorService
            .Setup(s => s.VerifyTwoFactorTokenAsync(team, user, code))
            .ReturnsAsync(true);

        _mockCookieAuthService
            .Setup(s => s.SignInAsync(rememberMe, user, team, deviceId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();

        // Verify that SignInAsync was called
        _mockCookieAuthService.Verify(
            s => s.SignInAsync(rememberMe, user, team, deviceId),
            Times.Once);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenTokenMissingFromCookies()
    {
        // Arrange
        var code = "123123";
        var dto = new Verify2FactorCookieDto { Code = code };
        var command = new Verify2FactorCookieCmd(dto);

        _mockCookieAuthService.Setup(s => s.TryGetTwoFactorToken()).Returns((string?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_CODE);
    }

    //------------------------------//
}