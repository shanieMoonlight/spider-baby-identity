using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class RegistrationDateTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        DateTime validDate = new DateTime(1990, 1, 1);

        // Act
        var dob = RegistrationDate.Create(validDate);

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
            RegistrationDate.Create(defaultDate))
            .Property.ShouldBe(nameof(RegistrationDate));
    }

    [Fact]
    public void Create_WithCurrentDate_ShouldReturnInstance()
    {
        // Arrange
        DateTime currentDate = DateTime.Now;

        // Act
        var dob = RegistrationDate.Create(currentDate);

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
        var dob = RegistrationDate.Create(pastDate);

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
        var dob = RegistrationDate.Create(futureDate);

        // Assert
        dob.ShouldNotBeNull();
        dob.Value.ShouldBe(futureDate);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var date = new DateTime(1990, 1, 1);
        var dob1 = RegistrationDate.Create(date);
        var dob2 = RegistrationDate.Create(date);

        // Act & Assert
        dob1.Equals(dob2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var dob1 = RegistrationDate.Create(new DateTime(1990, 1, 1));
        var dob2 = RegistrationDate.Create(new DateTime(1991, 1, 1));

        // Act & Assert
        dob1.Equals(dob2).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var date = new DateTime(1990, 1, 1);
        var dob1 = RegistrationDate.Create(date);
        var dob2 = RegistrationDate.Create(date);

        // Act & Assert
        dob1.GetHashCode().ShouldBe(dob2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldIncludeValueRepresentation()
    {
        // Arrange
        var date = new DateTime(1990, 1, 1);
        var dob = RegistrationDate.Create(date);

        // Act
        var result = dob.ToString();

        // Assert
        result.ShouldContain(date.ToString());
    }        [Fact]
    public void WithValueProperty_ShouldReturnCorrectValue()
    {
        // Arrange
        var date = new DateTime(1990, 1, 1);
        var dob = RegistrationDate.Create(date);

        // Act & Assert
        dob.Value.ShouldBe(date);
    }
}
