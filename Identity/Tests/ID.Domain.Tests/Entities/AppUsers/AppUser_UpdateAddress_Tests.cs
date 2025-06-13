using ClArch.ValueObjects;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.Events;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Models;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.AppUsers;

public class AppUser_UpdateAddress_Tests
{
    //------------------------------------//

    [Fact]
    public void Create_ShouldInitializeAppUser()
    {
        // Arrange
        var user = AppUserDataFactory.AnyUser;

        // Assert
        Assert.NotNull(user);
        Assert.Equal("anyone@anywhere.com", user.Email);
        Assert.Equal("anyone", user.UserName);
        Assert.Equal("066 666 666 66", user.PhoneNumber);
        Assert.Equal("Clarke", user.FirstName);
        Assert.Equal("Kent", user.LastName);
        Assert.Equal(TeamDataFactory.AnyTeam.Id, user.TeamId);
    }

    //------------------------------------//

    [Fact]
    public void Update_ShouldModifyAppUser()
    {
        // Arrange
        var user = AppUserDataFactory.AnyUser;

        var newEmail = EmailAddress.Create("new@example.com");
        var newUsername = UsernameNullable.Create("newuser");
        var newPhone = PhoneNullable.Create("0987654321");
        var newFirstName = FirstNameNullable.Create("Jane");
        var newLastName = LastNameNullable.Create("Smith");
        var newProvider = TwoFactorProvider.Sms;
        var twoFactorEnabled = true;

        // Act
        user.Update(newEmail, newUsername, newPhone, newFirstName, newLastName, newProvider, twoFactorEnabled);

        // Assert
        Assert.Equal(newEmail.Value, user.Email);
        Assert.Equal(newUsername.Value, user.UserName);
        Assert.Equal(newPhone.Value, user.PhoneNumber);
        Assert.Equal(newFirstName.Value, user.FirstName);
        Assert.Equal(newLastName.Value, user.LastName);
        Assert.Equal(newProvider, user.TwoFactorProvider);
        Assert.True(user.TwoFactorEnabled);
    }

    //------------------------------------//

    [Fact]
    public void UpdateAddress_ShouldModifyUserAddress()
    {
        // Arrange
        var user = AppUserDataFactory.AnyUser;
        var newAddress = IdentityAddress.Create(
            AddressLine.Create("123 Main St"),
            AddressLine.Create("Apt 4B"),
            AddressLineNullable.Create("Suite 5"),
            AddressLineNullable.Create("Building 6"),
            AddressLineNullable.Create("Floor 7"),
            AreaCodeNullable.Create("10001"),
            ShortNotesNullable.Create("Near Central Park")
        );

        // Act
        user.UpdateAddress(newAddress);

        // Assert
        Assert.Equal(newAddress, user.Address);
        user.GetDomainEvents().ShouldContain(e => e is UserAddressUpdatedDomainEvent);
    }

    //------------------------------------//

}//Cls