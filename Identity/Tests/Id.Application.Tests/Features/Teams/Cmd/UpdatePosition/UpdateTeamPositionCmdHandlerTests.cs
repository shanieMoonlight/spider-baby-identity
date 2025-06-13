using ID.Application.Features.Teams.Cmd.UpdatePositionRange;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;


namespace ID.Application.Tests.Features.Teams.Cmd.UpdatePosition;
public class UpdateTeamPositionCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamManagerMock;
    private readonly UpdateTeamPositionRangeCmdHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public UpdateTeamPositionCmdHandlerTests()
    {
        _teamManagerMock = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new UpdateTeamPositionRangeCmdHandler(_teamManagerMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Update_Team_Position_Range()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var dto = new UpdateTeamPositionRangeDto(1, 10);
        var request = new UpdateTeamPositionRangeCmd(dto) { PrincipalTeam = team };
        var cancellationToken = CancellationToken.None;

        _teamManagerMock.Setup(m => m.UpdateAsync(It.IsAny<Team>()))
            .ReturnsAsync(team);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.MinPosition.ShouldBe(dto.MinPosition);
        result.Value.MaxPosition.ShouldBe(dto.MaxPosition);

        _teamManagerMock.Verify(m => m.UpdateAsync(It.Is<Team>(t => t.MinPosition == dto.MinPosition && t.MaxPosition == dto.MaxPosition)), Times.Once);
    }

    //------------------------------------//

}//Cls
