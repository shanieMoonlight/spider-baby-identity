using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.GlobalSettings.Utility;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.Customers.Features.Account.Cmd.CloseAccount;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.CloseAccount;

public class CloseAccountCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly CloseMyAccountCmdHandler _handler;

    //- - - - - - - - - - - - - - - - - - - - -//

    public CloseAccountCmdHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new CloseMyAccountCmdHandler(_mockTeamManager.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserIsNotACustomer()
    {
        // Arrange
        var team = TeamDataFactory.Create(
           teamType: TeamType.Customer);
        var request = new CloseMyAccountCmd()
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
           teamType: TeamType.Customer);
        var request = new CloseMyAccountCmd()
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
        var request = new CloseMyAccountCmd()
        {
            IsAuthenticated = true,
            Principal = new System.Security.Claims.ClaimsPrincipal(),
            PrincipalTeam = TeamDataFactory.Create(teamType: TeamType.Customer),
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
        var request = new CloseMyAccountCmd()
        {

            Principal = new System.Security.Claims.ClaimsPrincipal(),
            PrincipalTeam = TeamDataFactory.Create(teamType: TeamType.Customer),
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