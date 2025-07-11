using ClArch.ValueObjects;
using Google.Apis.Auth;
using ID.Application.AppAbs.ApplicationServices.User;
using ID.Application.Customers.Abstractions;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.OAuth;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.OAuth.Google.Data;
using ID.OAuth.Google.Features.SignIn;
using ID.OAuth.Google.Services.Imps;

namespace ID.OAuth.Google.Tests.Services.Imps;

public class FindOrCreateServiceTests
{
    private readonly Mock<IFindUserService<AppUser>> _mockFindUserService;
    private readonly Mock<IIdCustomerRegistrationService> _mockSignupService;
    private readonly FindOrCreateService<AppUser> _findOrCreateService;

    public FindOrCreateServiceTests()
    {
        _mockFindUserService = new Mock<IFindUserService<AppUser>>();
        _mockSignupService = new Mock<IIdCustomerRegistrationService>();
        _findOrCreateService = new FindOrCreateService<AppUser>(_mockFindUserService.Object, _mockSignupService.Object);
    }

    [Fact]
    public async Task FindOrCreateUserAsync_WhenUserExists_ShouldReturnExistingUser()
    {
        // Arrange
        var email = "test@example.com";
        var existingUser = AppUserDataFactory.Create(email: email);
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload())
        {
            Email = email,
            // Other properties aren't needed for this test
        };
        var dto = new GoogleSignInDto();

