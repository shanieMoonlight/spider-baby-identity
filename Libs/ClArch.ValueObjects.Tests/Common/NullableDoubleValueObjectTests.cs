using ClArch.ValueObjects.Common;
using Shouldly;
using System;
using Xunit;

namespace ClArch.ValueObjects.Tests.Common;

public class NullableDoubleValueObjectTests
{
    // Concrete implementation for testing the abstract class
    private class TestNullableDoubleValue : NullableDoubleValueObject
    {
        public TestNullableDoubleValue(double? value) : base(value) { }
    }

    [Fact]
    public void Create_WithValidValue_ShouldSetValue()
    {
        // Arrange & Act
        var doubleValue = new TestNullableDoubleValue(42.5);

        // Assert
        doubleValue.Value.ShouldBe(42.5);
    }

    [Fact]
    public void Create_WithNullValue_ShouldSetValueToNull()
    {
        // Arrange & Act
        var doubleValue = new TestNullableDoubleValue(null);

        // Assert
        doubleValue.Value.ShouldBeNull();
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
        var doubleValue = new TestNullableDoubleValue(value);

        // Assert
        doubleValue.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestNullableDoubleValue(123.45);
        var value2 = new TestNullableDoubleValue(123.45);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestNullableDoubleValue(123.45);
        var value2 = new TestNullableDoubleValue(123.46);

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestNullableDoubleValue(null);
        var value2 = new TestNullableDoubleValue(null);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestNullableDoubleValue(null);
        var value2 = new TestNullableDoubleValue(123.45);

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();

        // Test in reverse order too
        value2.Equals(value1).ShouldBeFalse();
        (value2 == value1).ShouldBeFalse();
        (value2 != value1).ShouldBeTrue();
    }        [Fact]
    public void Equals_NullObject_ShouldBeFalse()
    {
        // Arrange
        var value = new TestNullableDoubleValue(123.45);
        object? nullObject = null;

        // Act & Assert
        value.Equals(nullObject).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ShouldBeFalse()
    {
        // Arrange
        var value = new TestNullableDoubleValue(123.45);
        var differentType = "Not a NullableDoubleValueObject";

        // Act & Assert
        value.Equals(differentType).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestNullableDoubleValue(123.45);
        var value2 = new TestNullableDoubleValue(123.45);

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestNullableDoubleValue(null);
        var value2 = new TestNullableDoubleValue(null);

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ShouldBeDifferent()
    {
        // Arrange
        var value1 = new TestNullableDoubleValue(123.45);
        var value2 = new TestNullableDoubleValue(123.46);

        // Act & Assert
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_OneNull_ShouldBeDifferent()
    {
        // Arrange
        var value1 = new TestNullableDoubleValue(null);
        var value2 = new TestNullableDoubleValue(123.45);

        // Act & Assert
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }
}
