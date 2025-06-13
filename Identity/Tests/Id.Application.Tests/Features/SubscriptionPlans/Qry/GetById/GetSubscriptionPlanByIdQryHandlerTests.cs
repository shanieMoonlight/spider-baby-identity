using ID.Application.Features.SubscriptionPlans;
using ID.Application.Features.SubscriptionPlans.Qry.GetById;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using MassTransit;
using MediatR;
using MyResults;
using NSubstitute;

namespace ID.Application.Tests.Features.SubscriptionPlans.Qry.GetById;

public class GetSubscriptionPlanByIdQryHandlerTests
{
    private readonly IIdentitySubscriptionPlanService _mockRepo;
    private readonly IMediator _mockMediator;
    private readonly GetSubscriptionPlanByIdQryHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public GetSubscriptionPlanByIdQryHandlerTests()
    {
        _mockRepo = Substitute.For<IIdentitySubscriptionPlanService>();
        _mockMediator = Substitute.For<IMediator>();
        _handler = new GetSubscriptionPlanByIdQryHandler(_mockRepo);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSubscriptionPlanDto_WhenExists()
    {
        // Arrange
        var subscriptionPlanId = NewId.NextSequentialGuid();
        var expectedSubscriptionPlan = SubscriptionPlanDataFactory.Create(subscriptionPlanId);
        _mockRepo.GetByIdWithFeatureFlagsAsync(subscriptionPlanId).Returns(expectedSubscriptionPlan);
        var handler = new GetSubscriptionPlanByIdQryHandler(_mockRepo);

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
        _mockRepo.GetByIdWithFeatureFlagsAsync(subscriptionPlanId).Returns((SubscriptionPlan?)null);
        var handler = new GetSubscriptionPlanByIdQryHandler(_mockRepo);


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