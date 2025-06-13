using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Domain.Tests.Entities.AppUsers;

public class AppUser_Equals_Tests
{


    //------------------------------------//

    [Fact]
    public void Equals_ShouldReturnTrue_WhenIdsAreEqual()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser1 = AppUserDataFactory.Create(teamId: teamId);
        var appUser2 = AppUserDataFactory.Create(teamId: teamId, id: appUser1.Id);

        // Act
        var result = appUser1.Equals(appUser2);

        // Assert
        result.ShouldBeTrue();
    }



    //------------------------------------//


    [Fact]
    public void Equals_ShouldReturnFalse_WhenIdsAreNotEqual()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser1 = AppUserDataFactory.Create(teamId: teamId);
        var appUser2 = AppUserDataFactory.Create(teamId: teamId);

        // Act
        var result = appUser1.Equals(appUser2);

        // Assert
        result.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Equals_ShouldReturnFalse_WhenObjectIsNull()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser1 = AppUserDataFactory.Create(teamId: teamId);

        // Act
        var result = appUser1.Equals(null);

        // Assert
        result.ShouldBeFalse();
    }

    //------------------------------------//


    [Fact]
    public void Equals_ShouldReturnFalse_WhenObjectIsDifferentType()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser1 = AppUserDataFactory.Create(teamId: teamId);
        var differentTypeObject = new { appUser1.Id };

        // Act
        var result = appUser1.Equals(differentTypeObject);

        // Assert
        result.ShouldBeFalse();
    }

    //------------------------------------//

}//Cls
