using System.Threading;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Xunit;
using MyResults;
using ID.OAuth.Facebook.Features.SignIn.FacebookCookieSignIn;
using ID.OAuth.Facebook.Services.Abs;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using ID.Application.AppAbs.SignIn;
using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;

namespace ID.OAuth.Facebook.Tests.Features.SignIn.FacebookCookieSignIn;

public class FacebookCookieSignInCmdHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnSuccessAndCallSignIn_WhenTwoFactorDisabled()
    {
        // Arrange
        var dto = new FacebookCookieSignInDto { AuthToken = "token", RememberMe = true, DeviceId = "dev-1" };
        var cmd = new FacebookCookieSignInCmd(dto);

        var mockFindOrCreate = new Mock<IFindOrCreateService<AppUser>>();
        var mockCookieService = new Mock<ICookieAuthService<AppUser>>();
        var mockVerifier = new Mock<IFacebookAuthenticationService>();
        var mock2FactorService = new Mock<Application.AppAbs.TokenVerificationServices.ITwoFactorVerificationService<AppUser>>();
        var mockTwoFactorMsg = new Mock<ITwoFactorMsgService>();

        var user = AppUserDataFactory.AnyUser;
        var team = user.Team!;

        mockVerifier.Setup(v => v.VerifyAndGetProfileAsync(dto.AuthToken, dto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<FacebookUserProfile>.Success(new FacebookUserProfile { Id = "u1", Email = "a@b.com", Name = "Name" }));

        mockFindOrCreate.Setup(f => f.FindOrCreateUserAsync(It.IsAny<FacebookUserProfile>(), dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(user));

        mock2FactorService.Setup(x => x.IsTwoFactorEnabledAsync(user))
            .ReturnsAsync(false);

        var handler = new FacebookCookieSignInCmdHandler(
            mockFindOrCreate.Object,
            mockCookieService.Object,
            mockVerifier.Object,
            mock2FactorService.Object,
            mockTwoFactorMsg.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        mockCookieService.Verify(s => s.SignInAsync(true, user, user.Team!, dto.DeviceId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnPreconditionRequired_WhenTwoFactorEnabled()
    {
        // Arrange
        var dto = new FacebookCookieSignInDto { AuthToken = "token", RememberMe = false, DeviceId = "dev-2" };
        var cmd = new FacebookCookieSignInCmd(dto);

        var mockFindOrCreate = new Mock<IFindOrCreateService<AppUser>>();
        var mockCookieService = new Mock<ICookieAuthService<AppUser>>();
        var mockVerifier = new Mock<IFacebookAuthenticationService>();
        var mock2FactorService = new Mock<Application.AppAbs.TokenVerificationServices.ITwoFactorVerificationService<AppUser>>();
        var mockTwoFactorMsg = new Mock<ITwoFactorMsgService>();

        var user = AppUserDataFactory.AnyUser;
        var team = user.Team!;

        mockVerifier.Setup(v => v.VerifyAndGetProfileAsync(dto.AuthToken, dto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<FacebookUserProfile>.Success(new FacebookUserProfile { Id = "u1", Email = "a@b.com", Name = "Name" }));

        mockFindOrCreate.Setup(f => f.FindOrCreateUserAsync(It.IsAny<FacebookUserProfile>(), dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(user));

        mock2FactorService.Setup(x => x.IsTwoFactorEnabledAsync(user))
            .ReturnsAsync(true);

        mockTwoFactorMsg.Setup(x => x.SendOTPFor2FactorAuth(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<TwoFactorProvider?>()))
            .ReturnsAsync(GenResult<MfaResultData>.Success(MfaResultData.Create(TwoFactorProvider.Email)));

        var handler = new FacebookCookieSignInCmdHandler(
            mockFindOrCreate.Object,
            mockCookieService.Object,
            mockVerifier.Object,
            mock2FactorService.Object,
            mockTwoFactorMsg.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(BasicResult.ResultStatus.PreconditionRequired);
        mockTwoFactorMsg.Verify(x => x.SendOTPFor2FactorAuth(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<TwoFactorProvider?>()), Times.Once);
        mockCookieService.Verify(x => x.CreateWithTwoFactorRequiredAsync(false, user, dto.DeviceId), Times.Once);
    }
}
