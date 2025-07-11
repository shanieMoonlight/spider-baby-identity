using Id.Application.Tests.Utility.Mocks;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Features.Account.Cmd.ConfirmEmailWithPwd;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using Moq;
using MyResults;
using Shouldly;



namespace ID.Application.Tests.Features.Account.Cmd.ConfirmEmailWithPwd;
public class ConfirmEmailWithPwdCmdHandlerTests
{
    public ConfirmEmailWithPwdCmdHandlerTests() { }

    //------------------------------------//

    [Fact]
    public async Task Should_ReturnNotFoundResultWhenUserNotFound()
    {
        //Arrange
        var _mockFindUserService = MockFindUserService.GetFailureService();
        var _mockTknService = new Mock<IEmailConfirmationService<AppUser>>();
        var cmd = new ConfirmEmailWithPwdCmd(
            new ConfirmEmailWithPwdDto(new Guid(), "sfgyiyujdfbgst", "NEWPWD", "NEWPWD"));

        ConfirmEmailWithPwdCmdHandler handler = new(
          _mockFindUserService.Object,
          _mockTknService.Object
          );

        //Act
        var result = await handler.Handle(cmd, default);

        //Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH);
    }

    //------------------------------------//

    [Fact]
    public async Task Should_CallConfirmEmailAsyncWhenUerFound()
    {
        //Arrange
        var _mockFindUserService = MockFindUserService.GetSuccessfulService();

        var _mockTknService = new Mock<IEmailConfirmationService<AppUser>>();
        _mockTknService.Setup(um => um.ConfirmEmailWithPasswordAsync(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(BasicResult.Success());
        var cmd = new ConfirmEmailWithPwdCmd(
           new ConfirmEmailWithPwdDto(new Guid(), "sfgyiyujdfbgst", "NEWPWD", "NEWPWD"));
        ConfirmEmailWithPwdCmdHandler handler = new(
          _mockFindUserService.Object,
          _mockTknService.Object
          );

        //Act
        var result = await handler.Handle(cmd, default);

        //Assert
        _mockTknService.Verify(x =>
                x.ConfirmEmailWithPasswordAsync(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once
            );

    }

    //------------------------------------//

    [Fact]
    public async Task Should_ReturnSuccessWhenConfirmSucceeds()
    {
        //Arrange
        var _mockFindUserService = MockFindUserService.GetSuccessfulService();

        var _mockTknService = new Mock<IEmailConfirmationService<AppUser>>();
        _mockTknService.Setup(um => um.ConfirmEmailWithPasswordAsync(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(BasicResult.Success());
        var cmd = new ConfirmEmailWithPwdCmd(
           new ConfirmEmailWithPwdDto(new Guid(), "sfgyiyujdfbgst", "NEWPWD", "NEWPWD"));

        ConfirmEmailWithPwdCmdHandler handler = new(
          _mockFindUserService.Object,
          _mockTknService.Object
          );

        //Act
        var result = await handler.Handle(cmd, default);

        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Info.Email.EMAIL_CONFIRMED);

    }

    //------------------------------------//

    [Fact]
    public async Task Should_ReturnFailureWhenConfirmFails()
    {
        //Arrange
        var _mockFindUserService = MockFindUserService.GetSuccessfulService();

        var _mockTknService = new Mock<IEmailConfirmationService<AppUser>>();
        _mockTknService.Setup(um => um.ConfirmEmailWithPasswordAsync(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(BasicResult.Failure());
        var cmd = new ConfirmEmailWithPwdCmd(
           new ConfirmEmailWithPwdDto(new Guid(), "sfgyiyujdfbgst", "NEWPWD", "NEWPWD"));

        ConfirmEmailWithPwdCmdHandler handler = new(
          _mockFindUserService.Object,
          _mockTknService.Object
          );

        //Act
        var result = await handler.Handle(cmd, default);

        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldNotBe(IDMsgs.Info.Email.EMAIL_CONFIRMED);

    }

    //------------------------------------//


}//Cls
