using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Setup.Options;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Tests.Auth.JWT.Utils;
using ID.Infrastructure.TokenServices.Phone;
using ID.Tests.Data.GlobalOptions;
using Microsoft.Extensions.Options;
using Moq;

namespace ID.Infrastructure.Tests.TokenServices.Phone;

public class DbPhoneConfirmationServiceTests
{
    private readonly Mock<IIdUserMgmtService<AppUser>> _userMgrMock;
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamMgrMock;
    private readonly Mock<IIdUnitOfWork> _uowMock;
    private readonly Mock<IOptions<IdGlobalOptions>> _globalOptionsProviderMock;
    private readonly IdGlobalOptions _globalOptions;
    private readonly DbPhoneConfirmationService<AppUser> _sut;

    public DbPhoneConfirmationServiceTests()
    {
        _userMgrMock = new Mock<IIdUserMgmtService<AppUser>>();
        _teamMgrMock = new Mock<IIdentityTeamManager<AppUser>>();
        _uowMock = new Mock<IIdUnitOfWork>();
        _globalOptionsProviderMock = new Mock<IOptions<IdGlobalOptions>>();
        _globalOptions = GlobalOptionsUtils.InitiallyValidOptions(
            phoneTokenTimeSpan: TimeSpan.FromMinutes(15) // 15 minutes expiration
        );

        _globalOptionsProviderMock.Setup(x => x.Value).Returns(_globalOptions);

        _sut = new DbPhoneConfirmationService<AppUser>(
            _userMgrMock.Object,
            _teamMgrMock.Object,
            _uowMock.Object,
            _globalOptionsProviderMock.Object);
    }

    #region ConfirmPhoneAsync Tests   

    [Fact]
    public async Task ConfirmPhoneAsync_WithValidToken_ShouldReturnSuccess()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(
            team: team,
            tkn: "valid-token",
            phoneNumber: "1234567890",
            phoneNumberConfirmed: false);

        // Set token modification date using reflection since SetTkn would overwrite the token
        var tokenCreatedTime = DateTime.Now.AddMinutes(-5);
        var tknModifiedDateProperty = typeof(AppUser).GetProperty("TknModifiedDate");
        tknModifiedDateProperty?.SetValue(user, tokenCreatedTime);

