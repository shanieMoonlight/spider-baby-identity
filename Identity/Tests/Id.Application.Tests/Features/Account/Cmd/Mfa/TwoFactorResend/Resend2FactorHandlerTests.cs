using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;
using ID.Application.MFA;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorResend;

public class Resend2FactorHandlerTests
{
    private readonly Mock<ITwoFactorMsgService> _mockTwoFactorMsgHandler;
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _mock2FactorService;
    private readonly Mock<IFindUserService<AppUser>> _mockFindUserService;
    private readonly Mock<ITwofactorUserIdCacheService> _mock2FactorUserIdCache;
    private readonly Resend2FactorHandler _handler;

    public Resend2FactorHandlerTests()
    {
        _mockTwoFactorMsgHandler = new Mock<ITwoFactorMsgService>();
        _mock2FactorService = new Mock<ITwoFactorVerificationService<AppUser>>();
        _mock2FactorUserIdCache = new Mock<ITwofactorUserIdCacheService>();
        _mockFindUserService = new Mock<IFindUserService<AppUser>>();

        _handler = new Resend2FactorHandler(
            _mockTwoFactorMsgHandler.Object,
            _mock2FactorUserIdCache.Object,
            _mockFindUserService.Object,
            _mock2FactorService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_TwoFactorNotEnabled_ShouldReturnBadRequestResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(teamId: teamId);
        var dto = new Resend2FactorDto { Email = "test@example.com", Username = "testuser", UserId = user.Id };
        var request = new Resend2FactorCmd(dto);

        _mock2FactorService.Setup(x => x.IsTwoFactorEnabledAsync(user)).ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<MfaResultData>>();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBeNull();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Success_ShouldReturnTotpResultData()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = TeamDataFactory.Create(id: teamId);
        var user = AppUserDataFactory.Create(team: team);
        var dto = new Resend2FactorDto { Email = "test@example.com", Username = "testuser", UserId = user.Id };
        var request = new Resend2FactorCmd(dto);
        var totpResultData = MfaResultData.Create(TwoFactorProvider.AuthenticatorApp);

        _mock2FactorUserIdCache.Setup(x => x.GetUserId(dto.Token)).Returns(user.Id);

        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(user.Id))
            .ReturnsAsync(user);

        _mock2FactorService.Setup(x => x.IsTwoFactorEnabledAsync(user)).ReturnsAsync(true);

        _mockTwoFactorMsgHandler.Setup(x => x.SendOTPFor2FactorAuth(team, user, null))
            .ReturnsAsync(GenResult<MfaResultData>.Success(totpResultData));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<MfaResultData>>();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(totpResultData);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_UserOrTeamIsNull_ShouldReturnBadRequestResult()
    {
        // Arrange
        var dto = new Resend2FactorDto { Token = "invalid-token" };
        var request = new Resend2FactorCmd(dto);
        _mock2FactorUserIdCache.Setup(x => x.GetUserId(dto.Token)).Returns((Guid?)null);
        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(null)).ReturnsAsync((AppUser?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<MfaResultData>>();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldNotBeNull();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_TeamIsNull_ShouldReturnBadRequestResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(id: userId, team: null);
        var dto = new Resend2FactorDto { Token = "valid-token" };
        var request = new Resend2FactorCmd(dto);
        _mock2FactorUserIdCache.Setup(x => x.GetUserId(dto.Token)).Returns(userId);
        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<MfaResultData>>();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldNotBeNull();
    }
}