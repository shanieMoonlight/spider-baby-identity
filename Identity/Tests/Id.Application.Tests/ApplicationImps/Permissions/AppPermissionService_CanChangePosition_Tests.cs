using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.AppImps.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Tests.Permissions;

public class AppPermissionServiceTests
{
    private readonly CanChangePositionPermissions<AppUser> _appPermissionService;

    public AppPermissionServiceTests()
    {
        _appPermissionService = new CanChangePositionPermissions<AppUser>();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangePositionAsync_PrincipalHasLowerPosition_ReturnsBadRequest()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalPosition = 2;
        var principalUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: principalPosition);
        var userToUpdate = AppUserDataFactory.Create(teamId: teamId, teamPosition: principalPosition - 1);
        var principalTeam = TeamDataFactory.Create(id: teamId, members: [userToUpdate, principalUser]);
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalUser).Returns(principalUser);
        request.Setup(r => r.PrincipalTeam).Returns(principalTeam);

        // Act
        var result = await _appPermissionService.CanChangePositionAsync(userToUpdate.Id, principalPosition + 1, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangePositionAsync_UserNotInTeam_ReturnsForbidden()
    {
        // Arrange
        var principalUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), teamPosition: 1);
        var principalTeam = TeamDataFactory.Create(id: Guid.NewGuid(), leader: principalUser);
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalUser).Returns(principalUser);
        request.Setup(r => r.PrincipalTeam).Returns(principalTeam);

        // Act
        var result = await _appPermissionService.CanChangePositionAsync(Guid.NewGuid(), 1, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Succeeded.ShouldBeFalse();
        result.Forbidden.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangePositionAsync_PrincipalHasLowerPositionThanUser_ReturnsBadRequest()
    {
        // Arrange
        var principalUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), teamPosition: 1);
        var newPositionUser = AppUserDataFactory.Create(teamId: principalUser.TeamId, teamPosition: 2);
        var principalTeam = TeamDataFactory.Create(id: principalUser.TeamId, members: [newPositionUser, principalUser]);
        //principalTeam.Members.Add(newPositionUser);
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalUser).Returns(principalUser);
        request.Setup(r => r.PrincipalTeam).Returns(principalTeam);

        // Act
        var result = await _appPermissionService.CanChangePositionAsync(newPositionUser.Id, 1, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangePositionAsync_Should_Return_BadRequest_When_User_Is_Leader()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: 20);
        var newPositionUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: 10); // in the same team

        var principalTeam = TeamDataFactory.Create(id: principalUser.TeamId, leader: newPositionUser, members: [newPositionUser]); //updatee is the leader

        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalUser).Returns(principalUser);
        request.Setup(r => r.PrincipalTeam).Returns(principalTeam);


        // Act
        var result = await _appPermissionService.CanChangePositionAsync(newPositionUser.Id, newPositionUser.TeamPosition + 1, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.Teams.CANT_CHANGE_POSITION_OF_LEADER);
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangePositionAsync_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: 2);
        var newPositionUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: 1);
        var principalTeam = TeamDataFactory.Create(id: teamId, leader: principalUser, members: [newPositionUser]);


        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalUser).Returns(principalUser);
        request.Setup(r => r.PrincipalTeam).Returns(principalTeam);

        // Act
        var result = await _appPermissionService.CanChangePositionAsync(newPositionUser.Id, 2, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(newPositionUser);
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangePositionAsync_ValidRequest_ReturnsSuccess_NOT_LEADER()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: 2);
        var newPositionUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: 1);
        var principalTeam = TeamDataFactory.Create(id: teamId, members: [principalUser, newPositionUser]);



        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalUser).Returns(principalUser);
        request.Setup(r => r.PrincipalTeam).Returns(principalTeam);

        // Act
        var result = await _appPermissionService.CanChangePositionAsync(newPositionUser.Id, 2, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(newPositionUser);
    }

    //------------------------------------//

}//Cls
