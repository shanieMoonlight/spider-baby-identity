using ClArch.SimpleSpecification;
using Shouldly;

namespace ClArch.SimpleSpecification.Tests;

public class ShortCircuitTests
{
    [Fact]
    public void ShouldShortCircuit_WhenConditionIsTrue_ShouldReturnTrue()
    {
        // Arrange
        var spec = new TestEntityByNameSpec(""); // Empty string should trigger short circuit

        // Act & Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    [Fact]
    public void ShouldShortCircuit_WhenConditionIsTrue_WithWhitespace_ShouldReturnTrue()
    {
        // Arrange
        var spec = new TestEntityByNameSpec("   "); // Whitespace should trigger short circuit

        // Act & Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    [Fact]
    public void ShouldShortCircuit_WhenConditionIsTrue_WithNull_ShouldReturnTrue()
    {
        // Arrange
        var spec = new TestEntityByNameSpec(null!); // Null should trigger short circuit

        // Act & Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    [Fact]
    public void ShouldShortCircuit_WhenConditionIsFalse_ShouldReturnFalse()
    {
        // Arrange
        var spec = new TestEntityByNameSpec("ValidName"); // Valid name should not trigger short circuit

        // Act & Assert
        spec.ShouldShortCircuit().ShouldBeFalse();
    }

    [Fact]
    public void ShouldShortCircuit_WhenNotSet_ShouldReturnFalse()
    {
        // Arrange
        var spec = new TestEntityByIdSpec(1); // No short circuit set

        // Act & Assert
        spec.ShouldShortCircuit().ShouldBeFalse();
    }

    [Fact]
    public void ShouldShortCircuit_WithCustomCondition_ShouldWork()
    {
        // Arrange
        var specTrue = new TestEntityWithShortCircuitSpec(true);
        var specFalse = new TestEntityWithShortCircuitSpec(false);

        // Act & Assert
        specTrue.ShouldShortCircuit().ShouldBeTrue();
        specFalse.ShouldShortCircuit().ShouldBeFalse();
    }

    [Fact]
    public void ShortCircuit_ShouldPreventQueryExecution()
    {
        // Arrange
        var testData = new List<TestEntity>
        {
            new() { Id = 1, Name = "Alice", Age = 25, IsActive = true },
            new() { Id = 2, Name = "Bob", Age = 30, IsActive = false }
        };

        var queryable = testData.AsQueryable();
        var spec = new TestEntityByNameSpec(""); // This should short circuit

        // Act
        var shouldShortCircuit = spec.ShouldShortCircuit();

        // Assert
        shouldShortCircuit.ShouldBeTrue();
        
        // In a real repository, this would return empty list without executing query
        // We can't test the actual prevention here, but we can verify the condition
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData(null)]
    public void ShouldShortCircuit_WithVariousInvalidStrings_ShouldReturnTrue(string? invalidName)
    {
        // Arrange
        var spec = new TestEntityByNameSpec(invalidName!);

        // Act & Assert
        spec.ShouldShortCircuit().ShouldBeTrue();
    }

    [Theory]
    [InlineData("A")]
    [InlineData("ValidName")]
    [InlineData("Name with spaces")]
    [InlineData("123")]
    public void ShouldShortCircuit_WithValidStrings_ShouldReturnFalse(string validName)
    {
        // Arrange
        var spec = new TestEntityByNameSpec(validName);

        // Act & Assert
        spec.ShouldShortCircuit().ShouldBeFalse();
    }
}
