using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppEmailComplete;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppEmailComplete;

public class TwoFactorAuthAppEmailCompleteCmdHandlerTests
{
    private readonly Mock<ITwoFactorCompleteRegistrationHandler> _googleAuthMock;
    private readonly TwoFactorAuthAppEmailCompleteCmdHandler _handler;

    public TwoFactorAuthAppEmailCompleteCmdHandlerTests()
    {
        _googleAuthMock = new Mock<ITwoFactorCompleteRegistrationHandler>();
        _handler = new TwoFactorAuthAppEmailCompleteCmdHandler(_googleAuthMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenEnableAsyncSucceeds()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var request = new TwoFactorAuthAppEmailCompleteCmd("valid-code") { PrincipalUser = user };
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
        var user = AppUserDataFactory.Create(team: team);
        var request = new TwoFactorAuthAppEmailCompleteCmd("invalid-code") { PrincipalUser = user };
        var enableResult = BasicResult.Failure("Invalid 2FA code");

        _googleAuthMock.Setup(x => x.EnableAsync(user, request.TwoFactorCode))
            .ReturnsAsync(enableResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_CODE);
    }

    //------------------------------------//

}//Cls