//using ID.Tests.Data.Factories;

//namespace ID.Domain.Tests.Entities.AppUsers;

//public class AppUser_SetTeam_Tests
//{
//    [Fact]
//    public void Create_ShouldSetTeamId()
//    {
//        // Arrange
//        var team = TeamDataFactory.Create();
//        var user = AppUserDataFactory.Create(Guid.NewGuid());


//        // Assert
//        Assert.NotEqual(team.Id, user.TeamId);

//        // Act
//        user.SetTeam(team);

//        // Assert
//        Assert.Equal(team.Id, user.TeamId);
//    }
//}