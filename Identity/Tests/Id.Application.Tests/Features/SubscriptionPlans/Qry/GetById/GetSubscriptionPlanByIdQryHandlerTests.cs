using ID.Application.Features.SubscriptionPlans;
using ID.Application.Features.SubscriptionPlans.Qry.GetById;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans;
using Moq;
using MediatR;

namespace ID.Application.Tests.Features.SubscriptionPlans.Qry.GetById;

public class GetSubscriptionPlanByIdQryHandlerTests
{
    private readonly Mock<IIdentitySubscriptionPlanService> _mockRepo;
    private readonly Mock<IMediator> _mockMediator;
    private readonly GetSubscriptionPlanByIdQryHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public GetSubscriptionPlanByIdQryHandlerTests()
    {
        _mockRepo = new Mock<IIdentitySubscriptionPlanService>();
        _mockMediator = new Mock<IMediator>();
        _handler = new GetSubscriptionPlanByIdQryHandler(_mockRepo.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSubscriptionPlanDto_WhenExists()
    {
        // Arrange
        var subscriptionPlanId = Guid.NewGuid();
        var expectedSubscriptionPlan = SubscriptionPlanDataFactory.Create(subscriptionPlanId);
        _mockRepo.Setup(r => r.GetByIdWithFeatureFlagsAsync(subscriptionPlanId)).ReturnsAsync(expectedSubscriptionPlan);
        var handler = new GetSubscriptionPlanByIdQryHandler(_mockRepo.Object);

        // Act
        var result = await handler.Handle(new GetSubscriptionPlanByIdQry(subscriptionPlanId), CancellationToken.None);

        // Assert
        Assert.IsType<GenResult<SubscriptionPlanDto>>(result);
        Assert.NotNull(result.Value);
        Assert.Equal(subscriptionPlanId, result.Value.Id); // Assuming Id is mapped to Dto
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenSubscriptionPlanDoesNotExist()
    {
        // Arrange
        var expectedSubscriptionPlan = SubscriptionPlanDataFactory.Create();
        var subscriptionPlanId = expectedSubscriptionPlan.Id;
        _mockRepo.Setup(r => r.GetByIdWithFeatureFlagsAsync(subscriptionPlanId)).ReturnsAsync((SubscriptionPlan?)null);
        var handler = new GetSubscriptionPlanByIdQryHandler(_mockRepo.Object);


        // Act
        var result = await handler.Handle(new GetSubscriptionPlanByIdQry(subscriptionPlanId), CancellationToken.None);

        // Assert
        Assert.IsType<GenResult<SubscriptionPlanDto>>(result);
        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
        Assert.Equal(IDMsgs.Error.NotFound<SubscriptionPlan>(subscriptionPlanId), result.Info);
    }

    //------------------------------------//

}//Cls