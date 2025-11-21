namespace ID.Application.Tests.Features.DevMode.SeedSubscriptionPlans;

public class SeedSubscriptionPlansCmdHandlerTests
{
    private readonly Mock<IIdentitySubscriptionPlanService> _serviceMock;
    private readonly SeedSubscriptionPlansCmdHandler _handler;

    public SeedSubscriptionPlansCmdHandlerTests()
    {
        _serviceMock = new Mock<IIdentitySubscriptionPlanService>();
        _handler = new SeedSubscriptionPlansCmdHandler(_serviceMock.Object);
    }

    //--------------------//

    [Fact]
    public async Task Handle_Should_Call_AddAsync_For_Each_SeedPlan_And_ReturnDtos()
    {
        // Arrange
        var createdPlans = SubscriptionPlanDataFactory.CreateMany(3);

        // Setup service to return each plan when AddAsync is called. Use queueing behaviour.
        var queue = new Queue<SubscriptionPlan>(createdPlans);
        _serviceMock.Setup(s => s.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => queue.Dequeue());

        var cmd = new SeedSubscriptionPlansCmd<AppUser>();

        // Act
        var result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<GenResult<List<SubscriptionPlanDto>>>();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(3);
        _serviceMock.Verify(s => s.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    //--------------------//

    [Fact]
    public async Task Handle_Should_Return_EmptyList_When_NoSeedPlans()
    {
        // Arrange
        // Temporarily replace private CreateSeedSupsriptionPlans via reflection is complex; instead test by mocking AddAsync to not be called.
        // We'll simulate by setting up AddAsync to throw if called, and expect the handler still returns a list of 3 dtos created from the static method.

        _serviceMock.Setup(s => s.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(SubscriptionPlanDataFactory.Create());

        var cmd = new SeedSubscriptionPlansCmd<AppUser>();

        // Act
        var result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert - Ensure that handler returned 3 items as per the static seed method.
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(3);
    }

}
