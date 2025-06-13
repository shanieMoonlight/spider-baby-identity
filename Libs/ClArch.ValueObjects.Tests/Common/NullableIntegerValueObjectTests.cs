using ClArch.ValueObjects.Common;
using Shouldly;
using System;
using Xunit;

namespace ClArch.ValueObjects.Tests.Common;

public class NullableIntegerValueObjectTests
{
    // Concrete implementation for testing the abstract class
    private class TestNullableIntegerValue : NullableIntegerValueObject
    {
        public TestNullableIntegerValue(int? value) : base(value) { }
    }

    [Fact]
    public void Create_WithValidValue_ShouldSetValue()
    {
        // Arrange & Act
        var intValue = new TestNullableIntegerValue(42);

        // Assert
        intValue.Value.ShouldBe(42);
    }

    [Fact]
    public void Create_WithNullValue_ShouldSetValueToNull()
    {
        // Arrange & Act
        var intValue = new TestNullableIntegerValue(null);

        // Assert
        intValue.Value.ShouldBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Create_WithVariousValues_ShouldAcceptAll(int value)
    {
        // Arrange & Act
        var intValue = new TestNullableIntegerValue(value);

        // Assert
        intValue.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestNullableIntegerValue(123);
        var value2 = new TestNullableIntegerValue(123);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestNullableIntegerValue(123);
        var value2 = new TestNullableIntegerValue(456);

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestNullableIntegerValue(null);
        var value2 = new TestNullableIntegerValue(null);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestNullableIntegerValue(null);
        var value2 = new TestNullableIntegerValue(123);

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();

        // Test in reverse order too
        value2.Equals(value1).ShouldBeFalse();
        (value2 == value1).ShouldBeFalse();
        (value2 != value1).ShouldBeTrue();
    }

    [Fact]
    public void Equals_NullObject_ShouldBeFalse()
    {
        // Arrange
        var value = new TestNullableIntegerValue(123);
        object? nullObject = null;

        // Act & Assert
        value.Equals(nullObject).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ShouldBeFalse()
    {
        // Arrange
        var value = new TestNullableIntegerValue(123);
        var differentType = "Not a NullableIntegerValueObject";

        // Act & Assert
        value.Equals(differentType).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestNullableIntegerValue(123);
        var value2 = new TestNullableIntegerValue(123);

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestNullableIntegerValue(null);
        var value2 = new TestNullableIntegerValue(null);

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ShouldBeDifferent()
    {
        // Arrange
        var value1 = new TestNullableIntegerValue(123);
        var value2 = new TestNullableIntegerValue(456);

        // Act & Assert
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_OneNull_ShouldBeDifferent()
    {
        // Arrange
        var value1 = new TestNullableIntegerValue(null);
        var value2 = new TestNullableIntegerValue(123);

        // Act & Assert
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnStringRepresentation()
    {
        // Arrange
        var value = new TestNullableIntegerValue(123);

        // Act & Assert
        value.ToString().ShouldBe("123");
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var value = new TestNullableIntegerValue(null);

        // Act & Assert
        value.ToString().ShouldBe(string.Empty);
    }
}
