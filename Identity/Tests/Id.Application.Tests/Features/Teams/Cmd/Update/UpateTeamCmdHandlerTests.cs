using ID.Application.Features.Teams;
using ID.Tests.Data.Factories;
using Moq;
using Shouldly;
using ID.Application.Features.Teams.Cmd.Update;
using ID.Application.AppAbs.ApplicationServices.Principal;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;


namespace ID.Application.Tests.Features.Teams.Cmd.Update;
public class UpateTeamCmdHandlerTests
{
    private readonly Mock<IIdPrincipalInfo> _userInfoMock;
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamMgrMock;
    private readonly UpdateTeamCmdHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public UpateTeamCmdHandlerTests()
    {
        _userInfoMock = new Mock<IIdPrincipalInfo>();
        _teamMgrMock = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new UpdateTeamCmdHandler(_teamMgrMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task OnAuthenticated_ValidRequest_ReturnsTeamDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userInfoMock.Setup(u => u.IsMntcMinimum()).Returns(true);

        var dtoName = "Test Team";
        var dtoDescription = "This is a test team.";
        var dto = new TeamDto { Name = dtoName, Description = dtoDescription };
        var expectedTeamId = Guid.NewGuid();
        var expectedTeam = TeamDataFactory.Create(expectedTeamId, dtoName, dtoDescription);
        var request = new UpdateTeamCmd(dto)
        {
            PrincipalTeam = expectedTeam
        };

        _teamMgrMock.Setup(tm => tm.UpdateAsync(It.IsAny<Team>()))
            .ReturnsAsync(expectedTeam);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);


        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value?.Id.ShouldBe(expectedTeamId);
        result.Value?.Name.ShouldBe(dtoName);
        result.Value?.Description.ShouldBe(dtoDescription);

        _teamMgrMock.Verify(tm => tm.UpdateAsync(It.IsAny<Team>()), Times.Once);
    }

    //------------------------------------//

}//Cls
