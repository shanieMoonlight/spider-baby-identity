using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.AppUsers;

public class AppUser_SetCreated_Tests
{
    [Fact]
    public void SetCreated_Should_Update_DateCreated_And_AdministratorDetails()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser = AppUserDataFactory.Create(teamId: teamId);
        var username = "adminUser";
        var userId = "adminId";

        // Act
        appUser.SetCreated(username, userId);

        // Assert
        appUser.DateCreated.ShouldNotBe(default);
        appUser.DateCreated.ShouldBeGreaterThan(DateTime.MinValue);
        appUser.AdministratorUsername.ShouldBe(username);
        appUser.AdministratorId.ShouldBe(userId);
    }


}//Cls
