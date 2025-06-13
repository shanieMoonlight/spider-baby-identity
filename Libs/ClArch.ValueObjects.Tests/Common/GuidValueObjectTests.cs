using ClArch.ValueObjects.Common;
using Shouldly;
using System;
using Xunit;

namespace ClArch.ValueObjects.Tests.Common;

public class GuidValueObjectTests
{
    // Concrete implementation for testing the abstract class
    private class TestGuidValue : GuidValueObject
    {
        public TestGuidValue(Guid value) : base(value) { }
    }

    [Fact]
    public void Create_WithValidValue_ShouldSetValue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        
        // Act
        var guidValue = new TestGuidValue(guid);

        // Assert
        guidValue.Value.ShouldBe(guid);
    }

    [Fact]
    public void Create_WithEmptyGuid_ShouldSetEmptyGuid()
    {
        // Arrange & Act
        var guidValue = new TestGuidValue(Guid.Empty);

        // Assert
        guidValue.Value.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var value1 = new TestGuidValue(guid);
        var value2 = new TestGuidValue(guid);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestGuidValue(Guid.NewGuid());
        var value2 = new TestGuidValue(Guid.NewGuid());

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_NullObject_ShouldBeFalse()
    {
        // Arrange
        var value = new TestGuidValue(Guid.NewGuid());
        object? nullObject = null;

        // Act & Assert
        value.Equals(nullObject).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ShouldBeFalse()
    {
        // Arrange
        var value = new TestGuidValue(Guid.NewGuid());
        var differentType = "Not a GuidValueObject";

        // Act & Assert
        value.Equals(differentType).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var value1 = new TestGuidValue(guid);
        var value2 = new TestGuidValue(guid);

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ShouldBeDifferent()
    {
        // Arrange
        var value1 = new TestGuidValue(Guid.NewGuid());
        var value2 = new TestGuidValue(Guid.NewGuid());

        // Act & Assert
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var value = new TestGuidValue(guid);

        // Act & Assert
        value.ToString().ShouldBe(guid.ToString());
    }
}
