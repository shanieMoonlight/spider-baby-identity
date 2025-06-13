using ID.Application.Features.Teams.Cmd.Subs.RecordSubscriptionPayment;
using ID.Domain.Abstractions.Services.Teams.Subs;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Cmd.@Subs.Add;

public class RecordSubscriptionPaymentCmdHandlerTests
{
    private readonly Mock<ITeamSubscriptionServiceFactory> _mockSubsServiceFactory;
    private readonly RecordSubscriptionPaymentCmdHandler _handler;

    public RecordSubscriptionPaymentCmdHandlerTests()
    {
        _mockSubsServiceFactory = new Mock<ITeamSubscriptionServiceFactory>();
        _handler = new RecordSubscriptionPaymentCmdHandler(_mockSubsServiceFactory.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenGetServiceFails()
    {
        // Arrange
        var dto = new RecordSubscriptionPaymentDto(Guid.NewGuid(), Guid.NewGuid());
        var request = new RecordSubscriptionPaymentCmd(dto);
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

        var dto = new RecordSubscriptionPaymentDto(teamId, subscription.Id);
        var request = new RecordSubscriptionPaymentCmd(dto);

        var serviceMock = new Mock<ITeamSubscriptionService>();
        serviceMock.Setup(x => x.Team).Returns(team);

        var successfulServiceResult = GenResult<ITeamSubscriptionService>.Success(serviceMock.Object);
        var successfulRemoveResult = GenResult<bool>.Success(true);

        _mockSubsServiceFactory.Setup(x => x.GetServiceAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(successfulServiceResult);
        serviceMock.Setup(x => x.RecordPaymentAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(GenResult<TeamSubscription>.Success(subscription));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(subscription.Id);
    }

    //------------------------------------//

}//Cls