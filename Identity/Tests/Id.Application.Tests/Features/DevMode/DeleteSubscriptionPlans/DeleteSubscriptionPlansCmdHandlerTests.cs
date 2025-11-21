using ID.Application.Features.DevMode.DeleteSubscriptionPlans;

namespace ID.Application.Tests.Features.DevMode.DeleteSubscriptionPlans;

public class DeleteSubscriptionPlansCmdHandlerTests
{
    private readonly Mock<IIdentitySubscriptionPlanService> _serviceMock;
    private readonly DeleteSubscriptionPlansCmdHandler _handler;

    public DeleteSubscriptionPlansCmdHandlerTests()
    {
        _serviceMock = new Mock<IIdentitySubscriptionPlanService>();
        _handler = new DeleteSubscriptionPlansCmdHandler(_serviceMock.Object);
    }

    //--------------------//

    [Fact]
    public async Task Handle_Should_Call_ListAllAsync_And_DeleteAsync_For_Each_Plan()
    {
        // Arrange
        var plans = SubscriptionPlanDataFactory.CreateMany(3);
        _serviceMock.Setup(s => s.ListAllAsync()).ReturnsAsync(plans);
        _serviceMock.Setup(s => s.DeleteAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        var cmd = new DeleteSubscriptionPlansCmd<AppUser>();

        // Act
        var result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<List<SubscriptionPlanDto>>>();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(plans.Count);
        _serviceMock.Verify(s => s.ListAllAsync(), Times.Once);
        _serviceMock.Verify(s => s.DeleteAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()), Times.Exactly(plans.Count));
    }

    //--------------------//

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Plans()
    {
        // Arrange
        var plans = new List<SubscriptionPlan>();
        _serviceMock.Setup(s => s.ListAllAsync()).ReturnsAsync(plans);

        var cmd = new DeleteSubscriptionPlansCmd<AppUser>();

        // Act
        var result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<List<SubscriptionPlanDto>>>();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
        _serviceMock.Verify(s => s.ListAllAsync(), Times.Once);
        _serviceMock.Verify(s => s.DeleteAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()), Times.Never);
    }

}
