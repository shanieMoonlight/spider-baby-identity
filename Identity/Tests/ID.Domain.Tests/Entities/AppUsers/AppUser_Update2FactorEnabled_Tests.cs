using ID.Domain.Entities.AppUsers.Events;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.AppUsers;

public class AppUser_Update2FactorEnabled_Tests
{
    public static IEnumerable<object[]> TwoFactorEnabledData =>
        [
            [true],
            [false]
        ];

    //------------------------------------//

    [Theory]
    [MemberData(nameof(TwoFactorEnabledData))]
    public void Update2FactoEnabled_Should_Update_TwoFactorEnabled_And_Raise_Event(bool enabled)
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id, twoFactorEnabled: !enabled);

        // Act
        appUser.Update2FactoEnabled(enabled);

        // Assert
        appUser.TwoFactorEnabled.ShouldBe(enabled);
        appUser.GetDomainEvents().ShouldContain(e => e is User2FactorEnableChangedDomainEvent);
    }

    //------------------------------------//

    [Theory]
    [MemberData(nameof(TwoFactorEnabledData))]
    public void Update2FactoEnabled_Should_NOTUpdate_TwoFactorEnabled_And_NOTRaise_Event_WhenEnabledIsTheSameValue(bool enabled)
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id, twoFactorEnabled: enabled);

        // Act
        appUser.Update2FactoEnabled(enabled);

        // Assert
        appUser.TwoFactorEnabled.ShouldBe(enabled);
        appUser.GetDomainEvents().ShouldNotContain(e => e is User2FactorEnableChangedDomainEvent);
    }

    //------------------------------------//
}
