using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppComplete;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppComplete;

public class TwoFactorAuthAppCompleteRegCmdHandlerTests
{
    private readonly Mock<ITwoFactorCompleteRegistrationHandler> _mockRegCompleteHandler;
    private readonly TwoFactorAuthAppCompleteRegCmdHandler _handler;

    public TwoFactorAuthAppCompleteRegCmdHandlerTests()
    {
        _mockRegCompleteHandler = new Mock<ITwoFactorCompleteRegistrationHandler>();
        _handler = new TwoFactorAuthAppCompleteRegCmdHandler(_mockRegCompleteHandler.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Return_Success_When_TwoFactorCode_Is_Valid()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var dto = new TwoFactorAuthAppCompleteRegDto("123456", "secretKey");
        var request = new TwoFactorAuthAppCompleteRegCmd(dto)
        {
            PrincipalUser = user
        };

        _mockRegCompleteHandler.Setup(x => x.EnableAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.UserName.ShouldBe(user.UserName);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Return_Failure_When_TwoFactorCode_Is_Invalid()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var request = new TwoFactorAuthAppCompleteRegCmd(new TwoFactorAuthAppCompleteRegDto("invalidCode", "secretKey"))
        {
            PrincipalUser = user
        };

        _mockRegCompleteHandler.Setup(x => x.EnableAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(BasicResult.Failure("Invalid two-factor code"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.TwoFactor.INVALID_2_FACTOR_TOKEN);
    }

    //------------------------------------//

}//Cls