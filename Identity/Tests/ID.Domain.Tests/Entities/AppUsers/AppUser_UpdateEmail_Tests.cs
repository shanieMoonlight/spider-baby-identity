using ClArch.ValueObjects;
using ID.Domain.Entities.AppUsers.Events;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.AppUsers;

public class AppUser_UpdateEmail_Tests
{
    [Fact]
    public void UpdateEmailAddress_Should_RaiseDomainEvent_When_Email_Is_Different()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id, emailConfirmed: true);
        var newEmail = EmailAddress.Create("newemail@example.com");

        // Act
        appUser.UpdateEmailAddress(newEmail);

        // Assert
        appUser.Email.ShouldBe(newEmail.Value);
        appUser.EmailConfirmed.ShouldBeFalse();
        appUser.GetDomainEvents().ShouldContain(e => e is UserEmailUpdatedDomainEvent);
    }

    //------------------------------------//

    [Fact]
    public void UpdateEmailAddress_Should_Not_RaiseDomainEvent_When_Email_Is_Same()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id, emailConfirmed: true);
        var sameEmail = EmailAddress.Create(appUser.Email!);

        // Act
        appUser.UpdateEmailAddress(sameEmail);

        // Assert
        appUser.GetDomainEvents().ShouldNotContain(e => e is UserEmailUpdatedDomainEvent);
        appUser.EmailConfirmed.ShouldBeTrue(); //SHould not change
    }

    //------------------------------------//

}//Cls
