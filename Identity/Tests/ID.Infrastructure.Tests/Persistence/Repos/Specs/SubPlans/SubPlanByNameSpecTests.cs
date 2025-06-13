namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.SubPlans;

public class SubPlanByNameSpecTests
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create(name: "TestPlan");

        // Act
        var spec = new SubPlanByNameSpec(plan.Name);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(plan).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfWrongName()
    {
        // Arrange
        var plan = SubscriptionPlanDataFactory.Create(name: "TestPlan");
        var differentName = "DifferentPlan";

        // Act
        var spec = new SubPlanByNameSpec(differentName);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(plan).ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsShortCircuitCorrectly()
    {
        // Arrange
        string? name = null;

        // Act
        var spec = new SubPlanByNameSpec(name);

        // Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    //------------------------------------//
}
