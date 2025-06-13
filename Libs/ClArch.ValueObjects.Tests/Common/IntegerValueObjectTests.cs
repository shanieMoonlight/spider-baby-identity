using ClArch.ValueObjects.Common;
using Shouldly;
using System;
using Xunit;

namespace ClArch.ValueObjects.Tests.Common;

public class IntegerValueObjectTests
{
    // Concrete implementation for testing the abstract class
    private class TestIntegerValue : IntegerValueObject
    {
        public TestIntegerValue(int value) : base(value) { }
    }

    [Fact]
    public void Create_WithValidValue_ShouldSetValue()
    {
        // Arrange & Act
        var intValue = new TestIntegerValue(42);

        // Assert
        intValue.Value.ShouldBe(42);
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
        var intValue = new TestIntegerValue(value);

        // Assert
        intValue.Value.ShouldBe(value);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestIntegerValue(123);
        var value2 = new TestIntegerValue(123);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestIntegerValue(123);
        var value2 = new TestIntegerValue(456);

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_NullObject_ShouldBeFalse()
    {
        // Arrange
        var value = new TestIntegerValue(123);
        object? nullObject = null;

        // Act & Assert
        value.Equals(nullObject).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ShouldBeFalse()
    {
        // Arrange
        var value = new TestIntegerValue(123);
        var differentType = "Not an IntegerValueObject";

        // Act & Assert
        value.Equals(differentType).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestIntegerValue(123);
        var value2 = new TestIntegerValue(123);

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ShouldBeDifferent()
    {
        // Arrange
        var value1 = new TestIntegerValue(123);
        var value2 = new TestIntegerValue(456);

        // Act & Assert
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        // Arrange
        var value = new TestIntegerValue(123);

        // Act & Assert
        value.ToString().ShouldBe("123");
    }
}
