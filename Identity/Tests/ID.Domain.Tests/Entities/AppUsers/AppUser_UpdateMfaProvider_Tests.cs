using ID.Domain.Entities.AppUsers.Events;
using ID.Domain.Models;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.AppUsers;

public class AppUser_UpdateMfaProvider_Tests
{
    [Fact]
    public void Update2FactorProvider_ShouldRaiseEvent_WhenProviderIsDifferent()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team: team);
        var newProvider = TwoFactorProvider.Sms;

        // Act
        appUser.Update2FactorProvider(newProvider);

        // Assert
        var domainEvents = appUser.GetDomainEvents();
        appUser.TwoFactorProvider.ShouldBe(newProvider);
        domainEvents.ShouldContain(e => e is User2FactorUpdatedDomainEvent);
    }

    //------------------------------------//

    [Fact]
    public void Update2FactorProvider_ShouldNotRaiseEvent_WhenProviderIsSame()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team:team);
        var sameProvider = appUser.TwoFactorProvider;

        // Act
        appUser.Update2FactorProvider(sameProvider);

        // Assert
        var domainEvents = appUser.GetDomainEvents();
        domainEvents.ShouldNotContain(e => e is User2FactorUpdatedDomainEvent);
    }

    //------------------------------------//

}//Cls
