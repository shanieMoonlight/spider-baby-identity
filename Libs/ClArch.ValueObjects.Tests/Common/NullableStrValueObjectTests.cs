using ClArch.ValueObjects.Common;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests.Common;


#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
public class NullableStrValueObjectTests
{
    // Concrete implementation for testing the abstract class
    private class TestNullableStringValue(string? value) : NullableStringInvariantValueObject(value)
    {
    }

    [Fact]
    public void Create_WithValidValue_ShouldSetValue()
    {
        // Arrange & Act
        var stringValue = new TestNullableStringValue("Test");

        // Assert
        stringValue.Value.ShouldBe("Test");
    }

    [Fact]
    public void Create_WithNullValue_ShouldSetValueToNull()
    {
        // Arrange & Act
        var stringValue = new TestNullableStringValue(null);

        // Assert
        stringValue.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyString_ShouldSetEmptyString()
    {
        // Arrange & Act
        var stringValue = new TestNullableStringValue("");

        // Assert
        stringValue.Value.ShouldBe("");
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange & Act
        var stringValue = new TestNullableStringValue("  Test  ");

        // Assert
        stringValue.Value.ShouldBe("Test");
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("Hello World")]
    [InlineData("Special!@#$%^&*()")]
    public void Create_WithVariousValues_ShouldAcceptAll(string value)
    {
        // Arrange & Act
        var stringValue = new TestNullableStringValue(value);

        // Assert
        stringValue.Value.ShouldBe(value.Trim());
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestNullableStringValue("Test");
        var value2 = new TestNullableStringValue("Test");

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestNullableStringValue("Test1");
        var value2 = new TestNullableStringValue("Test2");

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestNullableStringValue("Test");
        var value2 = new TestNullableStringValue("TEST");

        // Act & Assert
        // This tests the ValuesAreEqual method which uses case-insensitive comparison (ToLower())
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestNullableStringValue(null);
        var value2 = new TestNullableStringValue(null);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestNullableStringValue(null);
        var value2 = new TestNullableStringValue("Test");

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_NullObject_ShouldBeFalse()
    {
        // Arrange
        var value = new TestNullableStringValue("Test");

        // Act & Assert
        value.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ShouldBeFalse()
    {
        // Arrange
        var value = new TestNullableStringValue("Test");
        var differentType = 123;

        // Act & Assert
        value.Equals(differentType).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestNullableStringValue("Test");
        var value2 = new TestNullableStringValue("Test");

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_NullValues_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestNullableStringValue(null);
        var value2 = new TestNullableStringValue(null);

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
        value1.GetHashCode().ShouldBe(0); // The base ValueObject returns 0 for null value's hash code
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValue()
    {
        // Arrange
        var value = new TestNullableStringValue("Test");

        // Act & Assert
        value.ToString().ShouldBe("Test");
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var value = new TestNullableStringValue(null);

        // Act & Assert
        value.ToString().ShouldBe(string.Empty);
    }
}
