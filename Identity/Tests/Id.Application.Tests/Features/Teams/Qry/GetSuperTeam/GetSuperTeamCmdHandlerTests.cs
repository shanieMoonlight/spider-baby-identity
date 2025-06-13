using ID.Tests.Data.Factories;
using Moq;
using Shouldly;
using ID.Application.Features.Teams.Qry.GetSuperTeam;
using ID.Application.AppAbs.ApplicationServices.Principal;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Constants;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;


namespace ID.Application.Tests.Features.Teams.Qry.GetSuperTeam;
public class GetSuperTeamQryHandlerTests
{
    private readonly Mock<IIdPrincipalInfo> _userInfoMock;
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamMgrMock;
    private readonly GetSuperTeamQryHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public GetSuperTeamQryHandlerTests()
    {
        _userInfoMock = new Mock<IIdPrincipalInfo>();
        _teamMgrMock = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new GetSuperTeamQryHandler();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenSuperTeamDoesNotExist()
    {
        // Arrange
        var request = new GetSuperTeamQry { 
            PrincipalTeamPosition = 1 ,
        };
        //_teamMgrMock.Setup(mgr => mgr.GetSuperTeamWithMembersAsync(It.IsAny<int>()))
        //    .ReturnsAsync((Team)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.NotFound.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<Team>(IdGlobalConstants.Teams.SUPER_TEAM_NAME));
    }


    //------------------------------------//

    [Fact]
    public async Task OnAuthenticated_ValidRequest_ReturnsTeamDto()
    {
        // Arrange
        var name = "Test Super Team";
        var description = "This is a test team.";
        var teamId = Guid.NewGuid();
        var superTeam = TeamDataFactory.Create(teamId, name, description);
        var request = new GetSuperTeamQry()
        {
            PrincipalTeamPosition = 666,
            PrincipalTeam = superTeam
        };



        // Act
        var result = await _handler.Handle(request, CancellationToken.None);


        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value?.Id.ShouldBe(teamId);
        result.Value?.Name.ShouldBe(name);
        result.Value?.Description.ShouldBe(description);

    }

    //------------------------------------//

}//Cls
