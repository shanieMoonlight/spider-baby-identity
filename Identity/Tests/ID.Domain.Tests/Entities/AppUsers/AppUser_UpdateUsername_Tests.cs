using ClArch.ValueObjects;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.AppUsers;

public class AppUserTests
{
    [Fact]
    public void UpdateUsername_ShouldUpdateUserName_WhenValidUsernameIsProvided()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id);
        var newUsername = UsernameNullable.Create("newUsername");

        // Act
        appUser.UpdateUsername(newUsername);

        // Assert
        appUser.UserName.ShouldBe(newUsername.Value);
    }

    //------------------------------------//

    [Fact]
    public void UpdateUsername_ShouldSetUserNameToEmail_WhenUsernameIsNullOrWhitespace()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var appUser = AppUserDataFactory.Create(team.Id);
        var newUsername = UsernameNullable.Create(null);

        // Act
        appUser.UpdateUsername(newUsername);

        // Assert
        appUser.UserName.ShouldBe(appUser.Email);
    }

    //------------------------------------//

}//Cls
