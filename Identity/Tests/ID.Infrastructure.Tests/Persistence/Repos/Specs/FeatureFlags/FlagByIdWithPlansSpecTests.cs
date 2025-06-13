namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.FeatureFlags;

public class FlagByIdWithPlansSpecTests
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var flag = FeatureFlagDataFactory.Create();

        // Act
        var spec = new FlagByIdWithPlansSpec(flag.Id);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(flag).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfWrongId()
    {
        // Arrange
        var flag = FeatureFlagDataFactory.Create();
        var differentId = Guid.NewGuid();

        // Act
        var spec = new FlagByIdWithPlansSpec(differentId);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(flag).ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsShortCircuitCorrectly()
    {
        // Arrange
        Guid? id = null;

        // Act
        var spec = new FlagByIdWithPlansSpec(id);

        // Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    //------------------------------------//

}
