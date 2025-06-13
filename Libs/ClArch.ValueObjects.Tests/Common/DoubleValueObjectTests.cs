using ClArch.ValueObjects.Common;
using Shouldly;

namespace ClArch.ValueObjects.Tests.Common;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
public class DoubleValueObjectTests
{
    // Concrete implementation for testing the abstract class
    private class TestDoubleValue(double value) : DoubleValueObject(value)
    {
    }

    [Fact]
    public void Create_WithValidValue_ShouldSetValue()
    {
        // Arrange & Act
        var doubleValue = new TestDoubleValue(42.5);

        // Assert
        doubleValue.Value.ShouldBe(42.5);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(-1.0)]
    [InlineData(double.MaxValue)]
    [InlineData(double.MinValue)]
    [InlineData(double.Epsilon)]
    public void Create_WithVariousValues_ShouldAcceptAll(double value)
    {
        // Arrange & Act
        var doubleValue = new TestDoubleValue(value);

        // Assert
        doubleValue.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestDoubleValue(123.45);
        var value2 = new TestDoubleValue(123.45);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestDoubleValue(123.45);
        var value2 = new TestDoubleValue(123.46);

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_NullObject_ShouldBeFalse()
    {
        // Arrange
        var value = new TestDoubleValue(123.45);

        // Act & Assert
        value.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ShouldBeFalse()
    {
        // Arrange
        var value = new TestDoubleValue(123.45);
        var differentType = "Not a DoubleValueObject";

        // Act & Assert
        value.Equals(differentType).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestDoubleValue(123.45);
        var value2 = new TestDoubleValue(123.45);

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ShouldBeDifferent()
    {
        // Arrange
        var value1 = new TestDoubleValue(123.45);
        var value2 = new TestDoubleValue(123.46);

        // Act & Assert
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }

    [Theory]
    [InlineData(1.0, 1.0000000000001)] // Very close but different values
    public void Equals_VeryCloseValues_ShouldBehaveCorrectly(double value1, double value2)
    {
        // Arrange
        var doubleValue1 = new TestDoubleValue(value1);
        var doubleValue2 = new TestDoubleValue(value2);

        // Act & Assert - With standard equality, these should be different
        // If you wanted epsilon-based comparison, this would need to change
        doubleValue1.Equals(doubleValue2).ShouldBeFalse();
    }
}
