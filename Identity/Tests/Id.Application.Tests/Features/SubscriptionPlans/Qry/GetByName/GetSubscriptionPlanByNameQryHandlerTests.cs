using ID.Application.Features.SubscriptionPlans;
using ID.Application.Features.SubscriptionPlans.Qry.GetByName;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans;
using Moq;

namespace ID.Application.Tests.Features.SubscriptionPlans.Qry.GetByName;

public class GetSubscriptionPlanByNameQryHandlerTests
{
    private readonly Mock<IIdentitySubscriptionPlanService> _mockRepo;
    private readonly GetSubscriptionPlanByNameQryHandler _handler;

    //- - - - - - - - - - - - - - - - - - //

    public GetSubscriptionPlanByNameQryHandlerTests()
    {
        _mockRepo = new Mock<IIdentitySubscriptionPlanService>();
        _handler = new GetSubscriptionPlanByNameQryHandler(_mockRepo.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSubscriptionPlanDto_WhenExists()
    {
        // Arrange
        var subscriptionPlanId = Guid.NewGuid();
        var subscriptionPlanName = "MySubscriptionPlan";
        var subscriptionPlanNameDescription = "MySubscriptionPlan_Description";
        var expectedSubscriptionPlan = SubscriptionPlanDataFactory.Create(
            subscriptionPlanId,
            subscriptionPlanName,
            subscriptionPlanNameDescription);

        _mockRepo.Setup(r => r.FirstByNameAsync(subscriptionPlanName)).ReturnsAsync(expectedSubscriptionPlan);

        // Act
        var result = await _handler.Handle(new GetSubscriptionPlanByNameQry(subscriptionPlanName), CancellationToken.None);

        // Assert
        Assert.IsType<GenResult<SubscriptionPlanDto>>(result);
        Assert.NotNull(result.Value);
        Assert.Equal(subscriptionPlanId, result.Value.Id); // Assuming Id is mapped to Dto
        Assert.Equal(subscriptionPlanName, result.Value.Name);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenSubscriptionPlanDoesNotExist()
    {
        // Arrange
        var subscriptionPlanName = "NonExistentFeature";
        _mockRepo.Setup(r => r.FirstByNameAsync(subscriptionPlanName)).ReturnsAsync((SubscriptionPlan?)null);

        // Act
        var result = await _handler.Handle(new GetSubscriptionPlanByNameQry(subscriptionPlanName), CancellationToken.None);

        // Assert
        Assert.IsType<GenResult<SubscriptionPlanDto>>(result);
        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
        Assert.Equal(IDMsgs.Error.NotFound<SubscriptionPlan>(subscriptionPlanName), result.Info);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var request = new GetSubscriptionPlanByNameQry("");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.IsType<GenResult<SubscriptionPlanDto>>(result);
        Assert.False(result.Succeeded);
        Assert.True(result.NotFound);
    }
}
