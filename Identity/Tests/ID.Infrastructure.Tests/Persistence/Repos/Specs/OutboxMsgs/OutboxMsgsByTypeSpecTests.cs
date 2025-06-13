namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.Outbox;

public class OutboxMsgsByTypeSpecTests
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var msg = OutboxMessageDataFactory.Create(type: "TestType");

        // Act
        var spec = new OutboxMsgsByTypeSpec(msg.Type);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(msg).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfWrongType()
    {
        // Arrange
        var msg = OutboxMessageDataFactory.Create(type: "TestType");
        var differentType = "DifferentType";

        // Act
        var spec = new OutboxMsgsByTypeSpec(differentType);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(msg).ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsShortCircuitCorrectly()
    {
        // Arrange
        string? type = null;

        // Act
        var spec = new OutboxMsgsByTypeSpec(type);

        // Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    //------------------------------------//
}
