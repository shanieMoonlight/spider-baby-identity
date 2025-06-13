using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.AppUsers;

public class AppUser_SetTkn_Tests
{
    [Fact]
    public void SetTkn_Should_Set_Tkn_Property()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser = AppUserDataFactory.Create(teamId);
        var expectedTkn = "newToken";

        // Act
        appUser.SetTkn(expectedTkn);

        // Assert
        appUser.Tkn.ShouldBe(expectedTkn);
    }


}//Cls
