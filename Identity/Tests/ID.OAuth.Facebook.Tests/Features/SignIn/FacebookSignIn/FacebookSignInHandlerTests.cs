using System.Threading;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Xunit;
using MyResults;
using ID.OAuth.Facebook.Features.SignIn.FacebookSignIn;
using ID.OAuth.Facebook.Services.Abs;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using ID.Application.JWT;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.Application.AppAbs.ApplicationServices.TwoFactor;

namespace ID.OAuth.Facebook.Tests.Features.SignIn.FacebookSignIn;

public class FacebookSignInHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnJwtPackage_WhenTwoFactorDisabled()
    {
        // Arrange
        var dto = new FacebookSignInDto { AuthToken = "token", DeviceId = "dev-1" };
        var cmd = new FacebookSignInCmd(dto);

        var mockFindOrCreate = new Mock<IFindOrCreateService<AppUser>>();
        var mockJwtProvider = new Mock<IJwtPackageProvider>();
        var mockVerifier = new Mock<IFacebookAuthenticationService>();
        var mock2FactorService = new Mock<Application.AppAbs.TokenVerificationServices.ITwoFactorVerificationService<AppUser>>();
        var mockTwoFactorMsg = new Mock<ITwoFactorMsgService>();

        var user = AppUserDataFactory.AnyUser;
        var team = user.Team!;

        mockVerifier.Setup(v => v.VerifyAndGetProfileAsync(dto.AuthToken, dto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<FacebookUserProfile>.Success(new FacebookUserProfile { Id = "u1", Email = "a@b.com", Name = "Name" }));

        mockFindOrCreate.Setup(f => f.FindOrCreateUserAsync(It.IsAny<FacebookUserProfile>(), dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(user));

        mock2FactorService.Setup(x => x.IsTwoFactorEnabledAsync(user)).ReturnsAsync(false);

        var expectedJwt = JwtPackage.Create("access", 999999, TwoFactorProvider.Sms, "refresh");
        mockJwtProvider.Setup(j => j.CreateJwtPackageAsync(user, user.Team!, dto.DeviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJwt);

        var handler = new FacebookSignInHandler(
            mockFindOrCreate.Object,
            mockJwtProvider.Object,
            mockVerifier.Object,
            mock2FactorService.Object,
            mockTwoFactorMsg.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.AccessToken.ShouldBe(expectedJwt.AccessToken);
        mockJwtProvider.Verify(j => j.CreateJwtPackageAsync(user, user.Team!, dto.DeviceId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnTwoFactorJwt_WhenTwoFactorEnabled()
    {
        // Arrange
        var dto = new FacebookSignInDto { AuthToken = "token", DeviceId = "dev-2" };
        var cmd = new FacebookSignInCmd(dto);

        var mockFindOrCreate = new Mock<IFindOrCreateService<AppUser>>();
        var mockJwtProvider = new Mock<IJwtPackageProvider>();
        var mockVerifier = new Mock<IFacebookAuthenticationService>();
        var mock2FactorService = new Mock<Application.AppAbs.TokenVerificationServices.ITwoFactorVerificationService<AppUser>>();
        var mockTwoFactorMsg = new Mock<ITwoFactorMsgService>();

        var user = AppUserDataFactory.AnyUser;
        var team = user.Team!;

        mockVerifier.Setup(v => v.VerifyAndGetProfileAsync(dto.AuthToken, dto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<FacebookUserProfile>.Success(new FacebookUserProfile { Id = "u1", Email = "a@b.com", Name = "Name" }));

        mockFindOrCreate.Setup(f => f.FindOrCreateUserAsync(It.IsAny<FacebookUserProfile>(), dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(user));

        mock2FactorService.Setup(x => x.IsTwoFactorEnabledAsync(user)).ReturnsAsync(true);

        mockTwoFactorMsg.Setup(x => x.SendOTPFor2FactorAuth(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<TwoFactorProvider?>()))
            .ReturnsAsync(GenResult<MfaResultData>.Success(MfaResultData.Create(TwoFactorProvider.Email)));

        var twoFactorJwt = JwtPackage.CreateWithTwoFactoRequired("tkn", 12345, TwoFactorProvider.Email);
        mockJwtProvider.Setup(j => j.CreateJwtPackageWithTwoFactorRequiredAsync(user, TwoFactorProvider.Email, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(twoFactorJwt);

        var handler = new FacebookSignInHandler(
            mockFindOrCreate.Object,
            mockJwtProvider.Object,
            mockVerifier.Object,
            mock2FactorService.Object,
            mockTwoFactorMsg.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TwoFactorVerificationRequired.ShouldBeTrue();
        mockTwoFactorMsg.Verify(x => x.SendOTPFor2FactorAuth(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<TwoFactorProvider?>()), Times.Once);
        mockJwtProvider.Verify(j => j.CreateJwtPackageWithTwoFactorRequiredAsync(user, TwoFactorProvider.Email, It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}