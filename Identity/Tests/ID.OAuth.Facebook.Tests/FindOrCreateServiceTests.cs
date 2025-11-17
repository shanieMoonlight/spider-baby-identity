namespace ID.OAuth.Facebook.Tests;

public class FindOrCreateServiceTests
{
    [Fact]
    public async Task FindOrCreateUserAsync_ReturnsExistingUser_WhenFoundByEmail()
    {
        // Arrange
        var existingUser = AppUserDataFactory.Create(email: "test@example.com");
        var mockFind = new Mock<IFindUserService<AppUser>>();
        mockFind.Setup(x => x.FindUserWithTeamDetailsAsync("test@example.com", It.IsAny<string?>(), It.IsAny<Guid?>())).ReturnsAsync(existingUser);

        var mockSignup = new Mock<IIdCustomerRegistrationService>();

        var service = new FindOrCreateService<AppUser>(mockFind.Object, mockSignup.Object);

        var profile = new FacebookUserProfile { Email = "test@example.com" };
        var dto = new FacebookSignInDto { Email = "test@example.com" };

        // Act
        var result = await service.FindOrCreateUserAsync(profile, dto, default);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value?.Email.ShouldBe("test@example.com");
        mockSignup.Verify(x => x.RegisterOAuthAsync(
                It.IsAny<ClArch.ValueObjects.EmailAddress>(),
                It.IsAny<ClArch.ValueObjects.UsernameNullable>(),
                It.IsAny<ClArch.ValueObjects.PhoneNullable>(),
                It.IsAny<ClArch.ValueObjects.FirstNameNullable>(),
                It.IsAny<ClArch.ValueObjects.LastNameNullable>(),
                It.IsAny<Domain.Entities.AppUsers.ValueObjects.TeamPositionNullable>(),
                It.IsAny<Domain.Entities.AppUsers.OAuth.OAuthInfo>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    //----------------------//

    [Fact]
    public async Task FindOrCreateUserAsync_CallsRegister_WhenUserNotFound()
    {
        // Arrange
        var mockFind = new Mock<IFindUserService<AppUser>>();
        mockFind.Setup(x => x.FindUserWithTeamDetailsAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
            .ReturnsAsync((AppUser)null!);

        var mockSignup = new Mock<IIdCustomerRegistrationService>();
        mockSignup.Setup(x => x.RegisterOAuthAsync(
            It.IsAny<ClArch.ValueObjects.EmailAddress>(),
            It.IsAny<ClArch.ValueObjects.UsernameNullable>(),
            It.IsAny<ClArch.ValueObjects.PhoneNullable>(),
            It.IsAny<ClArch.ValueObjects.FirstNameNullable>(),
            It.IsAny<ClArch.ValueObjects.LastNameNullable>(),
            It.IsAny<Domain.Entities.AppUsers.ValueObjects.TeamPositionNullable>(),
            It.IsAny<Domain.Entities.AppUsers.OAuth.OAuthInfo>(),
            It.IsAny<Guid?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenResult<AppUser>.Success(AppUserDataFactory.Create(email: "new@example.com")));

        var service = new FindOrCreateService<AppUser>(mockFind.Object, mockSignup.Object);

        var profile = new FacebookUserProfile { Email = "new@example.com", FirstName = "New", LastName = "User" };
        var dto = new FacebookSignInDto { Email = "new@example.com" };

        // Act
        var result = await service.FindOrCreateUserAsync(profile, dto, default);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value?.Email.ShouldBe("new@example.com");
        mockSignup.Verify(x => x.RegisterOAuthAsync(
                It.IsAny<ClArch.ValueObjects.EmailAddress>(),
                It.IsAny<ClArch.ValueObjects.UsernameNullable>(),
                It.IsAny<ClArch.ValueObjects.PhoneNullable>(),
                It.IsAny<ClArch.ValueObjects.FirstNameNullable>(),
                It.IsAny<ClArch.ValueObjects.LastNameNullable>(),
                It.IsAny<Domain.Entities.AppUsers.ValueObjects.TeamPositionNullable>(),
                It.IsAny<Domain.Entities.AppUsers.OAuth.OAuthInfo>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    //----------------------//

    [Fact]
    public async Task FindOrCreateUserAsync_Fails_WhenNoEmailProvided()
    {
        // Arrange
        var mockFind = new Mock<IFindUserService<AppUser>>();
        mockFind.Setup(x => x.FindUserWithTeamDetailsAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
            .ReturnsAsync((AppUser)null!);

        var mockSignup = new Mock<IIdCustomerRegistrationService>();

        var service = new FindOrCreateService<AppUser>(mockFind.Object, mockSignup.Object);

        var profile = new FacebookUserProfile { Email = null };
        var dto = new FacebookSignInDto { Email = null };

        // Act
        var result = await service.FindOrCreateUserAsync(profile, dto, default);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("Email is required");
        mockSignup.Verify(x => x.RegisterOAuthAsync(
                It.IsAny<ClArch.ValueObjects.EmailAddress>(),
                It.IsAny<ClArch.ValueObjects.UsernameNullable>(),
                It.IsAny<ClArch.ValueObjects.PhoneNullable>(),
                It.IsAny<ClArch.ValueObjects.FirstNameNullable>(),
                It.IsAny<ClArch.ValueObjects.LastNameNullable>(),
                It.IsAny<Domain.Entities.AppUsers.ValueObjects.TeamPositionNullable>(),
                It.IsAny<Domain.Entities.AppUsers.OAuth.OAuthInfo>(),
                It.IsAny<Guid?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

}//Cls
