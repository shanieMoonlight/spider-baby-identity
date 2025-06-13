using ID.Application.Features.Teams.Qry.GetAll;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Qry.GetAll;

public class GetAllTeamsQryHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly GetAllTeamsQryHandler _handler;

    public GetAllTeamsQryHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new GetAllTeamsQryHandler(_mockTeamManager.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnAllTeams_IsMntc_IsNotSuper()
    {
        // Arrange
        var cusTeam = TeamDataFactory.Create(teamType: TeamType.Customer);
        var mntcTeam = TeamDataFactory.Create(teamType: TeamType.Maintenance);
        var superTeam = TeamDataFactory.Create(teamType: TeamType.Super);
        var teams = new List<Team>
        {
            cusTeam,
            mntcTeam,
            superTeam,
        };
        _mockTeamManager.Setup(mgr => mgr.GetAllTeams(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([cusTeam, mntcTeam,]);

        var request = new GetAllTeamsQry { IsMntc = true, IsSuper = false };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        _mockTeamManager.Verify(mgr => mgr.GetAllTeams(true, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnAllTeams_IsSuper_INcludesIsMntc()
    {
        // Arrange
        var cusTeam = TeamDataFactory.Create(teamType: TeamType.Customer);
        var mntcTeam = TeamDataFactory.Create(teamType: TeamType.Maintenance);
        var superTeam = TeamDataFactory.Create(teamType: TeamType.Super);
        var teams = new List<Team>
        {
            cusTeam,
            mntcTeam,
            superTeam,
        };
        _mockTeamManager.Setup(mgr => mgr.GetAllTeams(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([cusTeam, mntcTeam,]);

        var request = new GetAllTeamsQry { IsMntc = false, IsSuper = true };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        _mockTeamManager.Verify(mgr => mgr.GetAllTeams(true, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnAllTeams_IsSuper_AND_IsMntc()
    {
        // Arrange
        var cusTeam = TeamDataFactory.Create(teamType: TeamType.Customer);
        var mntcTeam = TeamDataFactory.Create(teamType: TeamType.Maintenance);
        var superTeam = TeamDataFactory.Create(teamType: TeamType.Super);
        var teams = new List<Team>
        {
            cusTeam,
            mntcTeam,
            superTeam,
        };
        _mockTeamManager.Setup(mgr => mgr.GetAllTeams(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([cusTeam, mntcTeam,]);

        var request = new GetAllTeamsQry { IsMntc = true, IsSuper = true };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        _mockTeamManager.Verify(mgr => mgr.GetAllTeams(true, true, It.IsAny<CancellationToken>()), Times.Once);
    }

    //------------------------------------//

}//Cls
