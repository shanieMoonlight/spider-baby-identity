using ID.Tests.Data.Factories;
using Moq;
using Shouldly;
using ID.Application.AppImps.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.ApplicationImps.Permissions;

public class AppPermissionService_CanDeleteTeamAware_Tests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamManagerMock;
    private readonly CanDeletePermissions<AppUser> _service;

    public AppPermissionService_CanDeleteTeamAware_Tests()
    {
        _teamManagerMock = new Mock<IIdentityTeamManager<AppUser>>();
        _service = new CanDeletePermissions<AppUser>(_teamManagerMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task CanDeleteAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeam).Returns(team);
        request.Setup(r => r.PrincipalTeamId).Returns(team.Id);
        var deleteUserId = Guid.NewGuid();

        // Act
        var result = await _service.CanDeleteAsync(deleteUserId, request.Object);

        // Assert
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<AppUser>(deleteUserId));
    }

    //------------------------------------//

    [Fact]
    public async Task CanDeleteAsync_ShouldReturnBadRequest_WhenDeletingSelf()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(teamId:teamId);
        var team = TeamDataFactory.Create(id: teamId, members: [user]);

        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeam).Returns(team);
        request.Setup(r => r.PrincipalTeamId).Returns(team.Id);
        request.Setup(r => r.PrincipalUserId).Returns(user.Id);

        // Act
        var result = await _service.CanDeleteAsync(user.Id, request.Object);

        // Assert
        result.BadRequest.ShouldBeTrue();
        result.Succeeded.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public async Task CanDeleteAsync_ShouldReturnNotFound_WhenUserIsFromAnotherTeam()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var anotherTeamId = Guid.NewGuid();
        var user = AppUserDataFactory.Create(anotherTeamId);

        var team = TeamDataFactory.Create(id: teamId);
        var anotherTeam = TeamDataFactory.Create(id: anotherTeamId, members: [user]);

        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeam).Returns(team);
        request.Setup(r => r.PrincipalTeamId).Returns(team.Id);

        // Act
        var result = await _service.CanDeleteAsync(user.Id, request.Object);

        // Assert
        result.NotFound.ShouldBeTrue();
        result.Succeeded.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public async Task CanDeleteAsync_ShouldReturnForbidden_WhenPrincipalHasEqualPosition()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var maxPosition = 5; //Leader will be set to MaxPosition so start with that
        var principalUser = AppUserDataFactory.Create(teamId:teamId, teamPosition: maxPosition);
        var deleteUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: maxPosition);
        var team = TeamDataFactory.Create(id: teamId, maxPosition:maxPosition, members: [principalUser, deleteUser]);
        
        
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeam).Returns(team);
        request.Setup(r => r.PrincipalTeamId).Returns(team.Id);
        request.Setup(r => r.PrincipalUserId).Returns(principalUser.Id);
        request.Setup(r => r.PrincipalTeamPosition).Returns(principalUser.TeamPosition);

        // Act
        var result = await _service.CanDeleteAsync(deleteUser.Id, request.Object);

        // Assert
        result.Forbidden.ShouldBeTrue();
        result.Succeeded.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public async Task CanDeleteAsync_ShouldReturnForbidden_WhenPrincipalHasLowerPosition_AndNOtTheLeader()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalPosition = 3;
        var principalUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: principalPosition);
        var deleteUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: principalPosition + 2);
        var team = TeamDataFactory.Create(id: teamId, members: [principalUser, deleteUser]);

        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeam).Returns(team);
        request.Setup(r => r.PrincipalTeamId).Returns(team.Id);
        request.Setup(r => r.PrincipalUserId).Returns(principalUser.Id);
        request.Setup(r => r.PrincipalTeamPosition).Returns(principalUser.TeamPosition);

        // Act
        var result = await _service.CanDeleteAsync(deleteUser.Id, request.Object);

        // Assert
        result.Forbidden.ShouldBeTrue();
        result.Succeeded.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public async Task CanDeleteAsync_ShouldReturnSUccess_WhenPrincipalHasLowerPosition_But_IS_TheLeader()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalPosition = 3;
        var principalUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: principalPosition);
        var deleteUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: principalPosition + 2);

        var team = TeamDataFactory.Create(id: teamId, members: [principalUser, deleteUser]);



        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeam).Returns(team);
        request.Setup(r => r.PrincipalTeamId).Returns(team.Id);
        request.Setup(r => r.IsLeader).Returns(true);
        request.Setup(r => r.PrincipalUserId).Returns(principalUser.Id);
        request.Setup(r => r.PrincipalTeamPosition).Returns(principalUser.TeamPosition);

        // Act
        var result = await _service.CanDeleteAsync(deleteUser.Id, request.Object);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBeEquivalentTo(deleteUser);
    }

    //------------------------------------//


    [Fact]
    public async Task CanDeleteAsync_ShouldReturnSuccess_WhenAllConditionsAreMet()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: 2);
        var deleteUser = AppUserDataFactory.Create(teamId: teamId, teamPosition: 1);
        var team = TeamDataFactory.Create(id: teamId, members: [principalUser, deleteUser]);


        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeam).Returns(team);
        request.Setup(r => r.PrincipalTeamId).Returns(team.Id);
        request.Setup(r => r.PrincipalUserId).Returns(principalUser.Id);
        request.Setup(r => r.PrincipalTeamPosition).Returns(principalUser.TeamPosition);

        // Act
        var result = await _service.CanDeleteAsync(deleteUser.Id, request.Object);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBeEquivalentTo(deleteUser);
    }

    //------------------------------------//
}
