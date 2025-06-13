using Id.Application.Tests.Utility.Mocks;
using ID.Application.Features.Account.Cmd.PwdChange;
using ID.Application.Features.Account.Cmd.RefreshTokenRevoke;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using ID.Tests.Data.Factories.Dtos;
using Microsoft.AspNetCore.Identity;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.PwdCh;
public class ChPwdCmdHandlerTests
{
    public ChPwdCmdHandlerTests() { }

    //------------------------------------//


    [Fact]
    public async Task Should_ReturnUnauthorizedResultWhenPasswordIsWrong()
    {
        //Arrange
        //var _mockFindUserService = MockFindPrincipalService.GetSuccessfulService();

        var _mockUserMgmtService = new Mock<IIdUserMgmtService<AppUser>>();
        _mockUserMgmtService.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(true);
        _mockUserMgmtService.Setup(um => um.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string?>()))
            .ReturnsAsync(false);
        var cmd = new ChPwdCmd(
            new ChPwdDto()
            {
                Email = "r@s.tu",
                Password = "passofdkgoadfboword"
            });


        ChPwdHandler handler = new(_mockUserMgmtService.Object);

        //Act
        var result = await handler.Handle(cmd, default);

        //Assert
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH);
        _mockUserMgmtService.Verify(x =>
                x.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string?>()),
                Times.Once
            );
    }

    //------------------------------------//

    [Fact]
    public async Task Should_CallChangePasswordAsyncWhenPasswordIsCorrect()
    {
        //Arrange
        //var _mockFindUserService = MockFindPrincipalService.GetSuccessfulService();

        var _mockUserMgmtService = new Mock<IIdUserMgmtService<AppUser>>();
        _mockUserMgmtService.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(true);
        _mockUserMgmtService.Setup(um => um.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string?>()))
            .ReturnsAsync(true);

        _mockUserMgmtService.Setup(um => um.ChangePasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(GenResult<AppUser>.Success(AppUserDataFactory.Create()));

        var cmd = new ChPwdCmd(
            new ChPwdDto()
            {
                Email = "r@s.tu",
                Password = "passofdkgoadfboword"
            });

        ChPwdHandler handler = new(_mockUserMgmtService.Object);

        //Act
        var result = await handler.Handle(cmd, default);

        //Assert
        _mockUserMgmtService.Verify(x =>
                x.ChangePasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once
            );

    }

    //------------------------------------//

    [Fact]
    public async Task Should_ReturnBadRequestWhenChangeFails()
    {
        //Arrange
        //var _mockFindUserService = MockFindPrincipalService.GetSuccessfulService();

        var _mockUserMgmtService = new Mock<IIdUserMgmtService<AppUser>>();
        _mockUserMgmtService.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(true);
        _mockUserMgmtService.Setup(um => um.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string?>()))
            .ReturnsAsync(true);
        var cmd = new ChPwdCmd(
            new ChPwdDto()
            {
                Email = "r@s.tu",
                Password = "passofdkgoadfboword"
            });

        IdentityError[] errors = [
            new ( ){
               Code = "Code1",
               Description = "Description1"
            },
             new ( ){
               Code = "Code2",
               Description = "Description2"
            }
         ];

        var errorMsg = string.Join(
                 ", " + Environment.NewLine,
                errors.Select(err => "Code: " + err.Code + Environment.NewLine + err.Description));

        _mockUserMgmtService.Setup(um => um.ChangePasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()))
          .ReturnsAsync(GenResult<AppUser>.BadRequestResult(errorMsg)
        );



        ChPwdHandler handler = new(_mockUserMgmtService.Object);

        //Act
        var result = await handler.Handle(cmd, default);

        //Assert
        _mockUserMgmtService.Verify(x =>
                x.ChangePasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once
            );

        result.BadRequest.ShouldBeTrue();
        result.Succeeded.ShouldBeFalse();

    }

    //------------------------------------//

    [Fact]
    public async Task Should_ReturnSuccessWhenChangeSucceeds()
    {
        //Arrange
        //var _mockFindUserService = MockFindPrincipalService.GetSuccessfulService();

        var _mockUserMgmtService = new Mock<IIdUserMgmtService<AppUser>>();
        _mockUserMgmtService.Setup(um => um.IsEmailConfirmedAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(true);
        _mockUserMgmtService.Setup(um => um.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string?>()))
            .ReturnsAsync(true);
        var cmd = new ChPwdCmd(
            new ChPwdDto()
            {
                Email = "r@s.tu",
                Password = "passofdkgoadfboword"
            });

        _mockUserMgmtService.Setup(um => um.ChangePasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()))
          .ReturnsAsync(GenResult<AppUser>.Success(AppUserDataFactory.Create()));


        ChPwdHandler handler = new(_mockUserMgmtService.Object);

        //Act
        var result = await handler.Handle(cmd, default);

        //Assert
        _mockUserMgmtService.Verify(x =>
                x.ChangePasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once
            );
        result.BadRequest.ShouldBeFalse();
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Info.Passwords.PASSWORD_CHANGE_SUCCESSFUL);

    }

    //------------------------------------//


}//Cls
