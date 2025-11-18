using ClArch.ValueObjects;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.Customers.Abstractions;
using ID.Domain.Entities.AppUsers.OAuth;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.OAuth.Amazon.Features.SignIn;
using ID.OAuth.Amazon.Services.Imps;

namespace ID.OAuth.Amazon.Tests.Services;

public class FindOrCreateServiceTests
{
    [Fact]
    public async Task FindOrCreateUserAsync_ReturnsExistingUser_WhenFound()
    {
        // Arrange
        var existing = AppUserDataFactory.AnyUser;

        var mockFind = new Mock<IFindUserService<AppUser>>();
        mockFind.Setup(f => f.FindUserWithTeamDetailsAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
            .ReturnsAsync(existing);

        var mockSignup = new Mock<IIdCustomerRegistrationService>();

        var svc = new FindOrCreateService<AppUser>(mockFind.Object, mockSignup.Object);

        var profile = new AmazonUserProfile { Email = "a@b.com", Name = "Name", UserId = "uid" };
        var dto = new AmazonSignInDto { Email = null };

        // Act
        var res = await svc.FindOrCreateUserAsync(profile, dto, CancellationToken.None);

        // Assert
        res.Succeeded.ShouldBeTrue();
        res.Value.ShouldBe(existing);
        mockSignup.Verify(s => s.RegisterOAuthAsync(It.IsAny<EmailAddress>(),
            It.IsAny<UsernameNullable>(),
            It.IsAny<PhoneNullable>(),
            It.IsAny<FirstNameNullable>(),
            It.IsAny<LastNameNullable>(),
            It.IsAny<TeamPositionNullable>(),
            It.IsAny<OAuthInfo>(),
            It.IsAny<Guid?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    //-----------------------//   

    [Fact]
    public async Task FindOrCreateUserAsync_ReturnsFailure_WhenNoEmailProvided()
    {
        // Arrange
        var mockFind = new Mock<IFindUserService<AppUser>>();
        mockFind.Setup(f => f.FindUserWithTeamDetailsAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
            .ReturnsAsync((AppUser?)null);

        var mockSignup = new Mock<IIdCustomerRegistrationService>();

        var svc = new FindOrCreateService<AppUser>(mockFind.Object, mockSignup.Object);

        var profile = new AmazonUserProfile { Email = null, Name = "Name", UserId = "uid" };
        var dto = new AmazonSignInDto { Email = null };

        // Act
        var res = await svc.FindOrCreateUserAsync(profile, dto, CancellationToken.None);

        // Assert
        res.Succeeded.ShouldBeFalse();
        res.Status.ShouldBe(BasicResult.ResultStatus.Failure);
        mockSignup.Verify(s => s.RegisterOAuthAsync(It.IsAny<EmailAddress>(),
            It.IsAny<UsernameNullable>(),
            It.IsAny<PhoneNullable>(),
            It.IsAny<FirstNameNullable>(),
            It.IsAny<LastNameNullable>(),
            It.IsAny<TeamPositionNullable>(),
            It.IsAny<OAuthInfo>(),
            It.IsAny<Guid?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    //-----------------------//   

    [Fact]
    public async Task FindOrCreateUserAsync_CallsRegisterOAuth_WithAmazonOAuthInfo_WhenEmailPresent()
    {
        // Arrange
        var mockFind = new Mock<IFindUserService<AppUser>>();
        mockFind.Setup(f => f.FindUserWithTeamDetailsAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
            .ReturnsAsync((AppUser?)null);

        var mockSignup = new Mock<IIdCustomerRegistrationService>();

        OAuthInfo? capturedOAuth = null;
        mockSignup.Setup(s => s.RegisterOAuthAsync(
            It.IsAny<EmailAddress>(),
            It.IsAny<UsernameNullable>(),
            It.IsAny<PhoneNullable>(),
            It.IsAny<FirstNameNullable>(),
            It.IsAny<LastNameNullable>(),
            It.IsAny<TeamPositionNullable>(),
            It.IsAny<OAuthInfo>(),
            It.IsAny<Guid?>(),
            It.IsAny<CancellationToken>()))
            .Callback<EmailAddress,
                UsernameNullable,
                PhoneNullable,
                FirstNameNullable,
                LastNameNullable,
                TeamPositionNullable,
                OAuthInfo,
                Guid?,
                CancellationToken>((ea, un, ph, fn, ln, tp, oi, sp, ct) => capturedOAuth = oi)
            .ReturnsAsync(GenResult<AppUser>.Success(AppUserDataFactory.AnyUser));

        var svc = new FindOrCreateService<AppUser>(mockFind.Object, mockSignup.Object);

        var profile = new AmazonUserProfile { Email = "me@example.com", Name = "Given", UserId = "uid" };
        var dto = new AmazonSignInDto { Email = null, SubscriptionPlanId = null };

        // Act
        var res = await svc.FindOrCreateUserAsync(profile, dto, CancellationToken.None);

        // Assert
        res.Succeeded.ShouldBeTrue();
        mockSignup.Verify(s => s.RegisterOAuthAsync(It.IsAny<EmailAddress>(),
            It.IsAny<UsernameNullable>(),
            It.IsAny<PhoneNullable>(),
            It.IsAny<FirstNameNullable>(),
            It.IsAny<LastNameNullable>(),
            It.IsAny<TeamPositionNullable>(),
            It.IsAny<OAuthInfo>(),
            It.IsAny<Guid?>(),
            It.IsAny<CancellationToken>()), Times.Once);

        capturedOAuth.ShouldNotBeNull();
        capturedOAuth!.Provider.ShouldBe(OAuthProvider.Amazon);
        capturedOAuth.Issuer.ShouldNotBeNull();
    }

}//Cls