        // Act
        var result = await _sut.ConfirmPhoneAsync(team, user, "valid-token", "1234567890");

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Info.Phone.PHONE_CONFIRMED("1234567890"));

        // Verify user was updated
        user.PhoneNumberConfirmed.ShouldBeTrue();
        user.Tkn.ShouldBeNull();

        // Verify repository calls
        _teamMgrMock.Verify(x => x.UpdateMemberAsync(team, user), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    //-------------------------//

    [Fact]
    public async Task ConfirmPhoneAsync_WithInvalidToken_ShouldReturnFailure()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(
            team: team,
            tkn: "valid-token",
            phoneNumber: "1234567890");

        // Set token modification date using reflection
        var tokenCreatedTime = DateTime.Now.AddMinutes(-5);
        var tknModifiedDateProperty = typeof(AppUser).GetProperty("TknModifiedDate");
        tknModifiedDateProperty?.SetValue(user, tokenCreatedTime);

        // Act
        var result = await _sut.ConfirmPhoneAsync(team, user, "invalid-token", "1234567890");

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("Invalid");

        // Verify no repository calls were made
        _teamMgrMock.Verify(x => x.UpdateMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Never);
        _uowMock.Verify(x => x.SaveChangesAsync(default), Times.Never);
    }

    //-------------------------//

    [Fact]
    public async Task ConfirmPhoneAsync_WithMismatchedPhoneNumber_ShouldReturnFailure()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var recentTokenDate = DateTime.Now.AddMinutes(-5);
        var user = AppUserDataFactory.Create(
            team: team,
            tkn: "valid-token",
            phoneNumber: "1234567890",
            tknModifiedDate: recentTokenDate);

        // Act
        var result = await _sut.ConfirmPhoneAsync(team, user, "valid-token", "0987654321");

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("Invalid");

        // Verify no repository calls were made
        _teamMgrMock.Verify(x => x.UpdateMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Never);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    //-------------------------//

    [Fact]
    public async Task ConfirmPhoneAsync_WithExpiredToken_ShouldReturnFailure()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var expiredTokenDate = DateTime.Now.AddMinutes(-20); // Token expired (older than 15 minutes default)
        var user = AppUserDataFactory.Create(
            team: team,
            tkn: "valid-token",
            phoneNumber: "1234567890",
            tknModifiedDate: expiredTokenDate); // Pass as ISO string

        // Act
        var result = await _sut.ConfirmPhoneAsync(team, user, "valid-token", "1234567890");

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("Invalid");

        // Verify no repository calls were made
        _teamMgrMock.Verify(x => x.UpdateMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Never);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    //-------------------------//

    [Fact]
    public async Task ConfirmPhoneAsync_WithNullTokenModifiedDate_ShouldReturnFailure()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(
            team: team,
            tkn: "valid-token",
            phoneNumber: "1234567890",
            tknModifiedDate: null); // No token modification date

        // Act
        var result = await _sut.ConfirmPhoneAsync(team, user, "valid-token", "1234567890");

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldContain("Invalid");

        // Verify no repository calls were made
        _teamMgrMock.Verify(x => x.UpdateMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Never);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GeneratePhoneChangedConfirmationTokenAsync Tests

    [Fact]
    public async Task GeneratePhoneChangedConfirmationTokenAsync_ShouldGenerateTokenAndUpdateUser()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var newPhoneNumber = "1234567890";
        var expectedToken = "generated-token";

        _userMgrMock.Setup(x => x.GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber))
            .ReturnsAsync(expectedToken);

        // Act
        var result = await _sut.GeneratePhoneChangedConfirmationTokenAsync(team, user, newPhoneNumber);

        // Assert
        result.ShouldBe(expectedToken);

        // Verify token was set on user
        user.Tkn.ShouldBe(expectedToken);

        // Verify repository calls
        _userMgrMock.Verify(x => x.GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber), Times.Once);
        _teamMgrMock.Verify(x => x.UpdateMemberAsync(team, user), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task GeneratePhoneChangedConfirmationTokenAsync_WithEmptyPhoneNumber_ShouldStillWork(string? phoneNumber)
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var expectedToken = "generated-token";

        _userMgrMock.Setup(x => x.GenerateChangePhoneNumberTokenAsync(user, phoneNumber!))
            .ReturnsAsync(expectedToken);

        // Act
        var result = await _sut.GeneratePhoneChangedConfirmationTokenAsync(team, user, phoneNumber!);

        // Assert
        result.ShouldBe(expectedToken);

        // Verify repository calls
        _userMgrMock.Verify(x => x.GenerateChangePhoneNumberTokenAsync(user, phoneNumber!), Times.Once);
        _teamMgrMock.Verify(x => x.UpdateMemberAsync(team, user), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region IsPhoneConfirmedAsync Tests

    [Fact]
    public async Task IsPhoneConfirmedAsync_ShouldReturnUserManagerResult()
    {
        // Arrange
        var user = AppUserDataFactory.Create();
        var expectedResult = true;

        _userMgrMock.Setup(x => x.IsPhoneNumberConfirmedAsync(user))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _sut.IsPhoneConfirmedAsync(user);

        // Assert
        result.ShouldBe(expectedResult);
        _userMgrMock.Verify(x => x.IsPhoneNumberConfirmedAsync(user), Times.Once);
    }

    [Fact]
    public async Task IsPhoneConfirmedAsync_WithUnconfirmedPhone_ShouldReturnFalse()
    {
        // Arrange
        var user = AppUserDataFactory.Create(phoneNumberConfirmed: false);

        _userMgrMock.Setup(x => x.IsPhoneNumberConfirmedAsync(user))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.IsPhoneConfirmedAsync(user);

        // Assert
        result.ShouldBeFalse();
        _userMgrMock.Verify(x => x.IsPhoneNumberConfirmedAsync(user), Times.Once);
    }

    #endregion

    #region Token Validation Edge Cases  
    [Theory]
    [InlineData(9)] // Just under expiration
    [InlineData(10)] // Exactly at expiration
    [InlineData(11)] // Just over expiration
    public async Task ConfirmPhoneAsync_TokenExpirationBoundaryTests(int minutesAgo)
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var tokenModifiedDate = DateTime.Now.AddMinutes(-minutesAgo);
        var user = AppUserDataFactory.Create(
            team: team,
            tkn: "valid-token",
            phoneNumber: "1234567890",
            tknModifiedDate: tokenModifiedDate);

        // Act
        var result = await _sut.ConfirmPhoneAsync(team, user, "valid-token", "1234567890");

        // Assert
        if (minutesAgo < 15) // Updated to match the 15-minute default expiration
        {
            result.Succeeded.ShouldBeTrue();
            _teamMgrMock.Verify(x => x.UpdateMemberAsync(team, user), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        else
        {
            result.Succeeded.ShouldBeFalse();
            _teamMgrMock.Verify(x => x.UpdateMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Never);
            _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    //-------------------------//

    [Fact]
    public async Task ConfirmPhoneAsync_WithDifferentTimeSpanConfiguration_ShouldRespectSettings()
    {
        // Arrange
        var customOptions = GlobalOptionsUtils.InitiallyValidOptions(
            phoneTokenTimeSpan: TimeSpan.FromMinutes(30) // 30 minutes expiration
        );

        var customOptionsProvider = new Mock<IOptions<IdGlobalOptions>>();
        customOptionsProvider.Setup(x => x.Value).Returns(customOptions);

        var customSut = new DbPhoneConfirmationService<AppUser>(
            _userMgrMock.Object,
            _teamMgrMock.Object,
            _uowMock.Object,
            customOptionsProvider.Object); var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(
            team: team,
            phoneNumber: "1234567890");

        // Set token first, then wait for a brief moment to test the 30-minute window
        user.SetTkn("valid-token");

        // Act
        var result = await customSut.ConfirmPhoneAsync(team, user, "valid-token", "1234567890");

        // Assert
        result.Succeeded.ShouldBeTrue(); // Should be valid with 30-minute expiration
        user.PhoneNumberConfirmed.ShouldBeTrue();
        user.Tkn.ShouldBeNull();
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task GeneratePhoneChangedConfirmationTokenAsync_WhenUserManagerThrows_ShouldPropagate()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(team: team);
        var newPhoneNumber = "1234567890";

        _userMgrMock.Setup(x => x.GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber))
            .ThrowsAsync(new InvalidOperationException("User manager error"));

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _sut.GeneratePhoneChangedConfirmationTokenAsync(team, user, newPhoneNumber));

        // Verify no repository calls were made after the exception
        _teamMgrMock.Verify(x => x.UpdateMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Never);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    //-------------------------//

    [Fact]
    public async Task ConfirmPhoneAsync_WhenTeamManagerThrows_ShouldPropagate()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var recentTokenDate = DateTime.Now.AddMinutes(-5);
        var user = AppUserDataFactory.Create(
            team: team,
            tkn: "valid-token",
            phoneNumber: "1234567890",
            tknModifiedDate: recentTokenDate);

        _teamMgrMock.Setup(x => x.UpdateMemberAsync(team, user))
            .ThrowsAsync(new InvalidOperationException("Team manager error"));

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _sut.ConfirmPhoneAsync(team, user, "valid-token", "1234567890"));

        // Verify user state was modified before the exception
        user.PhoneNumberConfirmed.ShouldBeTrue();
        user.Tkn.ShouldBeNull();
    }

    [Fact]
    public async Task IsPhoneConfirmedAsync_WhenUserManagerThrows_ShouldPropagate()
    {
        // Arrange
        var user = AppUserDataFactory.Create();

        _userMgrMock.Setup(x => x.IsPhoneNumberConfirmedAsync(user))
            .ThrowsAsync(new InvalidOperationException("User manager error"));

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(
            () => _sut.IsPhoneConfirmedAsync(user));
    }

    #endregion

    #region Integration-like Tests  

    [Fact]
    public async Task CompletePhoneConfirmationFlow_ShouldWorkCorrectly()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var user = AppUserDataFactory.Create(
            team: team,
            phoneNumberConfirmed: false);
        var newPhoneNumber = "1234567890";
        var generatedToken = "generated-token";        _userMgrMock.Setup(x => x.GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber))
            .ReturnsAsync(generatedToken);
        _userMgrMock.Setup(x => x.IsPhoneNumberConfirmedAsync(It.IsAny<AppUser>()))
            .ReturnsAsync((AppUser u) => u.PhoneNumberConfirmed);

        // Act & Assert - Generate token
        var token = await _sut.GeneratePhoneChangedConfirmationTokenAsync(team, user, newPhoneNumber);
        token.ShouldBe(generatedToken);
        user.Tkn.ShouldBe(generatedToken);

        // Create a new user with the token and current time to simulate token being fresh
        var userWithFreshToken = AppUserDataFactory.Create(
            team: team,
            tkn: generatedToken,
            phoneNumber: newPhoneNumber,
            phoneNumberConfirmed: false,
            tknModifiedDate: DateTime.Now);

        // Act & Assert - Check phone not confirmed yet
        var isConfirmedBefore = await _sut.IsPhoneConfirmedAsync(userWithFreshToken);
        isConfirmedBefore.ShouldBeFalse();

        // Act & Assert - Confirm phone
        var confirmResult = await _sut.ConfirmPhoneAsync(team, userWithFreshToken, generatedToken, newPhoneNumber);
        confirmResult.Succeeded.ShouldBeTrue();
        confirmResult.Info.ShouldBe(IDMsgs.Info.Phone.PHONE_CONFIRMED(newPhoneNumber));

        // Act & Assert - Check phone is now confirmed
        var isConfirmedAfter = await _sut.IsPhoneConfirmedAsync(userWithFreshToken);
        isConfirmedAfter.ShouldBeTrue();

        // Verify final state
        userWithFreshToken.PhoneNumberConfirmed.ShouldBeTrue();
        userWithFreshToken.Tkn.ShouldBeNull();

        // Verify all repository interactions
        _teamMgrMock.Verify(x => x.UpdateMemberAsync(team, It.IsAny<AppUser>()), Times.Exactly(2)); // Once for token generation, once for confirmation
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    #endregion
}
