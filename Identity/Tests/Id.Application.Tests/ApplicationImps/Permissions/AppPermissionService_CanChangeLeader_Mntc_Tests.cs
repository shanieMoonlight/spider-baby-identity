using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.AppImps.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.ApplicationImps.Permissions;

public class AppPermissionService_CanChangeLeader_Mntc_Tests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly CanChangeLeaderPermissions<AppUser> _service;

    public AppPermissionService_CanChangeLeader_Mntc_Tests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _service = new CanChangeLeaderPermissions<AppUser>(_mockTeamManager.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangeLeader_Mntc_Async_Should_Return_NotFoundWhen_TeamIdMissing()
    {
        // Arrange
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.IsCustomer).Returns(true);

        // Act
        var result = await _service.CanChange_SpecifiedTeam_LeaderAsync(null, Guid.NewGuid(), request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<Team>>();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangeLeader_Mntc_Async_Should_Return_NotFoundWhen_NewLeaderIdMissing()
    {
        // Arrange
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.IsCustomer).Returns(true);

        // Act
        var result = await _service.CanChange_SpecifiedTeam_LeaderAsync(Guid.NewGuid(), null, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<Team>>();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangeLeader_Mntc_Async_Should_Return_Forbidden_If_Request_IsCustomer()
    {
        // Arrange
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.IsCustomer).Returns(true);

        // Act
        var result = await _service.CanChange_SpecifiedTeam_LeaderAsync(Guid.NewGuid(), Guid.NewGuid(), request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<Team>>();
        result.Forbidden.ShouldBeTrue();
    }

    //------------------------------------//


    [Fact]
    public async Task CanChangeLeader_Mntc_Async_Should_Return_NotFound_If_Team_Not_Found()
    {
        // Arrange
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.IsCustomer).Returns(false);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _mockTeamManager.Setup(m => m.GetByIdWithMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Team)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act
        var result = await _service.CanChange_SpecifiedTeam_LeaderAsync(Guid.NewGuid(), Guid.NewGuid(), request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<Team>>();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangeLeader_Mntc_Async_Should_Return_Forbidden_If_Request_IsMntc_And_Team_Is_Super()
    {
        // Arrange
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.IsCustomer).Returns(false);
        request.Setup(r => r.IsMntc).Returns(true);
        var team = TeamDataFactory.Create(teamType: TeamType.super);
        _mockTeamManager.Setup(m => m.GetByIdWithMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(team);

        // Act
        var result = await _service.CanChange_SpecifiedTeam_LeaderAsync(Guid.NewGuid(), Guid.NewGuid(), request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<Team>>();
        result.Forbidden.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangeLeader_Mntc_Async_Should_Return_BadRequest_If_NewLeader_Not_In_Team()
    {
        // Arrange
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.IsCustomer).Returns(false);
        var team = TeamDataFactory.Create();
        _mockTeamManager.Setup(m => m.GetByIdWithMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(team);

        // Act
        var result = await _service.CanChange_SpecifiedTeam_LeaderAsync(Guid.NewGuid(), Guid.NewGuid(), request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<Team>>();
        result.BadRequest.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangeLeader_Mntc_Async_Should_Return_Success_If_Conditions_Met()
    {
        // Arrange
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.IsCustomer).Returns(false);
        request.Setup(r => r.IsSuper).Returns(true);
        var teamId = Guid.NewGuid();
        var newLeader = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(leader:newLeader);
        
        _mockTeamManager.Setup(m => m.GetByIdWithMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(team);

        // Act
        var result = await _service.CanChange_SpecifiedTeam_LeaderAsync(team.Id, newLeader.Id, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<Team>>();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBeEquivalentTo(team);
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangeLeader_Mntc_Async_Should_Return_Forbidden_If_NOtLeaderOfTeam_AfterOtherCondtionsMet_1()
    {
        // Arrange
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.IsCustomer).Returns(false);
        request.Setup(r => r.IsMntc).Returns(true);
        request.Setup(r => r.PrincipalUserId).Returns(Guid.NewGuid());

        var teamId = Guid.NewGuid();
        var newLeader = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(id:teamId, teamType: TeamType.maintenance, leader: newLeader); //different from PrincipalUserId

        _mockTeamManager.Setup(m => m.GetByIdWithMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(team);

        // Act
        var result = await _service.CanChange_SpecifiedTeam_LeaderAsync(team.Id, newLeader.Id, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<Team>>();
        result.Succeeded.ShouldBeFalse();
        result.Forbidden.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangeLeader_Mntc_Async_Should_Return_Forbidden_If_NOtLeaderOfTeam_AfterOtherCondtionsMet_2()
    {
        // Arrange
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.IsCustomer).Returns(false);
        request.Setup(r => r.IsSuper).Returns(true);
        request.Setup(r => r.PrincipalUserId).Returns(Guid.NewGuid());

        var teamId = Guid.NewGuid();
        var newLeader = AppUserDataFactory.Create(teamId: teamId);
        var team = TeamDataFactory.Create(
            id: teamId, 
            teamType: TeamType.super, 
            leaderId: Guid.NewGuid(),//different from PrincipalUserId
            members: [newLeader]); //Must be already in team

        _mockTeamManager.Setup(m => m.GetByIdWithMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(team);

        // Act
        var result = await _service.CanChange_SpecifiedTeam_LeaderAsync(team.Id, newLeader.Id, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<Team>>();
        result.Succeeded.ShouldBeFalse();
        result.Forbidden.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanChangeLeader_Mntc_Async_Should_Success_WhenCondtionsMet_2()
    {
        // Arrange
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        var leaderId = Guid.NewGuid();
        request.Setup(r => r.IsCustomer).Returns(false);
        request.Setup(r => r.IsSuper).Returns(true);
        request.Setup(r => r.PrincipalUserId).Returns(leaderId);

        var teamId = Guid.NewGuid();
        var newLeader = AppUserDataFactory.Create(id: leaderId, teamId: teamId);
        var team = TeamDataFactory.Create(id: teamId, teamType: TeamType.super, leader: newLeader); //different from PrincipalUserId


        _mockTeamManager.Setup(m => m.GetByIdWithMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(team);

        // Act
        var result = await _service.CanChange_SpecifiedTeam_LeaderAsync(team.Id, newLeader.Id, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<Team>>();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBeEquivalentTo(team);
    }

    //------------------------------------//

}//Cls
