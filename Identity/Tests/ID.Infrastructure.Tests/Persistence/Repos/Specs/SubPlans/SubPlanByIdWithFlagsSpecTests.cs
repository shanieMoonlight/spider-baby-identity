namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.SubPlans;

public class SubPlanByIdWithFlagsSpecTests
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create();

        // Act
        var spec = new SubPlanByIdWithFlagsSpec(plan.Id);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(plan).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfWrongId()
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create();
        var differentId = Guid.NewGuid();

        // Act
        var spec = new SubPlanByIdWithFlagsSpec(differentId);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(plan).ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsShortCircuitCorrectly()
    {
        // Arrange
        Guid? id = null;

        // Act
        var spec = new SubPlanByIdWithFlagsSpec(id);

        // Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    //------------------------------------//
}
