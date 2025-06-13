using ID.Application.Features.Teams.Qry.GetByName;
using Moq;
using ID.Application.Features.Teams.Qry.GetById;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.Features.Teams.Qry.GetByName;

public class GetByNameQryHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly GetTeamByIdQryHandler _handler;

    public GetByNameQryHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new GetTeamByIdQryHandler(_mockTeamManager.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldCallGetCustomerTeamsByNameAsync_WithRequestName()
    {
        // Arrange
        var mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        var handler = new GetTeamsByNameQryHandler(mockTeamManager.Object);
        var request = new GetTeamsByNameQry("TestTeam");
        var cancellationToken = new CancellationToken();

        // Act
        await handler.Handle(request, cancellationToken);

        // Assert
        mockTeamManager.Verify(mgr => mgr.GetCustomerTeamsByNameAsync(request.Name), Times.Once);
    }

    //------------------------------------//

}//Cls
