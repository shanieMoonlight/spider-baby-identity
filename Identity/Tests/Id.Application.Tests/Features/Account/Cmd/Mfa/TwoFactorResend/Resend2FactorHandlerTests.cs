using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Domain.Utility.Messages;
using ID.Domain.Models;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorResend;

public class Resend2FactorHandlerTests
{
    private readonly Mock<ITwoFactorMsgService> _mockTwoFactorMsgHandler;
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _mock2FactorService;
    private readonly Resend2FactorHandler _handler;

    public Resend2FactorHandlerTests()
    {
        _mockTwoFactorMsgHandler = new Mock<ITwoFactorMsgService>();
        _mock2FactorService = new Mock<ITwoFactorVerificationService<AppUser>>();
        _handler = new Resend2FactorHandler(_mockTwoFactorMsgHandler.Object, _mock2FactorService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_TwoFactorNotEnabled_ShouldReturnBadRequestResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(teamId: teamId);
        var dto = new Resend2FactorDto { Email = "test@example.com", Username = "testuser", UserId = user.Id };
        var request = new Resend2FactorCmd(dto)
        {
            PrincipalUser = user
        };

        _mock2FactorService.Setup(x => x.IsTwoFactorEnabledAsync(user)).ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<MfaResultData>>();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.TwoFactor.NO_MSG_PROVIDER_SET);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Success_ShouldReturnTotpResultData()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = TeamDataFactory.Create(id: teamId);
        var user = AppUserDataFactory.Create( team: team);
        var dto = new Resend2FactorDto { Email = "test@example.com", Username = "testuser", UserId = user.Id };
        var request = new Resend2FactorCmd(dto)
        {
            PrincipalUser = user,
            PrincipalTeam = team
        };
        var totpResultData = MfaResultData.Create(TwoFactorProvider.AuthenticatorApp);
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

}//Cls