using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.ForgotPwd;
using ID.IntegrationEvents.Events.Account.Subscriptions;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.Features.Account.Cmd.PwdForgot;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Domain.Utility.Messages;
using ID.Domain.Models;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Tests.Features.Account.Cmd.PwdForgot;

public class ForgotPwdHandlerTests
{
    private readonly Mock<IFindUserService<AppUser>> _mockFindUserService;
    private readonly Mock<IPwdResetService<AppUser>> _mockPwdResetService;
    private readonly Mock<IEventBus> _mockEventBus;
    private readonly ForgotPwdHandler _handler;

    public ForgotPwdHandlerTests()
    {
        _mockFindUserService = new Mock<IFindUserService<AppUser>>();
        _mockPwdResetService = new Mock<IPwdResetService<AppUser>>();
        _mockEventBus = new Mock<IEventBus>();
        _handler = new ForgotPwdHandler(_mockFindUserService.Object, _mockPwdResetService.Object, _mockEventBus.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_UserNotFound_ShouldReturnNotFoundResult()
    {
        // Arrange
        var dto = new ForgotPwdDto { Email = "test@example.com" };
        var command = new ForgotPwdCmd(dto);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync((AppUser)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);


        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_UserWithoutEmail_ShouldReturnNotFoundResult()
    {
        // Arrange
        var user = AppUserDataFactory.Create(Guid.NewGuid(), email: "", twoFactorProvider: TwoFactorProvider.Sms);
        var dto = new ForgotPwdDto { Email = null };
        var command = new ForgotPwdCmd(dto);
        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Email.USER_WITHOUT_EMAIL);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ValidUser_ShouldPublishEventAndReturnSuccessResult()
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: TeamType.customer);
        var user = AppUserDataFactory.Create(
            team: team,
            email: "test@example.com");

        var dto = new ForgotPwdDto { Email = user.Email };
        var command = new ForgotPwdCmd(dto);
        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);
        _mockPwdResetService.Setup(x => x.GenerateSafePasswordResetTokenAsync(It.IsAny<Team>(), It.IsAny<AppUser>()))
            .ReturnsAsync("safe-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Info.Passwords.PASSWORD_RESET);
        _mockEventBus.Verify(x => x.Publish(It.IsAny<ForgotPwdEmailRequestIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

}//Cls