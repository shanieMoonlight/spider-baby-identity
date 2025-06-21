using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorVerify;
using ID.Application.JWT;
using ID.Application.Tests.Utility;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Setup.Options;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorVerify;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
[Collection(TestingConstants.NonParallelCollection)]
public class Verify2FactorHandlerTests
{
    private readonly Mock<IJwtPackageProvider> _mockPackageProvider;
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _mock2FactorService;
    private readonly Mock<IFindUserService<AppUser>> _mockFindUserService;
    private readonly Mock<IJwtRefreshTokenService<AppUser>> _mockRefreshProvider;


    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions_refreshEnabled;
    private readonly Mock<IOptions<IdGlobalOptions>> _mockGlobalOptions_refreshDisabled;
    private Verify2FactorHandler _handler_RefreshEnabled;
    private readonly Verify2FactorHandler _handler_RefreshDisabled;

    private readonly IdGlobalOptions _globalOptions_RefreshEnabled = GlobalOptionsUtils.InitiallyValidOptions(
            refreshTokensEnabled: true);
    private readonly IdGlobalOptions _globalOptions_RefreshDisabled = GlobalOptionsUtils.InitiallyValidOptions(
            refreshTokensEnabled: false);




    public Verify2FactorHandlerTests()
    {
        _mockPackageProvider = new Mock<IJwtPackageProvider>();
        _mock2FactorService = new Mock<ITwoFactorVerificationService<AppUser>>();
        _mockRefreshProvider = new Mock<IJwtRefreshTokenService<AppUser>>();
        _mockFindUserService = new Mock<IFindUserService<AppUser>>();

        _mockGlobalOptions_refreshEnabled = new Mock<IOptions<IdGlobalOptions>>();
        _mockGlobalOptions_refreshEnabled.Setup(x => x.Value).Returns(_globalOptions_RefreshEnabled);
        _handler_RefreshEnabled = new Verify2FactorHandler(
            _mockPackageProvider.Object,
            _mockFindUserService.Object,
            _mock2FactorService.Object);



        _mockGlobalOptions_refreshDisabled = new Mock<IOptions<IdGlobalOptions>>();
        _mockGlobalOptions_refreshDisabled.Setup(x => x.Value).Returns(_globalOptions_RefreshDisabled);
        _handler_RefreshDisabled = new Verify2FactorHandler(
            _mockPackageProvider.Object,
            _mockFindUserService.Object,
            _mock2FactorService.Object);

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
        result.Info.ShouldBe(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_TOKEN);
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

        _mockRefreshProvider
            .Setup(r => r.GenerateTokenAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);

        _mockPackageProvider
            .Setup(p => p.CreateJwtPackageAsync(
                user,
                user.Team!,
                true,
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
                true,
                deviceId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler_RefreshDisabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);
        _mockRefreshProvider.Verify(
            r => r.GenerateTokenAsync(It.IsAny<AppUser>(), It.IsAny<CancellationToken>()),
            Times.Never);

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

        var dto = new Verify2FactorDto { Code = token, DeviceId = deviceId, UserId = Guid.NewGuid() };
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
        var team = TeamDataFactory.Create();
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
        result.Unauthorized.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH);
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
            //PrincipalUser = null,
            //PrincipalTeam = team
        };

        // Act
        var result = await _handler_RefreshEnabled.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH);
    }

    //------------------------------//
}
