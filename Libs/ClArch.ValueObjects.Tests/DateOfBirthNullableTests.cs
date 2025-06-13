using ClArch.ValueObjects;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class DateOfBirthNullableTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        DateTime? validDate = new DateTime(1990, 1, 1);

        // Act
        var dob = DateOfBirthNullable.Create(validDate);

        // Assert
        dob.ShouldNotBeNull();
        dob.Value.ShouldBe(validDate);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnInstanceWithNullValue()
    {
        // Arrange
        DateTime? nullDate = null;

        // Act
        var dob = DateOfBirthNullable.Create(nullDate);

        // Assert
        dob.ShouldNotBeNull();
        dob.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithDefaultValue_ShouldReturnInstanceWithNullValue()
    {
        // Arrange
        DateTime? defaultDate = default;

        // Act
        var dob = DateOfBirthNullable.Create(defaultDate);

        // Assert
        dob.ShouldNotBeNull();
        dob.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithDefaultDateTime_ShouldReturnInstanceWithNullValue()
    {
        // Arrange
        DateTime? defaultDateTime = default(DateTime);

        // Act
        var dob = DateOfBirthNullable.Create(defaultDateTime);

        // Assert
        dob.ShouldNotBeNull();
        dob.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithNoParameters_ShouldReturnInstanceWithNullValue()
    {
        // Act
        var dob = DateOfBirthNullable.Create();

        // Assert
        dob.ShouldNotBeNull();
        dob.Value.ShouldBeNull();
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var date = new DateTime(1990, 1, 1);
        var dob1 = DateOfBirthNullable.Create(date);
        var dob2 = DateOfBirthNullable.Create(date);

        // Act & Assert
        dob1.Equals(dob2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var dob1 = DateOfBirthNullable.Create(null);
        var dob2 = DateOfBirthNullable.Create(null);

        // Act & Assert
        dob1.Equals(dob2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var dob1 = DateOfBirthNullable.Create(null);
        var dob2 = DateOfBirthNullable.Create(new DateTime(1990, 1, 1));

        // Act & Assert
        dob1.Equals(dob2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var dob1 = DateOfBirthNullable.Create(new DateTime(1990, 1, 1));
        var dob2 = DateOfBirthNullable.Create(new DateTime(1991, 1, 1));

        // Act & Assert
        dob1.Equals(dob2).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var date = new DateTime(1990, 1, 1);
        var dob1 = DateOfBirthNullable.Create(date);
        var dob2 = DateOfBirthNullable.Create(date);

        // Act & Assert
        dob1.GetHashCode().ShouldBe(dob2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var dob1 = DateOfBirthNullable.Create(null);
        var dob2 = DateOfBirthNullable.Create(null);

        // Act & Assert
        dob1.GetHashCode().ShouldBe(dob2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldIncludeValueRepresentation()
    {
        // Arrange
        var date = new DateTime(1990, 1, 1);
        var dob = DateOfBirthNullable.Create(date);

        // Act
        var result = dob.ToString();

        // Assert
        result.ShouldContain(date.ToString());
    }

    [Fact]
    public void ToString_WithNull_ShouldNotBeEmpty()
    {
        // Arrange
        var dob = DateOfBirthNullable.Create(null);

        // Assert
        dob.Value.ShouldBeNull();
    }

    [Fact]
    public void WithValueProperty_ShouldReturnCorrectValue()
    {
        // Arrange
        DateTime? date = new DateTime(1990, 1, 1);
        var dob = DateOfBirthNullable.Create(date);

        // Act & Assert
        dob.Value.ShouldBe(date);
    }
}
