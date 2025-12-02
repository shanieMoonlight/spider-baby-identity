using ID.Application.JWT;
using ID.Domain.Claims.AuthMethods;
using ID.OAuth.Amazon.Features.SignIn;
using ID.OAuth.Amazon.Features.SignIn.AmazonSignIn;

namespace ID.OAuth.Amazon.Tests.Features.SignIn.AmazonSignIn;

public class AmazonSignInHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnJwtPackage_WhenTwoFactorDisabled()
    {
        // Arrange
        var dto = new AmazonSignInDto { AuthToken = "token", DeviceId = "dev-1" };
        var cmd = new AmazonSignInCmd(dto);

        var mockFindOrCreate = new Mock<IFindOrCreateService<AppUser>>();
        var mockJwtProvider = new Mock<IJwtPackageProvider>();
        var mockVerifier = new Mock<IAmazonAuthenticationService>();
        var mock2FactorService = new Mock<Application.AppAbs.TokenVerificationServices.ITwoFactorVerificationService<AppUser>>();
        var mockTwoFactorMsg = new Mock<ITwoFactorMsgService>();
        var authMethods = new List<AuthMethodRef> { AuthMethodRef.oauth };

        var user = AppUserDataFactory.AnyUser;
        var team = user.Team!;

        mockVerifier.Setup(v => v.VerifyAndGetProfileAsync(dto.AuthToken, dto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonUserProfile>.Success(new AmazonUserProfile { UserId = "u1", Email = "a@b.com", Name = "Name" }));

        mockFindOrCreate.Setup(f => f.FindOrCreateUserAsync(It.IsAny<AmazonUserProfile>(), dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(user));

        mock2FactorService.Setup(x => x.IsTwoFactorEnabledAsync(user)).ReturnsAsync(false);

        var expectedJwt = JwtPackage.Create("access", 999999, TwoFactorProvider.Sms, "refresh");
        mockJwtProvider.Setup(j => j.CreateJwtPackageAsync(user, user.Team!, authMethods, dto.DeviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedJwt);

        var handler = new AmazonSignInHandler(
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
        mockJwtProvider.Verify(j => j.CreateJwtPackageAsync(user, user.Team!, authMethods,  dto.DeviceId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnTwoFactorJwt_WhenTwoFactorEnabled()
    {
        // Arrange
        var dto = new AmazonSignInDto { AuthToken = "token", DeviceId = "dev-2" };
        var cmd = new AmazonSignInCmd(dto);

        var mockFindOrCreate = new Mock<IFindOrCreateService<AppUser>>();
        var mockJwtProvider = new Mock<IJwtPackageProvider>();
        var mockVerifier = new Mock<IAmazonAuthenticationService>();
        var mock2FactorService = new Mock<Application.AppAbs.TokenVerificationServices.ITwoFactorVerificationService<AppUser>>();
        var mockTwoFactorMsg = new Mock<ITwoFactorMsgService>();

        var user = AppUserDataFactory.AnyUser;
        var team = user.Team!;

        mockVerifier.Setup(v => v.VerifyAndGetProfileAsync(dto.AuthToken, dto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AmazonUserProfile>.Success(new AmazonUserProfile { UserId = "u1", Email = "a@b.com", Name = "Name" }));

        mockFindOrCreate.Setup(f => f.FindOrCreateUserAsync(It.IsAny<AmazonUserProfile>(), dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(user));

        mock2FactorService.Setup(x => x.IsTwoFactorEnabledAsync(user)).ReturnsAsync(true);

        mockTwoFactorMsg.Setup(x => x.SendOTPFor2FactorAuth(It.IsAny<Team>(), It.IsAny<AppUser>(), It.IsAny<TwoFactorProvider?>()))
            .ReturnsAsync(GenResult<MfaResultData>.Success(MfaResultData.Create(TwoFactorProvider.Email)));

        var twoFactorJwt = JwtPackage.CreateWithTwoFactoRequired("tkn", 12345, TwoFactorProvider.Email);
        mockJwtProvider.Setup(j => j.CreateJwtPackageWithTwoFactorRequiredAsync(user, TwoFactorProvider.Email, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(twoFactorJwt);

        var handler = new AmazonSignInHandler(
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
