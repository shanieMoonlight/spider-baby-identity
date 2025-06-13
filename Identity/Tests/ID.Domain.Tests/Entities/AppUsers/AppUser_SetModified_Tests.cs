using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.AppUsers;

public class AppUser_SetModified_Tests
{
    [Fact]
    public void SetModified_Should_Update_LastModifiedDate_And_AdministratorDetails()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser = AppUserDataFactory.Create(teamId: teamId);
        var username = "adminUser";
        var userId = "adminId";

        // Act
        appUser.SetModified(username, userId);

        // Assert
        appUser.LastModifiedDate.ShouldNotBeNull();
        appUser.LastModifiedDate.Value.ShouldBeGreaterThan(DateTime.MinValue);
        appUser.AdministratorUsername.ShouldBe(username);
        appUser.AdministratorId.ShouldBe(userId);
    }


}//Cls
