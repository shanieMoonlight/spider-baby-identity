using Id.Application.Tests.Utility.Mocks;
using ID.Application.AppAbs.ApplicationServices;
using ID.Application.Features.Account.Cmd.ResendEmailConfirmation;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.ResendEmailConf;
public class ResendEmailConfCmdHandlerTests
{

    //------------------------------------//

    [Fact]
    public async Task Should_Return_SuccessWithMessage_WhenAlreadyConfirmed()
    {
        //Arrange
        var dbUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), emailConfirmed: true);
        var _mockFindUserService = MockFindUserService.GetSuccessfulService(dbUser);
        var _mockConfBus = new Mock<IEmailConfirmationBus>();
        var cmd = new ResendEmailConfirmationCmd(
            new ResendEmailConfirmationDto()
            {
                Email = dbUser.Email
            });

        ResendEmailConfirmationHandler handler = new(
          _mockFindUserService.Object,
          _mockConfBus.Object
          );

        //Act
        var result = await handler.Handle(cmd, default);

        //Assert
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Info.Email.EMAIL_ALREADY_CONFIRMED(dbUser.Email));

    }

    //------------------------------------//

    [Fact]
    public async Task Should_ReturnNotFoundResultWhenUserNotFound()
    {
        //Arrange
        var _mockFindUserService = MockFindUserService.GetFailureService();
        var _mockConfBus = new Mock<IEmailConfirmationBus>();
        var cmd = new ResendEmailConfirmationCmd(
            new ResendEmailConfirmationDto()
            {
                Email = "r@s.tu"
            });

        ResendEmailConfirmationHandler handler = new(
          _mockFindUserService.Object,
          _mockConfBus.Object
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
    public async Task Should_PublishEventWhenUserFound()
    {
        //Arrange
        var dbUser = AppUserDataFactory.Create();
        var _mockFindUserService = MockFindUserService.GetSuccessfulService(dbUser);
        var _mockConfBus = new Mock<IEmailConfirmationBus>();
        var cmd = new ResendEmailConfirmationCmd(
            new ResendEmailConfirmationDto()
            {
                Email = dbUser.Email
            });

        ResendEmailConfirmationHandler handler = new(
          _mockFindUserService.Object,
          _mockConfBus.Object);

        //Act
        var result = await handler.Handle(cmd, default);

        //Assert
        _mockConfBus.Verify(x =>
                x.GenerateTokenAndPublishEventAsync(It.IsAny<AppUser>(), It.IsAny<Team>(), default),
                Times.Once
            );


        result.Succeeded.ShouldBeTrue();
        result.NotFound.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Info.Email.EMAIL_CONFIRMATION_SENT(cmd.Dto.Email));

    }

    //------------------------------------//



}//Cls
