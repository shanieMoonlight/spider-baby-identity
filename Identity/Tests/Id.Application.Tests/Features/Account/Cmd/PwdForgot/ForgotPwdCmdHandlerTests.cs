//using Helpers;
//using MyResults;
//using Id.Application.Tests.Utility.Mocks;
//using ID.Application.Abstractions.TokenServices;
//using ID.Application.Features.Account.Cmd.PwdForgot;
//using ID.Domain.Entities.AppUsers;
//using ID.Domain.Utility;
//using ID.IntegrationEvents;
//using ID.IntegrationEvents.Account.ForgotPwd;
//using Moq;
//using Shouldly;
//using ID.Application.Abstractions.TokenVerificationServices;

//namespace Id.Application.Tests.Account.ForgotPwd;
//public class ForgotPwdCmdHandlerTests
//{
//    public ForgotPwdCmdHandlerTests() { }

//    //------------------------------------//

//    [Fact]
//    public async Task Should_ReturnFailureWhenNotDataSupplied()
//    {
//        //Arrange
//        var _mockFindUserService = MockFindUserService.GetSuccessfulService();

//        var _mockBus = new Mock<IEventBus>();

//        var _mockTknService = new Mock<IPwdResetService<AppUser>>();
//        _mockTknService.Setup(um => um.ConfirmEmailAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
//            .ReturnsAsync(BasicResult.Success());

//#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
//        var cmd = new ForgotPwdCmd(null);
//#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

//        ForgotPwdHandler handler = new(
//          _mockFindUserService.Object,
//          _mockTknService.Object,
//          _mockBus.Object
//          );

//        //Act
//        var result = await handler.Handle(cmd, default);

//        //Assert
//        result.Succeeded.ShouldBeFalse();
//        result.Info.ShouldBe(IDMsgs.Error.NO_DATA_SUPPLIED);

//    }

//    //------------------------------------//

//    [Fact]
//    public async Task Should_ReturnNotFoundResultWhenUserNotFound()
//    {
//        //Arrange
//        var _mockFindUserService = MockFindUserService.GetFailureService();

//        var _mockBus = new Mock<IEventBus>();

//        var _mockTknService = new Mock<IPwdResetService<AppUser>>();
//        _mockTknService.Setup(um => um.ConfirmEmailAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
//            .ReturnsAsync(BasicResult.Success());

//        var cmd = new ForgotPwdCmd(
//            new ForgotPwdDto()
//            {
//                Email = "r@s.tu"
//            });

//        ForgotPwdHandler handler = new(
//          _mockFindUserService.Object,
//          _mockTknService.Object,
//          _mockBus.Object
//          );

//        //Act
//        var result = await handler.Handle(cmd, default);

//        //Assert
//        result.Succeeded.ShouldBeFalse();
//        result.NotFound.ShouldBeTrue();
//        result.Info.ShouldBe(IDMsgs.Error.Authorization.INVALID_AUTH);

//    }

//    //------------------------------------//

//    [Fact]
//    public async Task Should_ReturnSuccessWhenChangeSucceeds()
//    {
//        //Arrange
//        var _mockFindUserService = MockFindUserService.GetSuccessfulService();

//        var _mockBus = new Mock<IEventBus>();

//        var _mockTknService = new Mock<IPwdResetService<AppUser>>();
//        _mockTknService.Setup(um => um.ConfirmEmailAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
//            .ReturnsAsync(BasicResult.Success());

//        var cmd = new ForgotPwdCmd(
//            new ForgotPwdDto()
//            {
//                Email = "r@s.tu",
//            });


//        ForgotPwdHandler handler = new(
//          _mockFindUserService.Object,
//          _mockTknService.Object,
//          _mockBus.Object
//          );

//        //Act
//        var result = await handler.Handle(cmd, default);

//        //Assert
//        _mockBus.Verify(x =>
//                x.Publish(It.IsAny<ForgotPwdEmailRequestEvent>(), default),
//                Times.Once
//            );
//        _mockTknService.Verify(x =>
//                x.GenerateSafePasswordResetTokenAsync(It.IsAny<AppUser>()),
//                Times.Once
//            );

//        result.Succeeded.ShouldBeTrue();
//        result.Info.ShouldBe(IDMsgs.Info.PASSWORD_RESET);

//    }

//    //------------------------------------//


//}//Cls
