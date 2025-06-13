namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.FeatureFlags;

public class FlagByNameSpecTests
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var flag = FeatureFlagDataFactory.Create(name: "TestFlag");

        // Act
        var spec = new FlagByNameSpec(flag.Name);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(flag).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfWrongName()
    {
        // Arrange
        var flag = FeatureFlagDataFactory.Create(name: "TestFlag");
        var differentName = "DifferentFlag";

        // Act
        var spec = new FlagByNameSpec(differentName);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(flag).ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsShortCircuitCorrectly()
    {
        // Arrange
        string? name = null;

        // Act
        var spec = new FlagByNameSpec(name);

        // Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    //------------------------------------//
}
