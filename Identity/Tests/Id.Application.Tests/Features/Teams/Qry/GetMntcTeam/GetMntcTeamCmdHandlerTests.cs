using ID.Tests.Data.Factories;
using Moq;
using Shouldly;
using ID.Application.Features.Teams;
using ID.Application.Features.Teams.Qry.GetMntcTeam;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.Features.Teams.Qry.GetMntcTeam;

public class GetMntcTeamQryHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly GetMntcTeamQryHandler _handler;

    public GetMntcTeamQryHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new GetMntcTeamQryHandler(_mockTeamManager.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnTeamDto_WhenTeamExists()
    {
        // Arrange
        var request = new GetMntcTeamQry { IsSuper = false, PrincipalTeamPosition = 2 };
        var team = TeamDataFactory.Create();
        _mockTeamManager.Setup(mgr => mgr.GetMntcTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(team);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<TeamDto>();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFoundResult_WhenTeamDoesNotExist()
    {
        // Arrange
        var teamPosition = 5;
        var request = new GetMntcTeamQry { IsSuper = false, PrincipalTeamPosition = teamPosition };
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _mockTeamManager.Setup(mgr => mgr.GetMntcTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync((Team)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
        _mockTeamManager.Verify(mgr => mgr.GetMntcTeamWithMembersAsync(teamPosition));
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShoulCallGetMntcTeamWithMembersAsync_With10000_ifIsSuper()
    {
        // Arrange
        var teamPosition = 5;
        var team = TeamDataFactory.Create();
        var request = new GetMntcTeamQry { IsSuper = true, PrincipalTeamPosition = teamPosition };
        _mockTeamManager.Setup(mgr => mgr.GetMntcTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(team);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        _mockTeamManager.Verify(mgr => mgr.GetMntcTeamWithMembersAsync(10000));
    }

    //------------------------------------//
}//Cls