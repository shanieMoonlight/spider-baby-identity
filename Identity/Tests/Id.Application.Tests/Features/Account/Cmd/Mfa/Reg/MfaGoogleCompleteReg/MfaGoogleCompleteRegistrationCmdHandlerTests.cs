using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorGoogleComplete;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.Reg.MfaGoogleCompleteReg;

public class MfaGoogleCompleteRegistrationCmdHandlerTests
{
    private readonly Mock<IAuthenticatorAppService> _googleAuthMock;
    private readonly MfaGoogleCompleteRegistrationCmdHandler _handler;

    public MfaGoogleCompleteRegistrationCmdHandlerTests()
    {
        _googleAuthMock = new Mock<IAuthenticatorAppService>();
        _handler = new MfaGoogleCompleteRegistrationCmdHandler(_googleAuthMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenEnableAsyncSucceeds()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var request = new MfaGoogleCompleteRegistrationCmd("valid-code") { PrincipalUser = user };
        var enableResult = BasicResult.Success();

        _googleAuthMock.Setup(x => x.EnableAsync(user, request.TwoFactorCode))
            .ReturnsAsync(enableResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.UserName.ShouldBe(user.UserName);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEnableAsyncFails()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team:team);
        var request = new MfaGoogleCompleteRegistrationCmd("invalid-code") { PrincipalUser = user };
        var enableResult = BasicResult.Failure("Invalid 2FA code");

        _googleAuthMock.Setup(x => x.EnableAsync(user, request.TwoFactorCode))
            .ReturnsAsync(enableResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_TOKEN);
    }

    //------------------------------------//

}//Cls