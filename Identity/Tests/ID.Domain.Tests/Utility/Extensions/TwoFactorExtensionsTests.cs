using ID.Domain.Models;
using ID.Domain.Utility.Extensions;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Utility.Extensions;

public class TwoFactorExtensionsTests
{
    [Fact]
    public void CanChangeToProvider_ShouldReturnFailure_WhenSmsProviderAndNoPhoneNumber()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser = AppUserDataFactory.Create(teamId: teamId, phoneNumber: "");
        var newProvider = TwoFactorProvider.Sms;

        // Act
        var result = appUser.CanChangeToProvider(newProvider);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.TwoFactor.NO_PHONE_FOR_TWO_FACTOR(appUser));
    }

    //------------------------------------//

    [Fact]
    public void CanChangeToProvider_ShouldReturnFailure_WhenEmailProviderAndNoEmail()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser = AppUserDataFactory.Create(teamId: teamId, email: "");
        var newProvider = TwoFactorProvider.Email;

        // Act
        var result = appUser.CanChangeToProvider(newProvider);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.TwoFactor.NO_EMAIL_FOR_TWO_FACTOR(appUser));
    }

    //------------------------------------//

    [Fact]
    public void CanChangeToProvider_ShouldReturnSuccess_WhenSmsProviderAndPhoneNumberExists()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser = AppUserDataFactory.Create(teamId: teamId, phoneNumber: "1234567890");
        var newProvider = TwoFactorProvider.Sms;

        // Act
        var result = appUser.CanChangeToProvider(newProvider);

        // Assert
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void CanChangeToProvider_ShouldReturnSuccess_WhenEmailProviderAndEmailExists()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser = AppUserDataFactory.Create(teamId: teamId, email: "test@example.com");
        var newProvider = TwoFactorProvider.Email;

        // Act
        var result = appUser.CanChangeToProvider(newProvider);

        // Assert
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls
