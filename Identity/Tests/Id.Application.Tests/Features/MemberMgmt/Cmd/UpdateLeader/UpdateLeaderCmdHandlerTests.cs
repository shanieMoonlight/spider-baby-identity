using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.Features.Teams;
using ID.Application.AppAbs.Permissions;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;
using ID.Application.Features.MemberMgmt.Cmd.UpdateMyTeamLeader;

namespace ID.Application.Tests.Features.MemberMgmt.Cmd.UpdateLeader;

public class UpdateLeaderCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly Mock<IAppPermissionService<AppUser>> _appPermissionsMock;
    private readonly Mock<ICanChangeLeaderPermissions<AppUser>> _canChangeLeaderPermissionService;
    private readonly UpdateMyTeamLeaderCmdHandler _handler;

    public UpdateLeaderCmdHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _appPermissionsMock = new Mock<IAppPermissionService<AppUser>>();
        _canChangeLeaderPermissionService = new Mock<ICanChangeLeaderPermissions<AppUser>>();
        _appPermissionsMock.Setup(x => x.ChangeLeaderPermissions)
            .Returns(_canChangeLeaderPermissionService.Object);
        _handler = new UpdateMyTeamLeaderCmdHandler(_mockTeamManager.Object, _appPermissionsMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldCall_CanChangeLeaderAsync()
    {
        // Arrange
        var newLeaderId = Guid.NewGuid();
        var team = TeamDataFactory.Create();
        var newLeader = AppUserDataFactory.Create(team.Id);
        var request = new UpdateMyTeamLeaderCmd(newLeaderId) { PrincipalTeam = team };

        _canChangeLeaderPermissionService.Setup(x => x.CanChange_MyTeam_LeaderAsync(newLeaderId, request))
            .ReturnsAsync(GenResult<AppUser>.Success(newLeader));

        _mockTeamManager.Setup(x => x.SetLeaderAsync(team, newLeader))
            .ReturnsAsync(GenResult<Team>.Success(team));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        _canChangeLeaderPermissionService.Verify(x => x.CanChange_MyTeam_LeaderAsync(newLeaderId, request), Times.AtLeastOnce);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenLeaderIsUpdated()
    {
        // Arrange
        var newLeaderId = Guid.NewGuid();
        var team = TeamDataFactory.Create();
        var newLeader = AppUserDataFactory.Create(team.Id);
        var request = new UpdateMyTeamLeaderCmd(newLeaderId) { PrincipalTeam = team };

        _canChangeLeaderPermissionService.Setup(x => x.CanChange_MyTeam_LeaderAsync(newLeaderId, request))
            .ReturnsAsync(GenResult<AppUser>.Success(newLeader));

        _mockTeamManager.Setup(x => x.SetLeaderAsync(team, newLeader))
            .ReturnsAsync(GenResult<Team>.Success(team));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<TeamDto>();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenPermissionCheckFails()
    {
        // Arrange
        var newLeaderId = Guid.NewGuid();
        var team = TeamDataFactory.Create();
        var request = new UpdateMyTeamLeaderCmd(newLeaderId) { PrincipalTeam = team };

        _canChangeLeaderPermissionService.Setup(x => x.CanChange_MyTeam_LeaderAsync(newLeaderId, request))
            .ReturnsAsync(GenResult<AppUser>.Failure("Permission denied"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe("Permission denied");
    }

    //------------------------------------//

}//Cls