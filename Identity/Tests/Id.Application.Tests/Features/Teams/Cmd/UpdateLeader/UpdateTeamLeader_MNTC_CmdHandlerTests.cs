using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.AppAbs.Permissions;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;
using ID.Application.Features.MemberMgmt.Cmd.UpdateLeaderMntc;

namespace ID.Application.Tests.Features.Teams.Cmd.UpdateLeader;

public class UpdateTeamLeader_MNTC_CmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly Mock<IAppPermissionService<AppUser>> _appPermissionsMock;
    private readonly Mock<ICanChangeLeaderPermissions<AppUser>> _canChangeLeaderPermissionService;
    private readonly UpdateTeamLeaderCmdHandler _handler;

    public UpdateTeamLeader_MNTC_CmdHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _appPermissionsMock = new Mock<IAppPermissionService<AppUser>>();
        _canChangeLeaderPermissionService = new Mock<ICanChangeLeaderPermissions<AppUser>>();
        _appPermissionsMock.Setup(x => x.ChangeLeaderPermissions)
            .Returns(_canChangeLeaderPermissionService.Object);
        _handler = new UpdateTeamLeaderCmdHandler(_mockTeamManager.Object, _appPermissionsMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Update_Team_Leader_When_Permission_Granted()
    {
        // Arrange
        var expectedTeamId = Guid.NewGuid();
        var appUser = AppUserDataFactory.Create(teamId: expectedTeamId);
        var expectedTeam = TeamDataFactory.Create(id: expectedTeamId, members: [appUser]);
        var newLeaderId = Guid.NewGuid();
        var request = new UpdateTeamLeaderCmd(new UpdateTeamLeaderDto(newLeaderId, expectedTeam.Id));

        _canChangeLeaderPermissionService
            .Setup(x => x.CanChange_SpecifiedTeam_LeaderAsync(expectedTeam.Id, newLeaderId, request))
            .ReturnsAsync(GenResult<Team>.Success(expectedTeam));

        _mockTeamManager
            .Setup(x => x.SetLeaderAsync(expectedTeam, It.IsAny<AppUser>()))
            .ReturnsAsync(GenResult<Team>.Success(expectedTeam));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(expectedTeam.Id);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Permission_Denied()
    {
        // Arrange
        var appUser = AppUserDataFactory.Create(teamId: Guid.NewGuid());
        var expectedTeam = TeamDataFactory.Create(id: Guid.NewGuid());
        var newLeaderId = Guid.NewGuid();
        var request = new UpdateTeamLeaderCmd(new UpdateTeamLeaderDto(newLeaderId, expectedTeam.Id));

        _canChangeLeaderPermissionService
            .Setup(x => x.CanChange_SpecifiedTeam_LeaderAsync(expectedTeam.Id, newLeaderId, request))
            .ReturnsAsync(GenResult<Team>.Failure("Permission denied"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe("Permission denied");
    }

    //------------------------------------//
}