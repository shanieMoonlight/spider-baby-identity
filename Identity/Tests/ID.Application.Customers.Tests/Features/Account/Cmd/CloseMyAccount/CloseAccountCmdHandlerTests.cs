using ID.Application.Customers.Features.Account.Cmd.CloseAccount;
using ID.Application.Customers.Features.Account.Cmd.CloseMyAccount;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.CloseMyAccount;

public class CloseMyAccountCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly CloseMyAccountCmdHandler _handler;

    //- - - - - - - - - - - - - - - - - - - - -//

    public CloseMyAccountCmdHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new CloseMyAccountCmdHandler(_mockTeamManager.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturUnauthorized_WhenTeamIdsDOntMatch()
    {
        // Arrange
        var team = TeamDataFactory.Create(
           teamType: TeamType.customer);
        var request = new CloseMyAccountCmd(team.Id)
        {
            PrincipalTeam = team,
            IsCustomer = false,
            TeamId = Guid.NewGuid(), // Ensure TeamId matches PrincipalTeam.Id
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotACustomer()
    {
        // Arrange
        var team = TeamDataFactory.Create(
           teamType: TeamType.customer);
        var request = new CloseMyAccountCmd(team.Id)
        {
            PrincipalTeam = team,
            IsCustomer = false,
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotALeader()
    {
        // Arrange
        var team = TeamDataFactory.Create(
           teamType: TeamType.customer);
        var request = new CloseMyAccountCmd(team.Id)
        {
            PrincipalTeam = team,
            IsCustomer = true,
            IsLeader = false,
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_TeamIsDeletedSuccessfully()
    {
        // Arrange
        var team = TeamDataFactory.Create(teamType: TeamType.customer);
        var request = new CloseMyAccountCmd(team.Id)
        {
            IsAuthenticated = true,
            Principal = new System.Security.Claims.ClaimsPrincipal(),
            PrincipalTeam = team,
            IsCustomer = true,
            IsLeader = true,
        };

        _mockTeamManager.Setup(m => m.DeleteTeamAsync(It.IsAny<Team>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Call_DeleteTeamAsyncAsync()
    {
        // Arrange
        var teamMgrMock = new Mock<IIdentityTeamManager<AppUser>>();
        var handler = new CloseMyAccountCmdHandler(teamMgrMock.Object);
        var team = TeamDataFactory.Create(teamType: TeamType.customer);
        var request = new CloseMyAccountCmd(team.Id)
        {

            Principal = new System.Security.Claims.ClaimsPrincipal(),
            PrincipalTeam = team,
            IsCustomer = true,
            IsLeader = true,
        };

        teamMgrMock.Setup(m => m.DeleteTeamAsync(It.IsAny<Team>()))
                   .ReturnsAsync(BasicResult.Success());

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        teamMgrMock.Verify(m => m.DeleteTeamAsync(It.IsAny<Team>()), Times.Once);
    }

    //------------------------------------//


}//Cls