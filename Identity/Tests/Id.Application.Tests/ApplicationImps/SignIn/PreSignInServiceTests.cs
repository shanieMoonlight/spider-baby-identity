using ID.Application.AppAbs.ApplicationServices;
using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.FromApp;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppImps.SignIn;
using ID.Application.Features.Account.Cmd.Login;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.ApplicationImps.SignIn;

public class PreSignInServiceTests
{
    private readonly Mock<IIdUserMgmtService<AppUser>> _userMgrMock;
    private readonly Mock<IFindUserService<AppUser>> _findUserServiceMock;
    private readonly Mock<IEmailConfirmationBus> _emailConfirmationBusMock;
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _2FactorServiceMock;
    private readonly Mock<ITwoFactorMsgService> _twoFactorMsgServiceMock;
    private readonly Mock<IIsFromMobileApp> _fromAppServiceMock;
    private readonly PreSignInService<AppUser> _preSignInService;

    //- - - - - - - - - - - - - - - - - - - -//

    public PreSignInServiceTests()
    {
        _userMgrMock = new Mock<IIdUserMgmtService<AppUser>>();
        _findUserServiceMock = new Mock<IFindUserService<AppUser>>();
        _emailConfirmationBusMock = new Mock<IEmailConfirmationBus>();
        _2FactorServiceMock = new Mock<ITwoFactorVerificationService<AppUser>>();
        _twoFactorMsgServiceMock = new Mock<ITwoFactorMsgService>();
        _fromAppServiceMock = new Mock<IIsFromMobileApp>();

        _preSignInService = new PreSignInService<AppUser>(
            _userMgrMock.Object,
            _findUserServiceMock.Object,
            _emailConfirmationBusMock.Object,
            _2FactorServiceMock.Object,
            _twoFactorMsgServiceMock.Object,
            _fromAppServiceMock.Object
        );
    }

    //---------------------------------------//

    [Fact]
    public async Task Authenticate_UserNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com" };
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _findUserServiceMock.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync((AppUser)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.NotFound.ShouldBeTrue();
    }

    //---------------------------------------//

    [Fact]
    public async Task Authenticate_EmailNotConfirmed_ReturnsEmailConfirmedRequiredResult()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var loginDto = new LoginDto { Email = user.Email };
        _findUserServiceMock.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);
        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(false);

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.EmailConfirmationRequired.ShouldBeTrue();
    }

    //---------------------------------------//

    [Fact]
    public async Task Authenticate_InvalidPassword_ReturnsUnauthorizedResult()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var loginDto = new LoginDto { Email = user.Email, Password = "wrongpassword" };
        _findUserServiceMock.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);
        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _userMgrMock.Setup(s => s.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.Unauthorized.ShouldBeTrue();
    }

    //---------------------------------------//

    [Fact]
    public async Task Authenticate_TwoFactorEnabled_MsgSuccess_ReturnsTwoFactorRequiredResult()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var loginDto = new LoginDto { Email = user.Email, Password = "correctpassword" };
        _findUserServiceMock.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);
        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _userMgrMock.Setup(s => s.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
        _2FactorServiceMock.Setup(s => s.IsTwoFactorEnabledAsync(user)).ReturnsAsync(true);
        _fromAppServiceMock.Setup(s => s.IsFromApp).Returns(false);
        MfaResultData mfaResultData = MfaResultData.Create(user.TwoFactorProvider, "otp-extyra-info");

        _twoFactorMsgServiceMock.Setup(s => s.SendOTPFor2FactorAuth(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<TwoFactorProvider?>()))
            .ReturnsAsync(GenResult<MfaResultData>.Success(MfaResultData.Create(user.TwoFactorProvider, "otp-extyra-info")));

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.TwoFactorRequired.ShouldBeTrue();
    }

    //---------------------------------------//

    [Fact]
    public async Task Authenticate_TwoFactorEnabled_MsgFailed_ReturnsTwoFactorRequiredResult()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var loginDto = new LoginDto { Email = user.Email, Password = "correctpassword" };
        _findUserServiceMock.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);
        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _userMgrMock.Setup(s => s.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
        _2FactorServiceMock.Setup(s => s.IsTwoFactorEnabledAsync(user)).ReturnsAsync(true);
        _fromAppServiceMock.Setup(s => s.IsFromApp).Returns(false);

        _twoFactorMsgServiceMock.Setup(s => s.SendOTPFor2FactorAuth(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<TwoFactorProvider?>()))
            .ReturnsAsync(GenResult<MfaResultData>.Failure("Something went wrong"));

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.TwoFactorRequired.ShouldBeFalse();
        result.Succeeded.ShouldBeFalse();
    }

    //---------------------------------------//

    [Fact]
    public async Task Authenticate_Success_ReturnsSuccessResult()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var loginDto = new LoginDto { Email = user.Email, Password = "correctpassword" };
        _findUserServiceMock.Setup(s => s.FindUserWithTeamDetailsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);
        _userMgrMock.Setup(s => s.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _userMgrMock.Setup(s => s.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
        _2FactorServiceMock.Setup(s => s.IsTwoFactorEnabledAsync(user)).ReturnsAsync(false);
        MfaResultData mfaResultData = MfaResultData.Create(user.TwoFactorProvider, "otp-extyra-info");

        _twoFactorMsgServiceMock.Setup(s => s.SendOTPFor2FactorAuth(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<TwoFactorProvider?>()))
            .ReturnsAsync(GenResult<MfaResultData>.Success(mfaResultData));

        // Act
        var result = await _preSignInService.Authenticate(loginDto, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.User.ShouldBe(user);
        result.Team.ShouldBe(team);
    }

    //---------------------------------------//

}//Cls
