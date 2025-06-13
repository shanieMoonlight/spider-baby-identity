using ClArch.ValueObjects.Common;
using Shouldly;

namespace ClArch.ValueObjects.Tests.Common;


#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
public class StrValueObjectTests
{
    // Concrete implementation for testing the abstract class
    private class TestStringValue(string value) : StringInvariantValueObject(value)
    {
    }

    [Fact]
    public void Create_WithValidValue_ShouldSetValue()
    {
        // Arrange & Act
        var stringValue = new TestStringValue("Test");

        // Assert
        stringValue.Value.ShouldBe("Test");
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange & Act
        var stringValue = new TestStringValue("  Test  ");

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
        var stringValue = new TestStringValue(value);

        // Assert
        stringValue.Value.ShouldBe(value.Trim());
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestStringValue("Test");
        var value2 = new TestStringValue("Test");

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestStringValue("Test1");
        var value2 = new TestStringValue("Test2");

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestStringValue("Test");
        var value2 = new TestStringValue("TEST");

        // Act & Assert
        // This tests the ValuesAreEqual method in StrValueObject which uses StringComparison.CurrentCultureIgnoreCase
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_NullObject_ShouldBeFalse()
    {
        // Arrange
        var value = new TestStringValue("Test");

        // Act & Assert
        value.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ShouldBeFalse()
    {
        // Arrange
        var value = new TestStringValue("Test");
        var differentType = 123;

        // Act & Assert
        value.Equals(differentType).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestStringValue("Test");
        var value2 = new TestStringValue("Test");

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var value = new TestStringValue("Test");

        // Act & Assert
        value.ToString().ShouldBe("Test");
    }
}
