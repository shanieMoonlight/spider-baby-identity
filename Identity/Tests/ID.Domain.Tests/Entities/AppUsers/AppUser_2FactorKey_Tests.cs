using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.AppUsers;

public class AppUser_2FactorKey_Tests
{
    [Fact]
    public void SetTwoFactorKey_Should_Set_TwoFactorKey_Property()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser = AppUserDataFactory.Create(teamId);
        var expectedKey = "newTwoFactorKey";

        // Act
        appUser.SetTwoFactorKey(expectedKey);

        // Assert
        appUser.TwoFactorKey.ShouldBe(expectedKey);
    }

}//Cls
