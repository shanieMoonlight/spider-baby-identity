using ID.Application.Customers.Features.Account.Cmd.CloseAccount;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.CloseAccount;

public class CloseAccountCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly CloseAccountCmdHandler _handler;

    //- - - - - - - - - - - - - - - - - - - - -//

    public CloseAccountCmdHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new CloseAccountCmdHandler(_mockTeamManager.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenNotMntcMinimum_1()
    {
        // Arrange
        var team = TeamDataFactory.Create(
           teamType: TeamType.customer
        );
        var request = new CloseAccountCmd(team.Id)
        {
            PrincipalTeam = team,
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
    public async Task Handle_ShouldReturnUnauthorized_WhenNotMntcMinimum_2()
    {
        // Arrange
        var request = new CloseAccountCmd(Guid.NewGuid())
        {
            IsCustomer = true,
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
    public async Task Handle_ShouldReturnNotFoundWhenTeamNotFound()
    {
        // Arrange
        var team = TeamDataFactory.Create(
           teamType: TeamType.maintenance
        );
        var request = new CloseAccountCmd(Guid.NewGuid())
        {
            PrincipalTeam = team,
            IsCustomer = false
        };

        _mockTeamManager.Setup(m => m.GetByIdWithEverythingAsync(request.TeamId, 10000))
            .ReturnsAsync((Team)null!); // Simulate team not found   

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_TeamIsDeletedSuccessfully()
    {
        // Arrange
        var team = TeamDataFactory.Create(
             teamType: TeamType.maintenance
          );
        var request = new CloseAccountCmd(Guid.NewGuid())
        {
            PrincipalTeam = team,
            IsCustomer = false
        };

        _mockTeamManager.Setup(m => m.GetByIdWithEverythingAsync(request.TeamId, It.IsAny<int>()))
            .ReturnsAsync(team); 

        _mockTeamManager.Setup(m => m.DeleteTeamAsync(team))
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
        // Arrange
        var team = TeamDataFactory.Create(
             teamType: TeamType.maintenance
          );
        var request = new CloseAccountCmd(Guid.NewGuid())
        {
            PrincipalTeam = team,
            IsCustomer = false
        };

        _mockTeamManager.Setup(m => m.GetByIdWithEverythingAsync(request.TeamId, It.IsAny<int>()))
            .ReturnsAsync(team);

        _mockTeamManager.Setup(m => m.DeleteTeamAsync(team))
            .ReturnsAsync(BasicResult.Success());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockTeamManager.Verify(m => m.DeleteTeamAsync(It.IsAny<Team>()), Times.Once);
    }

    //------------------------------------//


}//Cls