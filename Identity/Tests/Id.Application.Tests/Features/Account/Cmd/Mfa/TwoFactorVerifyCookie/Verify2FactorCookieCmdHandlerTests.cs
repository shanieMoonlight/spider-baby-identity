using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorVerifyCookie;
using ID.Application.Tests.Utility;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using Xunit;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorVerifyCookie;

[Collection(TestingConstants.NonParallelCollection)]
public class Verify2FactorCookieCmdHandlerTests
{
    private readonly Mock<ICookieSignInService<AppUser>> _mockCookieSignInService;
    private readonly Mock<IFindUserService<AppUser>> _mockFindUserService;
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _mock2FactorService;
    private readonly Verify2FactorCookieCmdHandler _handler;

    public Verify2FactorCookieCmdHandlerTests()
    {
        _mockCookieSignInService = new Mock<ICookieSignInService<AppUser>>();
        _mockFindUserService = new Mock<IFindUserService<AppUser>>();
        _mock2FactorService = new Mock<ITwoFactorVerificationService<AppUser>>();

        _handler = new Verify2FactorCookieCmdHandler(
            _mockCookieSignInService.Object,
            _mockFindUserService.Object,
            _mock2FactorService.Object);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = "valid-token";
        var deviceId = "device-123";

        var dto = new Verify2FactorCookieDto { Token = token, DeviceId = deviceId, UserId = userId };
        var command = new Verify2FactorCookieCmd(dto);

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync((AppUser)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenTeamIsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = AppUserDataFactory.CreateNoTeam(id: userId);
        var token = "valid-token";
        var deviceId = "device-123";

        var dto = new Verify2FactorCookieDto { Token = token, DeviceId = deviceId, UserId = userId };
        var command = new Verify2FactorCookieCmd(dto);

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH);
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
        var deviceId = "device-123";

        var dto = new Verify2FactorCookieDto { Token = token, DeviceId = deviceId, UserId = userId };
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
        result.Info.ShouldBe(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_TOKEN);
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

        var dto = new Verify2FactorCookieDto 
        { 
            Token = token, 
            DeviceId = deviceId, 
            UserId = userId,
            RememberMe = rememberMe
        };
        var command = new Verify2FactorCookieCmd(dto);

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        _mock2FactorService
            .Setup(s => s.VerifyTwoFactorTokenAsync(team, user, token))
            .ReturnsAsync(true);

        _mockCookieSignInService
            .Setup(s => s.SignInAsync(rememberMe, user, team, false, deviceId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldBe("Signed In!");

        // Verify that SignInAsync was called
        _mockCookieSignInService.Verify(
            s => s.SignInAsync(rememberMe, user, team, false, deviceId),
            Times.Once);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldUsePrincipalUserId_WhenAvailable()
    {
        // Arrange
        var principalUserId = Guid.NewGuid();
        var dtoUserId = Guid.NewGuid();  // Different from principalUserId
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(id: principalUserId, team: team);
        var token = "valid-token";
        var deviceId = "device-123";

        var dto = new Verify2FactorCookieDto 
        { 
            Token = token, 
            DeviceId = deviceId, 
            UserId = dtoUserId
        };
        var command = new Verify2FactorCookieCmd(dto)
        {
            PrincipalUserId = principalUserId
        };

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(principalUserId))
            .ReturnsAsync(user);

        _mock2FactorService
            .Setup(s => s.VerifyTwoFactorTokenAsync(team, user, token))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();

        // Verify that FindUserWithTeamDetailsAsync was called with principalUserId, not dtoUserId
        _mockFindUserService.Verify(
            s => s.FindUserWithTeamDetailsAsync(principalUserId),
            Times.Once);
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenServiceThrows()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(id: userId, team: team);
        var token = "valid-token";
        var deviceId = "device-123";
        var expectedException = new InvalidOperationException("Service failure");

        var dto = new Verify2FactorCookieDto { Token = token, DeviceId = deviceId, UserId = userId };
        var command = new Verify2FactorCookieCmd(dto);

        _mockFindUserService.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        _mock2FactorService
            .Setup(s => s.VerifyTwoFactorTokenAsync(team, user, token))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        exception.Message.ShouldBe(expectedException.Message);
    }

    //------------------------------//
}