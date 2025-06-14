using ClArch.ValueObjects;
using Google.Apis.Auth;
using ID.Application.AppAbs.ApplicationServices.TwoFactor;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.Customers.Abstractions;
using ID.Application.JWT;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.OAuth;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.OAuth.Google.Auth.Abs;
using ID.OAuth.Google.Data;
using ID.OAuth.Google.Features.GoogleCookieSignUp;
using ID.OAuth.Google.Features.GoogleSignIn;
using static MyResults.BasicResult;

namespace ID.OAuth.Google.Tests.Handler;

public class GoogleSignInHandlerTests
{
    private readonly Mock<IJwtPackageProvider> _mockPackageProvider;
    private readonly Mock<IGoogleTokenVerifier> _mockVerifier;
    private readonly Mock<IIdCustomerRegistrationService> _mockSignupService;
    private readonly Mock<IFindUserService<AppUser>> _mockFindUserService;
    private readonly Mock<ITwoFactorVerificationService<AppUser>> _mockTwoFactorService;
    private readonly Mock<ITwoFactorMsgService> _mockTwoFactorMsgService;
    private readonly GoogleSignInHandler _handler;

    //------------------------------------//

    public GoogleSignInHandlerTests()
    {
        _mockPackageProvider = new Mock<IJwtPackageProvider>();
        _mockVerifier = new Mock<IGoogleTokenVerifier>();
        _mockSignupService = new Mock<IIdCustomerRegistrationService>();
        _mockFindUserService = new Mock<IFindUserService<AppUser>>();
        _mockTwoFactorService = new Mock<ITwoFactorVerificationService<AppUser>>();
        _mockTwoFactorMsgService = new Mock<ITwoFactorMsgService>();

        _handler = new GoogleSignInHandler(
            _mockFindUserService.Object,
            _mockPackageProvider.Object,
            _mockVerifier.Object,
            _mockSignupService.Object,
            _mockTwoFactorService.Object,
            _mockTwoFactorMsgService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Token_Verification_Fails()
    {
        // Arrange
        var dto = GoogleSignInDtoFactory.Create();
        var command = new GoogleSignInCmd(dto);
        var verificationError = "Invalid token signature";

        _mockVerifier.Setup(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.UnauthorizedResult(verificationError));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.Unauthorized);
        result.Info.ShouldBe(verificationError);

        _mockVerifier.Verify(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()), Times.Once);
        _mockFindUserService.Verify(x => x.FindUserWithTeamDetailsAsync(It.IsAny<string>(), null, null), Times.Never);
    }

    //------------------------------------//   

    [Fact]
    public async Task Handle_Should_Return_Success_When_Existing_User_Signs_In_Without_TwoFactor()
    {
        // Arrange
        var dto = GoogleSignInDtoFactory.Create();
        var command = new GoogleSignInCmd(dto);
        var googlePayload = CreateMockGooglePayload("existing@example.com");
        var existingUser = AppUserDataFactory.Create(email: googlePayload.Email);
        var jwtPackage = JwtPackageDataFactory.Create();

        _mockVerifier.Setup(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Success(googlePayload));

        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(googlePayload.Email, null, null))
            .ReturnsAsync(existingUser);

        _mockTwoFactorService.Setup(x => x.IsTwoFactorEnabledAsync(existingUser))
            .ReturnsAsync(false);