        _mockFindUserService
            .Setup(s => s.FindUserWithTeamDetailsAsync(email, null, null))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _findOrCreateService.FindOrCreateUserAsync(payload, dto, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(existingUser);

        // Verify that registration was not attempted
        _mockSignupService.Verify(
            s => s.RegisterOAuthAsync(
                It.IsAny<EmailAddress>(),
                It.IsAny<UsernameNullable>(),
                It.IsAny<PhoneNullable>(),
                It.IsAny<FirstNameNullable>(),
                It.IsAny<LastNameNullable>(),
                It.IsAny<TeamPositionNullable>(),
                It.IsAny<OAuthInfo>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task FindOrCreateUserAsync_WhenUserDoesNotExist_ShouldRegisterNewUser()
    {
        // Arrange
        var email = "newuser@example.com";
        var givenName = "John";
        var familyName = "Doe";
        var issuer = "Google";
        var picture = "https://example.com/profile.jpg";
        var emailVerified = true;
        var subscriptionId = Guid.NewGuid();
        
        var newUser = AppUserDataFactory.Create(email: email);

        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload())
        {
            Email = email,
            GivenName = givenName,
            FamilyName = familyName,
            Issuer = issuer,
            Picture = picture,
            EmailVerified = emailVerified
        };
        
        var dto = new GoogleSignInDto
        {
            SubscriptionId = subscriptionId
        };

        _mockFindUserService
            .Setup(s => s.FindUserWithTeamDetailsAsync(email, null, null))
            .ReturnsAsync((AppUser)null!);

        _mockSignupService
            .Setup(s => s.RegisterOAuthAsync(
                It.Is<EmailAddress>(e => e.Value == email),
                It.Is<UsernameNullable>(u => u.Value == email),
                It.IsAny<PhoneNullable>(),
                It.Is<FirstNameNullable>(f => f.Value == givenName),
                It.Is<LastNameNullable>(l => l.Value == familyName),
                It.IsAny<TeamPositionNullable>(),
                It.Is<OAuthInfo>(o => 
                    o.Provider == OAuthProvider.Google && 
                    o.Issuer == issuer && 
                    o.ImageUrl == picture && 
                    o.EmailVerified.Value == emailVerified),
                It.Is<Guid?>(s => s == subscriptionId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(newUser));

        // Act
        var result = await _findOrCreateService.FindOrCreateUserAsync(payload, dto, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(newUser);

        // Verify registration was called with correct parameters
        _mockSignupService.Verify(
            s => s.RegisterOAuthAsync(
                It.Is<EmailAddress>(e => e.Value == email),
                It.Is<UsernameNullable>(u => u.Value == email),
                It.IsAny<PhoneNullable>(),
                It.Is<FirstNameNullable>(f => f.Value == givenName),
                It.Is<LastNameNullable>(l => l.Value == familyName),
                It.IsAny<TeamPositionNullable>(),
                It.Is<OAuthInfo>(o => o.Provider == OAuthProvider.Google),
                It.Is<Guid?>(s => s == subscriptionId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task FindOrCreateUserAsync_WhenRegistrationFails_ShouldReturnFailureResult()
    {
        // Arrange
        var email = "newuser@example.com";
        var errorMessage = "Registration failed";
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload()) { Email = email };
        var dto = new GoogleSignInDto();

        _mockFindUserService
            .Setup(s => s.FindUserWithTeamDetailsAsync(email, null, null))
            .ReturnsAsync((AppUser)null!);

        _mockSignupService
            .Setup(s => s.RegisterOAuthAsync(
                It.IsAny<EmailAddress>(),
                It.IsAny<UsernameNullable>(),
                It.IsAny<PhoneNullable>(),
                It.IsAny<FirstNameNullable>(),
                It.IsAny<LastNameNullable>(),
                It.IsAny<TeamPositionNullable>(),
                It.IsAny<OAuthInfo>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Failure(errorMessage));

        // Act
        var result = await _findOrCreateService.FindOrCreateUserAsync(payload, dto, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(errorMessage);
    }

    [Fact]
    public async Task FindOrCreateUserAsync_WithNullPayloadValues_ShouldHandleGracefully()
    {
        // Arrange
        var email = "minimal@example.com";
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload())
        {
            Email = email,
            GivenName = null!,
            FamilyName = null!,
            Issuer = null!,
            Picture = null!,
            EmailVerified = false
        };
        var dto = new GoogleSignInDto();
        var newUser = AppUserDataFactory.Create(email: email);

        _mockFindUserService
            .Setup(s => s.FindUserWithTeamDetailsAsync(email, null, null))
            .ReturnsAsync((AppUser)null!);

        _mockSignupService
            .Setup(s => s.RegisterOAuthAsync(
                It.Is<EmailAddress>(e => e.Value == email),
                It.Is<UsernameNullable>(u => u.Value == email),
                It.IsAny<PhoneNullable>(),
                It.IsAny<FirstNameNullable>(),
                It.IsAny<LastNameNullable>(),
                It.IsAny<TeamPositionNullable>(),
                It.Is<OAuthInfo>(o => o.Provider == OAuthProvider.Google),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(newUser));

        // Act
        var result = await _findOrCreateService.FindOrCreateUserAsync(payload, dto, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task FindOrCreateUserAsync_WhenFindUserThrowsException_ShouldPropagateException()
    {
        // Arrange
        var email = "error@example.com";
        var expectedException = new InvalidOperationException("Database error");
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload()) { Email = email };
        var dto = new GoogleSignInDto();

        _mockFindUserService
            .Setup(s => s.FindUserWithTeamDetailsAsync(email, null, null))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _findOrCreateService.FindOrCreateUserAsync(payload, dto, CancellationToken.None)
        );

        exception.Message.ShouldBe(expectedException.Message);
    }

    [Fact]
    public async Task FindOrCreateUserAsync_WhenSignupServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var email = "error@example.com";
        var expectedException = new InvalidOperationException("Registration error");
        var payload = new GoogleVerifiedPayload(new GoogleJsonWebSignature.Payload()) { Email = email };
        var dto = new GoogleSignInDto();

        _mockFindUserService
            .Setup(s => s.FindUserWithTeamDetailsAsync(email, null, null))
            .ReturnsAsync((AppUser)null!);

        _mockSignupService
            .Setup(s => s.RegisterOAuthAsync(
                It.IsAny<EmailAddress>(),
                It.IsAny<UsernameNullable>(),
                It.IsAny<PhoneNullable>(),
                It.IsAny<FirstNameNullable>(),
                It.IsAny<LastNameNullable>(),
                It.IsAny<TeamPositionNullable>(),
                It.IsAny<OAuthInfo>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            async () => await _findOrCreateService.FindOrCreateUserAsync(payload, dto, CancellationToken.None)
        );

        exception.Message.ShouldBe(expectedException.Message);
    }

}