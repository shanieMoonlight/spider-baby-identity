using ClArch.ValueObjects.Common;
using Shouldly;

namespace ClArch.ValueObjects.Tests.Common;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class ValueObjectTests
{
    // Concrete implementation for testing the abstract class
    private class TestValueObject(int value) : ValueObject<int>(value)
    { }

    // Another concrete implementation for reference equality tests
    private class AnotherTestValueObject(int value) : ValueObject<int>(value)
    { }

    // String-based implementation for string comparisons
    private class TestStringValueObject(string value) : ValueObject<string>(value)
    { }

    // Nullable implementation
    private class TestNullableValueObject(int? value) : ValueObject<int?>(value)
    {        }

    [Fact]
    public void Create_WithValidValue_ShouldSetValue()
    {
        // Arrange & Act
        var valueObject = new TestValueObject(42);

        // Assert
        valueObject.Value.ShouldBe(42);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var value1 = new TestValueObject(42);
        var value2 = new TestValueObject(42);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var value1 = new TestValueObject(42);
        var value2 = new TestValueObject(43);

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_NullObject_ShouldBeFalse()
    {
        // Arrange
        var value = new TestValueObject(42);
        TestValueObject nullValue = null;

        // Act & Assert
        value.Equals(nullValue).ShouldBeFalse();
        (value == nullValue).ShouldBeFalse();
        (value != nullValue).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNullObjects_ShouldBeTrue()
    {
        // Arrange
        TestValueObject value1 = null;
        TestValueObject value2 = null;

        // Act & Assert
        // Using operator overloads for null comparison
        (value1 == value2).ShouldBeTrue();
        (value1 != value2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentType_ShouldBeFalse()
    {
        // Arrange
        var value = new TestValueObject(42);
        var differentType = "Not a ValueObject";

        // Act & Assert
        value.Equals(differentType).ShouldBeFalse();
    }


    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestValueObject(42);
        var value2 = new TestValueObject(42);

        // Act & Assert
        value1.GetHashCode().ShouldBe(value2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValues_ShouldBeDifferent()
    {
        // Arrange
        var value1 = new TestValueObject(42);
        var value2 = new TestValueObject(43);

        // Act & Assert
        value1.GetHashCode().ShouldNotBe(value2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentationOfValue()
    {
        // Arrange
        var value = new TestValueObject(42);

        // Act & Assert
        value.ToString().ShouldBe("42");
    }

    [Fact]
    public void StringValueObject_WithNullValue_ShouldHandleToString()
    {
        // Arrange
        var value = new TestStringValueObject(null!);

        // Act & Assert
        value.ToString().ShouldBe(string.Empty);
    }

    [Fact]
    public void NullableValueObject_WithValue_ShouldHandleEquality()
    {
        // Arrange
        var value1 = new TestNullableValueObject(42);
        var value2 = new TestNullableValueObject(42);

        // Act & Assert
        value1.Equals(value2).ShouldBeTrue();
        (value1 == value2).ShouldBeTrue();
    }

    [Fact]
    public void NullableValueObject_MixedNullAndValue_ShouldNotBeEqual()
    {
        // Arrange
        var value1 = new TestNullableValueObject(null);
        var value2 = new TestNullableValueObject(42);

        // Act & Assert
        value1.Equals(value2).ShouldBeFalse();
        (value1 == value2).ShouldBeFalse();
        (value1 != value2).ShouldBeTrue();
    }
}
