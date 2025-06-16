namespace ID.Application.Tests.Features.MemberMgmt.Cmd.UpdatePosition;

public class UpdatePositionCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamMgrMock;
    private readonly Mock<IAppPermissionService<AppUser>> _appPermissionsMock;
    private readonly Mock<ICanChangePositionPermissions<AppUser>> _canUpdatePosiitonPermissionService;
    private readonly UpdatePositionCmdHandler _handler;

    public UpdatePositionCmdHandlerTests()
    {
        _teamMgrMock = new Mock<IIdentityTeamManager<AppUser>>();
        _appPermissionsMock = new Mock<IAppPermissionService<AppUser>>();
        _canUpdatePosiitonPermissionService = new Mock<ICanChangePositionPermissions<AppUser>>();
        _appPermissionsMock.Setup(x => x.ChangePositionPermissions)
            .Returns(_canUpdatePosiitonPermissionService.Object);
        _handler = new UpdatePositionCmdHandler(_teamMgrMock.Object, _appPermissionsMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldCall_CanChangePositionAsync()
    {
        // Arrange
        var newLeaderId = Guid.NewGuid();
        var team = TeamDataFactory.Create();
        var newPositionUser = AppUserDataFactory.Create(team.Id);
        var newPosition = 5;
        var dto = new UpdatePositionDto(newLeaderId, newPosition);
        var request = new UpdatePositionCmd(dto) { PrincipalTeam = team };

        _canUpdatePosiitonPermissionService.Setup(x => x.CanChangePositionAsync(newLeaderId, newPosition, request))
            .ReturnsAsync(GenResult<AppUser>.Success(newPositionUser));

        _teamMgrMock.Setup(x => x.SetLeaderAsync(team, newPositionUser))
            .ReturnsAsync(GenResult<Team>.Success(team));

        _teamMgrMock.Setup(x => x.UpdateMemberAsync(team, It.IsAny<AppUser>()))
            .ReturnsAsync(GenResult<AppUser>.Success(newPositionUser));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        _canUpdatePosiitonPermissionService.Verify(x => x.CanChangePositionAsync(newLeaderId, newPosition, request), Times.AtLeastOnce);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Update_Position_When_Permission_Granted()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser = AppUserDataFactory.Create(teamId: teamId);
        var expectedTeam = TeamDataFactory.Create(id: teamId, members: [appUser]);
        var dto = new UpdatePositionDto(appUser.Id, 2);
        var request = new UpdatePositionCmd(dto)
        {
            PrincipalTeam = expectedTeam
        };

        _canUpdatePosiitonPermissionService.Setup(x => x.CanChangePositionAsync(appUser.Id, 2, request))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));

        _teamMgrMock.Setup(x => x.UpdateMemberAsync(expectedTeam, appUser))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TeamPosition.ShouldBe(2);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Permission_Denied()
    {
        // Arrange
        var appUser = AppUserDataFactory.Create(teamId: Guid.NewGuid());
        var expectedTeam = TeamDataFactory.Create(id: Guid.NewGuid());
        var dto = new UpdatePositionDto(appUser.Id, 2);
        var request = new UpdatePositionCmd(dto)
        {
            PrincipalTeam = expectedTeam
        };

        _canUpdatePosiitonPermissionService.Setup(x => x.CanChangePositionAsync(appUser.Id, 2, request))
            .ReturnsAsync(GenResult<AppUser>.Failure("Permission denied"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe("Permission denied");
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Return_Failure_When_MemberPositionUpdate_Validation_Fails()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var otherTeamId = Guid.NewGuid();

        // Create a user that belongs to a different team (will cause validation failure)
        var appUser = AppUserDataFactory.Create(teamId: otherTeamId);
        var team = TeamDataFactory.Create(id: teamId); // Different team

        var dto = new UpdatePositionDto(appUser.Id, 5);
        var request = new UpdatePositionCmd(dto)
        {
            PrincipalTeam = team
        };

        // Setup permission service to succeed (so we can test validation failure)
        _canUpdatePosiitonPermissionService.Setup(x => x.CanChangePositionAsync(appUser.Id, 5, request))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue(); // Or whatever your validation message is

        // Verify that UpdateMemberAsync was NOT called since validation failed
        _teamMgrMock.Verify(x => x.UpdateMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Never);
    }

}//Cls