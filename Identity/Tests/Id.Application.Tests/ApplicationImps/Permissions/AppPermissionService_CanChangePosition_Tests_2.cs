using ID.Application.AppImps.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.Application.Tests.Permissions;

public class AppPermissionService_CanChangePosition_Tests_2
{

    public AppPermissionService_CanChangePosition_Tests_2()
    {
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangePositionAsync_UserDoesNotExist_ReturnsForbiddenResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId);
        var principalTeam = TeamDataFactory.Create(id: teamId, members: [principalUser]);
        var requestMock = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        requestMock.Setup(r => r.PrincipalUser).Returns(principalUser);
        requestMock.Setup(r => r.PrincipalTeam).Returns(principalTeam);

        var sut = new CanChangePositionPermissions<AppUser>();

        // Act
        var result = await sut.CanChangePositionAsync(Guid.NewGuid(), 1, requestMock.Object);

        // Assert
        result.Forbidden.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Users.CantUpdateFromAnotherTeam);
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangePositionAsync_PrincipalIsLeader_ReturnsSuccess()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId);
        var newPositionUser = AppUserDataFactory.Create(teamId: teamId);
        var principalTeam = TeamDataFactory.Create(id: teamId, leader: principalUser, members: [principalUser, newPositionUser]);
        var requestMock = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        requestMock.Setup(r => r.PrincipalUser).Returns(principalUser);
        requestMock.Setup(r => r.PrincipalTeam).Returns(principalTeam);

        var sut = new CanChangePositionPermissions<AppUser>();

        // Act
        var result = await sut.CanChangePositionAsync(newPositionUser.Id, 1, requestMock.Object);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(newPositionUser);
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangePositionAsync_NewPositionUserIsLeader_ReturnsBadRequest()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId);
        var newPositionUser = AppUserDataFactory.Create(teamId: teamId);
        var principalTeam = TeamDataFactory.Create(id: teamId, leader: newPositionUser, members: [principalUser, newPositionUser]);
        var requestMock = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        requestMock.Setup(r => r.PrincipalUser).Returns(principalUser);
        requestMock.Setup(r => r.PrincipalTeam).Returns(principalTeam);

        var sut = new CanChangePositionPermissions<AppUser>();

        // Act
        var result = await sut.CanChangePositionAsync(newPositionUser.Id, 1, requestMock.Object);

        // Assert
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Teams.CANT_CHANGE_POSITION_OF_LEADER);
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangePositionAsync_PrincipalHasLowerPosition_ReturnsBadRequest()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: 1);
        var newPositionUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: 2);
        var principalTeam = TeamDataFactory.Create(id: teamId, members: [principalUser, newPositionUser]);
        var requestMock = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        requestMock.Setup(r => r.PrincipalUser).Returns(principalUser);
        requestMock.Setup(r => r.PrincipalTeam).Returns(principalTeam);

        var sut = new CanChangePositionPermissions<AppUser>();

        // Act
        var result = await sut.CanChangePositionAsync(newPositionUser.Id, 3, requestMock.Object);

        // Assert
        result.BadRequest.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Teams.CANT_ADMINISTER_HIGHER_POSITIONS(3));
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangePositionAsync_PrincipalHasHigherPosition_ReturnsSuccess()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: 3);
        var newPositionUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: 2);
        var principalTeam = TeamDataFactory.Create(id: teamId, members: [principalUser, newPositionUser]);
        var requestMock = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        requestMock.Setup(r => r.PrincipalUser).Returns(principalUser);
        requestMock.Setup(r => r.PrincipalTeam).Returns(principalTeam);

        var sut = new CanChangePositionPermissions<AppUser>();

        // Act
        var result = await sut.CanChangePositionAsync(newPositionUser.Id, 1, requestMock.Object);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(newPositionUser);
    }

    //------------------------------------//

}//Cls
