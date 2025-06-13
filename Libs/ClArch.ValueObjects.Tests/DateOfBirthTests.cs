using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class DateOfBirthTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        DateTime validDate = new DateTime(1990, 1, 1);

        // Act
        var dob = DateOfBirth.Create(validDate);

        // Assert
        dob.ShouldNotBeNull();
        dob.Value.ShouldBe(validDate);
    }

    [Fact]
    public void Create_WithDefaultValue_ShouldThrowException()
    {
        // Arrange
        DateTime defaultDate = default;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            DateOfBirth.Create(defaultDate))
            .Property.ShouldBe(nameof(DateOfBirth));
    }

    [Fact]
    public void Create_WithCurrentDate_ShouldReturnInstance()
    {
        // Arrange
        DateTime currentDate = DateTime.Now;

        // Act
        var dob = DateOfBirth.Create(currentDate);

        // Assert
        dob.ShouldNotBeNull();
        dob.Value.ShouldBe(currentDate);
    }

    [Fact]
    public void Create_WithPastDate_ShouldReturnInstance()
    {
        // Arrange
        DateTime pastDate = new DateTime(1950, 6, 15);

        // Act
        var dob = DateOfBirth.Create(pastDate);

        // Assert
        dob.ShouldNotBeNull();
        dob.Value.ShouldBe(pastDate);
    }

    [Fact]
    public void Create_WithFutureDate_ShouldReturnInstance()
    {
        // Arrange - This is a valid test case since the class doesn't restrict future dates
        DateTime futureDate = DateTime.Now.AddYears(1);

        // Act
        var dob = DateOfBirth.Create(futureDate);

        // Assert
        dob.ShouldNotBeNull();
        dob.Value.ShouldBe(futureDate);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var date = new DateTime(1990, 1, 1);
        var dob1 = DateOfBirth.Create(date);
        var dob2 = DateOfBirth.Create(date);

        // Act & Assert
        dob1.Equals(dob2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var dob1 = DateOfBirth.Create(new DateTime(1990, 1, 1));
        var dob2 = DateOfBirth.Create(new DateTime(1991, 1, 1));

        // Act & Assert
        dob1.Equals(dob2).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var date = new DateTime(1990, 1, 1);
        var dob1 = DateOfBirth.Create(date);
        var dob2 = DateOfBirth.Create(date);

        // Act & Assert
        dob1.GetHashCode().ShouldBe(dob2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldIncludeValueRepresentation()
    {
        // Arrange
        var date = new DateTime(1990, 1, 1);
        var dob = DateOfBirth.Create(date);

        // Act
        var result = dob.ToString();

        // Assert
        result.ShouldContain(date.ToString());
    }        [Fact]
    public void WithValueProperty_ShouldReturnCorrectValue()
    {
        // Arrange
        var date = new DateTime(1990, 1, 1);
        var dob = DateOfBirth.Create(date);

        // Act & Assert
        dob.Value.ShouldBe(date);
    }
}
