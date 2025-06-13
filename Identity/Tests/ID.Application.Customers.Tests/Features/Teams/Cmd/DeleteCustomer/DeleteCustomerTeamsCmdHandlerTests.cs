using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.Features.Teams.Cmd.Delete;
using ID.Application.AppAbs.ApplicationServices.Principal;
using ID.Domain.Utility.Messages;


namespace ID.Application.Customers.Tests.Features.Teams.Cmd.DeleteCustomer;


#pragma warning disable CS8600 // Cannot convert null literal to non-nullable reference type.
public class DeleteCustomerTeamCmdHandlerTests
{
    private readonly Mock<IIdPrincipalInfo> _userInfoMock;
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly DeleteCustomerTeamCmdHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public DeleteCustomerTeamCmdHandlerTests()
    {
        _userInfoMock = new Mock<IIdPrincipalInfo>();
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new DeleteCustomerTeamCmdHandler(_mockTeamManager.Object);
    }

    //------------------------------------//


    [Theory]
    [InlineData(TeamType.Maintenance)]
    [InlineData(TeamType.Super)]
    public async Task Handle_TeamIsNotCustomer_ReturnsBadRequestResult(TeamType teamType)
    {
        // Arrange
        var command = new DeleteCustomerTeamCmd(Guid.NewGuid());
        var team = TeamDataFactory.Create(teamType: teamType);
        _mockTeamManager.Setup(m => m.GetByIdWithMembersAsync(command.Id, 1000)).ReturnsAsync(team);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.Teams.CAN_ONLY_REMOVE_CUSTOMER_TEAM);
    }

    //------------------------------------//


    [Fact]
    public async Task Handle_TeamNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var command = new DeleteCustomerTeamCmd(Guid.NewGuid());
        _mockTeamManager.Setup(m => m.GetByIdWithMembersAsync(command.Id, 1000)).ReturnsAsync((Team)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<Team>(command.Id));
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ValidRequest_DeletesTeam()
    {
        // Arrange
        var command = new DeleteCustomerTeamCmd(Guid.NewGuid());
        var team = TeamDataFactory.Create(teamType: TeamType.Customer);
        _mockTeamManager.Setup(m => m.GetByIdWithMembersAsync(command.Id, 1000)).ReturnsAsync(team);
        _mockTeamManager.Setup(m => m.DeleteTeamAsync(team)).ReturnsAsync(BasicResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeTrue();
        _mockTeamManager.Verify(m => m.DeleteTeamAsync(team), Times.Once);
    }


    //------------------------------------//

}//Cls