        _mockPackageProvider.Setup(x => x.CreateJwtPackageAsync(
                existingUser,
                existingUser.Team!,
                false,
                dto.DeviceId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);

        _mockVerifier.Verify(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()), Times.Once);
        _mockFindUserService.Verify(x => x.FindUserWithTeamDetailsAsync(googlePayload.Email, null, null), Times.Once);
        _mockTwoFactorService.Verify(x => x.IsTwoFactorEnabledAsync(existingUser), Times.Once);
        _mockSignupService.Verify(x => x.RegisterOAuthAsync(
            It.IsAny<EmailAddress>(),
            It.IsAny<UsernameNullable>(),
            It.IsAny<PhoneNullable>(),
            It.IsAny<FirstNameNullable>(),
            It.IsAny<LastNameNullable>(),
            It.IsAny<TeamPositionNullable>(),
            It.IsAny<OAuthInfo>(),
            It.IsAny<Guid?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//   
    // 
     [Fact]
    public async Task Handle_Should_Create_New_User_When_User_Does_Not_Exist()
    {
        // Arrange
        var dto = GoogleSignInDtoFactory.Create();
        var command = new GoogleSignInCmd(dto);
        var googlePayload = CreateMockGooglePayload(
            email: "newuser@example.com",
            givenName: "John",
            familyName: "Doe",
            picture: "https://example.com/photo.jpg",
            emailVerified: true);
        var newUser = AppUserDataFactory.Create(email: googlePayload.Email);
        var jwtPackage = JwtPackageDataFactory.Create();

        _mockVerifier.Setup(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Success(googlePayload));

        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(googlePayload.Email, null, null))
            .ReturnsAsync((AppUser?)null);

        _mockSignupService.Setup(x => x.RegisterOAuthAsync(
                It.IsAny<EmailAddress>(),
                It.IsAny<UsernameNullable>(),
                It.IsAny<PhoneNullable>(),
                It.IsAny<FirstNameNullable>(),
                It.IsAny<LastNameNullable>(),
                It.IsAny<TeamPositionNullable>(),
                It.IsAny<OAuthInfo>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(newUser));

        _mockTwoFactorService.Setup(x => x.IsTwoFactorEnabledAsync(newUser))
            .ReturnsAsync(false);

        _mockPackageProvider.Setup(x => x.CreateJwtPackageAsync(
                newUser,
                newUser.Team!,
                false,
                dto.DeviceId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);

        _mockVerifier.Verify(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()), Times.Once);
        _mockFindUserService.Verify(x => x.FindUserWithTeamDetailsAsync(googlePayload.Email, null, null), Times.Once);
        _mockTwoFactorService.Verify(x => x.IsTwoFactorEnabledAsync(newUser), Times.Once);
        _mockSignupService.Verify(x => x.RegisterOAuthAsync(
            It.Is<EmailAddress>(e => e.Value == googlePayload.Email),
            It.Is<UsernameNullable>(u => u.Value == googlePayload.Email),
            It.Is<PhoneNullable>(p => p.Value == null),
            It.Is<FirstNameNullable>(f => f.Value == googlePayload.GivenName),
            It.Is<LastNameNullable>(l => l.Value == googlePayload.FamilyName),
            It.IsAny<TeamPositionNullable>(),
            It.IsAny<OAuthInfo>(),
            dto.SubscriptionId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Return_Failure_When_User_Registration_Fails()
    {
        // Arrange
        var dto = GoogleSignInDtoFactory.Create();
        var command = new GoogleSignInCmd(dto);
        var googlePayload = CreateMockGooglePayload("newuser@example.com");
        var registrationError = "Email already exists in different context";

        _mockVerifier.Setup(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Success(googlePayload));

        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(googlePayload.Email, null, null))
            .ReturnsAsync((AppUser?)null);

        _mockSignupService.Setup(x => x.RegisterOAuthAsync(
                It.IsAny<EmailAddress>(),
                It.IsAny<UsernameNullable>(),
                It.IsAny<PhoneNullable>(),
                It.IsAny<FirstNameNullable>(),
                It.IsAny<LastNameNullable>(),
                It.IsAny<TeamPositionNullable>(),
                It.IsAny<OAuthInfo>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.BadRequestResult(registrationError));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.BadRequest);
        result.Info.ShouldBe(registrationError);

        _mockVerifier.Verify(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()), Times.Once);
        _mockFindUserService.Verify(x => x.FindUserWithTeamDetailsAsync(googlePayload.Email, null, null), Times.Once);
        _mockSignupService.Verify(x => x.RegisterOAuthAsync(
            It.IsAny<EmailAddress>(),
            It.IsAny<UsernameNullable>(),
            It.IsAny<PhoneNullable>(),
            It.IsAny<FirstNameNullable>(),
            It.IsAny<LastNameNullable>(),
            It.IsAny<TeamPositionNullable>(),
            It.IsAny<OAuthInfo>(),
            It.IsAny<Guid?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Pass_TwoFactorVerified_False_To_JwtPackageProvider()
    {
        // Arrange
        var dto = GoogleSignInDtoFactory.Create();
        var command = new GoogleSignInCmd(dto);
        var googlePayload = CreateMockGooglePayload("test@example.com");
        var existingUser = AppUserDataFactory.Create(email: googlePayload.Email);
        var jwtPackage = JwtPackageDataFactory.Create();

        _mockVerifier.Setup(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Success(googlePayload));

        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(googlePayload.Email, null, null))
            .ReturnsAsync(existingUser);

        _mockPackageProvider.Setup(x => x.CreateJwtPackageAsync(
                It.IsAny<AppUser>(),
                It.IsAny<Team>(),
                It.IsAny<bool>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();

        // Verify that twoFactorVerified is set to false (OAuth doesn't complete 2FA by itself)
        _mockPackageProvider.Verify(x => x.CreateJwtPackageAsync(
            existingUser,
            existingUser.Team!,
            false, // ✅ Should be false - OAuth doesn't satisfy 2FA requirements
            dto.DeviceId,
            It.IsAny<CancellationToken>()), Times.Once);    }

    //------------------------------------//    [Fact]
    public async Task Handle_Should_Send_TwoFactor_When_User_Has_TwoFactor_Enabled()
    {
        // Arrange
        var dto = GoogleSignInDtoFactory.Create();
        var command = new GoogleSignInCmd(dto);
        var googlePayload = CreateMockGooglePayload("twofactor@example.com");
        var existingUser = AppUserDataFactory.Create(email: googlePayload.Email);
        var mfaResultData = new MfaResultData(TwoFactorProvider.Sms, "Sent to +1234567890");
        var jwtPackage = JwtPackageDataFactory.Create();

        _mockVerifier.Setup(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Success(googlePayload));

        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(googlePayload.Email, null, null))
            .ReturnsAsync(existingUser);

        _mockTwoFactorService.Setup(x => x.IsTwoFactorEnabledAsync(existingUser))
            .ReturnsAsync(true);

        _mockTwoFactorMsgService.Setup(x => x.SendOTPFor2FactorAuth(existingUser.Team!, existingUser, It.IsAny<TwoFactorProvider>()))
            .ReturnsAsync(GenResult<MfaResultData>.Success(mfaResultData));

        _mockPackageProvider.Setup(x => x.CreateJwtPackageWithTwoFactorRequiredAsync(
                existingUser,
                existingUser.Team!,
                mfaResultData.TwoFactorProvider,
                mfaResultData.ExtraInfo,
                dto.DeviceId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(jwtPackage);

        _mockVerifier.Verify(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()), Times.Once);
        _mockFindUserService.Verify(x => x.FindUserWithTeamDetailsAsync(googlePayload.Email, null, null), Times.Once);
        _mockTwoFactorService.Verify(x => x.IsTwoFactorEnabledAsync(existingUser), Times.Once);
        _mockTwoFactorMsgService.Verify(x => x.SendOTPFor2FactorAuth(existingUser.Team!, existingUser, It.IsAny<TwoFactorProvider>()), Times.Once);
        _mockPackageProvider.Verify(x => x.CreateJwtPackageWithTwoFactorRequiredAsync(
            existingUser,
            existingUser.Team!,
            mfaResultData.TwoFactorProvider,
            mfaResultData.ExtraInfo,
            dto.DeviceId,
            It.IsAny<CancellationToken>()), Times.Once);

        // Verify standard JWT package creation was NOT called
        _mockPackageProvider.Verify(x => x.CreateJwtPackageAsync(
            It.IsAny<AppUser>(),
            It.IsAny<Team>(),
            It.IsAny<bool>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Return_Failure_When_TwoFactor_Message_Sending_Fails()
    {
        // Arrange
        var dto = GoogleSignInDtoFactory.Create();
        var command = new GoogleSignInCmd(dto);
        var googlePayload = CreateMockGooglePayload("twofactor@example.com");
        var teamId = Guid.NewGuid();
        var team = TeamDataFactory.Create(id: teamId, name: "Test Team");
        var existingUser = AppUserDataFactory.Create(teamId: teamId, team: team, email: googlePayload.Email);
        var twoFactorError = "Failed to send SMS - invalid phone number";

        _mockVerifier.Setup(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Success(googlePayload));

        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(googlePayload.Email, null, null))
            .ReturnsAsync(existingUser);

        _mockTwoFactorService.Setup(x => x.IsTwoFactorEnabledAsync(existingUser))
            .ReturnsAsync(true);

        _mockTwoFactorMsgService.Setup(x => x.SendOTPFor2FactorAuth(existingUser.Team!, existingUser, It.IsAny<TwoFactorProvider?>()))
            .ReturnsAsync(GenResult<MfaResultData>.BadRequestResult(twoFactorError));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Status.ShouldBe(ResultStatus.Failure);
        result.Info.ShouldBe(twoFactorError);

        _mockVerifier.Verify(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()), Times.Once);
        _mockFindUserService.Verify(x => x.FindUserWithTeamDetailsAsync(googlePayload.Email, null, null), Times.Once);
        _mockTwoFactorService.Verify(x => x.IsTwoFactorEnabledAsync(existingUser), Times.Once);
        _mockTwoFactorMsgService.Verify(x => x.SendOTPFor2FactorAuth(existingUser.Team!, existingUser, It.IsAny<TwoFactorProvider?>()), Times.Once);
        
        // Verify no JWT packages were created when 2FA sending fails
        _mockPackageProvider.Verify(x => x.CreateJwtPackageAsync(
            It.IsAny<AppUser>(),
            It.IsAny<Team>(),
            It.IsAny<bool>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Never);
        _mockPackageProvider.Verify(x => x.CreateJwtPackageWithTwoFactorRequiredAsync(
            It.IsAny<AppUser>(),
            It.IsAny<Team>(),
            It.IsAny<TwoFactorProvider>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Pass_TwoFactorVerified_False_To_JwtPackageProvider_When_No_TwoFactor()
    {
        // Arrange
        var dto = GoogleSignInDtoFactory.Create();
        var command = new GoogleSignInCmd(dto);
        var googlePayload = CreateMockGooglePayload("test@example.com");
        var existingUser = AppUserDataFactory.Create(email: googlePayload.Email);
        var jwtPackage = JwtPackageDataFactory.Create();

        _mockVerifier.Setup(x => x.VerifyTokenAsync(dto.IdToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<GoogleVerifiedPayload>.Success(googlePayload));

        _mockFindUserService.Setup(x => x.FindUserWithTeamDetailsAsync(googlePayload.Email, null, null))
            .ReturnsAsync(existingUser);

        _mockTwoFactorService.Setup(x => x.IsTwoFactorEnabledAsync(existingUser))
            .ReturnsAsync(false);

        _mockPackageProvider.Setup(x => x.CreateJwtPackageAsync(
                It.IsAny<AppUser>(),
                It.IsAny<Team>(),
                It.IsAny<bool>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(jwtPackage);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();

        // Verify that twoFactorVerified is set to false (OAuth doesn't complete 2FA by itself)
        _mockPackageProvider.Verify(x => x.CreateJwtPackageAsync(
            existingUser,
            existingUser.Team!,
            false, // ✅ Should be false - OAuth doesn't satisfy 2FA requirements
            dto.DeviceId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//
    // Helper Methods
    //------------------------------------//

    private static GoogleVerifiedPayload CreateMockGooglePayload(
        string email = "test@example.com",
        string? givenName = "Test",
        string? familyName = "User", 
        string? picture = "https://lh3.googleusercontent.com/default",
        bool emailVerified = true,
        string issuer = "https://accounts.google.com")
    {
        // Create a mock Google JWT payload
        var mockPayload = new GoogleJsonWebSignature.Payload
        {
            Email = email,
            GivenName = givenName,
            FamilyName = familyName,
            Picture = picture,
            EmailVerified = emailVerified,
            Issuer = issuer,
            Subject = Guid.NewGuid().ToString(),
            //Audience = ["test-client-id"]
        };

        return new GoogleVerifiedPayload(mockPayload);
    }

    //------------------------------------//

}//Cls
