using ClArch.ValueObjects.Common;
using Shouldly;
using System;
using Xunit;

namespace ClArch.ValueObjects.Tests.Common;

public class NullableGuidValueObjectTests
{
    // Concrete implementation for testing the abstract class
    private class TestNullableGuidValue : NullableGuidValueObject
    {
        public TestNullableGuidValue(Guid? value) : base(value) { }
    }

    [Fact]
    public void Create_WithValidValue_ShouldSetValue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        
        // Act
        var guidValue = new TestNullableGuidValue(guid);

        // Assert
        guidValue.Value.ShouldBe(guid);
    }

    [Fact]
    public void Create_WithNullValue_ShouldSetValueToNull()
    {
        // Arrange & Act
        var guidValue = new TestNullableGuidValue(null);

        // Assert
        guidValue.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyGuid_ShouldSetEmptyGuid()
    {
        // Arrange & Act
        var guidValue = new TestNullableGuidValue(Guid.Empty);

        // Assert
        guidValue.Value.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var value1 = new TestNullableGuidValue(guid);
        var value2 = new TestNullableGuidValue(guid);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestNullableGuidValue(Guid.NewGuid());
        var value2 = new TestNullableGuidValue(Guid.NewGuid());

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestNullableGuidValue(null);
        var value2 = new TestNullableGuidValue(null);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestNullableGuidValue(null);
        var value2 = new TestNullableGuidValue(Guid.NewGuid());

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
        var value = new TestNullableGuidValue(Guid.NewGuid());
        object? nullObject = null;

        // Act & Assert
        value.Equals(nullObject).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ShouldBeFalse()
    {
        // Arrange
        var value = new TestNullableGuidValue(Guid.NewGuid());
        var differentType = "Not a NullableGuidValueObject";

        // Act & Assert
        value.Equals(differentType).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var value1 = new TestNullableGuidValue(guid);
        var value2 = new TestNullableGuidValue(guid);

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestNullableGuidValue(null);
        var value2 = new TestNullableGuidValue(null);

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ShouldBeDifferent()
    {
        // Arrange
        var value1 = new TestNullableGuidValue(Guid.NewGuid());
        var value2 = new TestNullableGuidValue(Guid.NewGuid());

        // Act & Assert
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_OneNull_ShouldBeDifferent()
    {
        // Arrange
        var value1 = new TestNullableGuidValue(null);
        var value2 = new TestNullableGuidValue(Guid.NewGuid());

        // Act & Assert
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnStringRepresentation()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var value = new TestNullableGuidValue(guid);

        // Act & Assert
        value.ToString().ShouldBe(guid.ToString());
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var value = new TestNullableGuidValue(null);

        // Act & Assert
        value.ToString().ShouldBe(string.Empty);
    }
}
