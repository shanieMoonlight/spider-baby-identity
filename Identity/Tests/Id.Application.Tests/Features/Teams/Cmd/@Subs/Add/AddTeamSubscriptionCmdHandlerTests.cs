using ID.Application.Features.Teams.Cmd.Subs.AddSubscription;
using ID.Domain.Abstractions.Services.Teams.Subs;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Cmd.@Subs.Add;

public class AddTeamSubscriptionCmdHandlerTests
{
    private readonly Mock<ITeamSubscriptionServiceFactory> _mockSubsServiceFactory;
    private readonly AddTeamSubscriptionCmdHandler _handler;

    public AddTeamSubscriptionCmdHandlerTests()
    {
        _mockSubsServiceFactory = new Mock<ITeamSubscriptionServiceFactory>();
        _handler = new AddTeamSubscriptionCmdHandler(_mockSubsServiceFactory.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenGetServiceFails()
    {
        // Arrange
        var dto = new AddTeamSubscriptionDto (Guid.NewGuid(), Guid.NewGuid(), 10 );
        var request = new AddTeamSubscriptionCmd(dto);
        var failedServiceResult = GenResult<ITeamSubscriptionService>.Failure("Service not found");
        _mockSubsServiceFactory.Setup(x => x.GetServiceAsync(It.IsAny<Guid?>())).ReturnsAsync(failedServiceResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe("Service not found");
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenAddSubscriptionSucceeds()
    {
        // Arrange

        var teamId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var subscription = SubscriptionDataFactory.Create(subscriptionId, null, null, null, null, null, 0);
        var team = TeamDataFactory.Create(teamId, null, null, [subscription]);
        
        var dto = new AddTeamSubscriptionDto(teamId, subscription.Id, default);
        var request = new AddTeamSubscriptionCmd(dto);
        
        var serviceMock = new Mock<ITeamSubscriptionService>();
            serviceMock.Setup(x => x.Team).Returns(team);
        
        var successfulServiceResult = GenResult<ITeamSubscriptionService>.Success(serviceMock.Object);
        var successfulAddResult = GenResult<TeamSubscription>.Success(subscription);
     
        _mockSubsServiceFactory.Setup(x => x.GetServiceAsync(It.IsAny<Guid?>())).ReturnsAsync(successfulServiceResult);
        serviceMock.Setup(x => x.AddSubscriptionAsync(It.IsAny<Guid?>(), It.IsAny<Discount?>())).ReturnsAsync(successfulAddResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }

    //------------------------------------//

}//Cls