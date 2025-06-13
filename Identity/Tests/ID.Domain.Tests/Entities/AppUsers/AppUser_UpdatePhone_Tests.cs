using ClArch.ValueObjects;
using ID.Domain.Entities.AppUsers.Events;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.AppUsers;

public class AppUser_UpdatePhone_Tests
{
    [Fact]
    public void UpdatePhone_Should_RaiseDomainEvent_When_PhoneNumber_Is_Different()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id, phoneNumber: "0987654321", phoneNumberConfirmed: true);
        var newPhone = PhoneNullable.Create("1234567890");


        // Act
        var result = appUser.UpdatePhone(newPhone);

        // Assert
        result.PhoneNumber.ShouldBe(newPhone.Value);
        appUser.PhoneNumberConfirmed.ShouldBeFalse();
        appUser.GetDomainEvents().ShouldContain(e => e is UserPhoneUpdatedDomainEvent);
    }

    //------------------------------------//

    [Fact]
    public void UpdatePhone_Should_Not_RaiseDomainEvent_When_PhoneNumber_Is_Same()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id, phoneNumberConfirmed: true);
        var newPhone = PhoneNullable.Create(appUser.PhoneNumber);


        // Act
        var result = appUser.UpdatePhone(newPhone);

        // Assert
        result.PhoneNumber.ShouldBe(newPhone.Value);
        appUser.GetDomainEvents().ShouldNotContain(e => e is UserPhoneUpdatedDomainEvent);
        appUser.PhoneNumberConfirmed.ShouldBeTrue(); //SHould not change
    }

    //------------------------------------//

}//Cls
