using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.AppImps.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.Permissions;

public class AppPermission_CanDeleteAsync_ServiceTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamManagerMock;
    private readonly CanChangeLeaderPermissions<AppUser> _appPermissionService;

    public AppPermission_CanDeleteAsync_ServiceTests()
    {
        _teamManagerMock = new Mock<IIdentityTeamManager<AppUser>>();
        _appPermissionService = new CanChangeLeaderPermissions<AppUser>(_teamManagerMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangeLeaderAsync_PrincipalNotLeader_ReturnsForbiddenResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId);
        var newLeader = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(leaderId: Guid.NewGuid(), members: [principalUser, newLeader]);// Different leader

        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalUserId).Returns(principalUser.Id);
        request.Setup(r => r.PrincipalTeam).Returns(team);

        // Act
        var result = await _appPermissionService.CanChange_MyTeam_LeaderAsync(newLeader.Id, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Forbidden.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangeLeaderAsync_NewLeaderNotInTeam_ReturnsBadRequestResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId);
        var newLeader = AppUserDataFactory.Create(teamId: Guid.NewGuid()); //Different team

        var team = TeamDataFactory.Create(id: teamId, leader: principalUser);


        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalUserId).Returns(principalUser.Id);
        request.Setup(r => r.PrincipalTeam).Returns(team);

        // Act
        var result = await _appPermissionService.CanChange_MyTeam_LeaderAsync(newLeader.Id, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.BadRequest.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangeLeaderAsync_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId);
        var newLeader = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, leader: principalUser, members: [newLeader]);

        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalUserId).Returns(principalUser.Id);
        request.Setup(r => r.PrincipalTeam).Returns(team);

        // Act
        var result = await _appPermissionService.CanChange_MyTeam_LeaderAsync(newLeader.Id, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(newLeader);
    }

    //------------------------------------//

}//Cls
