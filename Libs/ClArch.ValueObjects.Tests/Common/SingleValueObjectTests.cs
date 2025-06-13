using ClArch.ValueObjects.Common;
using Shouldly;

namespace ClArch.ValueObjects.Tests.Common;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
public class SingleValueObjectTests
{
    // Concrete implementation for testing the abstract class
    private class TestSingleValueObject(int value) : SingleValueObject
    {
        public override IEnumerable<object> GetAtomicValues()
        {
            yield return value;
        }
    }

    // Another implementation with multiple atomic values
    private class ComplexSingleValueObject(int id, string name) : SingleValueObject
    {
        public override IEnumerable<object> GetAtomicValues()
        {
            yield return id;
            yield return name;
        }
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestSingleValueObject(42);
        var value2 = new TestSingleValueObject(42);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestSingleValueObject(42);
        var value2 = new TestSingleValueObject(43);

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_NullObject_ShouldBeFalse()
    {
        // Arrange
        var value = new TestSingleValueObject(42);

        // Act & Assert
        value.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ShouldBeFalse()
    {
        // Arrange
        var value = new TestSingleValueObject(42);
        var differentType = "Not a SingleValueObject";

        // Act & Assert
        value.Equals(differentType).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestSingleValueObject(42);
        var value2 = new TestSingleValueObject(42);

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ShouldBeDifferent()
    {
        // Arrange
        var value1 = new TestSingleValueObject(42);
        var value2 = new TestSingleValueObject(43);

        // Act & Assert
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }

    [Fact]
    public void ComplexObject_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var value1 = new ComplexSingleValueObject(1, "Test");
        var value2 = new ComplexSingleValueObject(1, "Test");

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void ComplexObject_WithDifferentId_ShouldNotBeEqual()
    {
        // Arrange
        var value1 = new ComplexSingleValueObject(1, "Test");
        var value2 = new ComplexSingleValueObject(2, "Test");

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }

    [Fact]
    public void ComplexObject_WithDifferentName_ShouldNotBeEqual()
    {
        // Arrange
        var value1 = new ComplexSingleValueObject(1, "Test1");
        var value2 = new ComplexSingleValueObject(1, "Test2");

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }

    [Fact]
    public void ComplexObject_WithNullName_ShouldHandleEquality()
    {
        // Arrange
        var value1 = new ComplexSingleValueObject(1, null);
        var value2 = new ComplexSingleValueObject(1, null);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void ComplexObject_WithOneNullName_ShouldNotBeEqual()
    {
        // Arrange
        var value1 = new ComplexSingleValueObject(1, null);
        var value2 = new ComplexSingleValueObject(1, "Test");

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }
}
