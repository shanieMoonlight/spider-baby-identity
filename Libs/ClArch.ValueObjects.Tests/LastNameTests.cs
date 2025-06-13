using ClArch.ValueObjects.Exceptions;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
public class LastNameTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validName = "Smith";

        // Act
        var lastName = LastName.Create(validName);

        // Assert
        lastName.ShouldNotBeNull();
        lastName.Value.ShouldBe(validName);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string nameWithSpaces = "  Smith  ";
        string expectedName = "Smith";

        // Act
        var lastName = LastName.Create(nameWithSpaces);

        // Assert
        lastName.Value.ShouldBe(expectedName);
    }

    [Fact]
    public void Create_WithNullValue_ShouldThrowException()
    {
        // Arrange
        string? nullName = null;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            LastName.Create(nullName))
            .Property.ShouldBe(nameof(LastName));
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        string emptyName = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            LastName.Create(emptyName))
            .Property.ShouldBe(nameof(LastName));
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldThrowException()
    {
        // Arrange
        string whiteSpaceName = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            LastName.Create(whiteSpaceName))
            .Property.ShouldBe(nameof(LastName));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongName = new('a', LastName.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            LastName.Create(tooLongName))
            .Property.ShouldBe(nameof(LastName));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthName = new('a', LastName.MaxLength);

        // Act
        var lastName = LastName.Create(maxLengthName);

        // Assert
        lastName.ShouldNotBeNull();
        lastName.Value.ShouldBe(maxLengthName);
    }

    [Fact]
    public void MaxLength_ShouldHaveExpectedValue()
    {
        // Assert
        LastName.MaxLength.ShouldBePositive();
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var lastName1 = LastName.Create("Smith");
        var lastName2 = LastName.Create("Smith");

        // Act & Assert
        lastName1.Equals(lastName2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var lastName1 = LastName.Create("Smith");
        var lastName2 = LastName.Create("Jones");

        // Act & Assert
        lastName1.Equals(lastName2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var lastName1 = LastName.Create("Smith");
        var lastName2 = LastName.Create("SMITH");

        // Act & Assert
        lastName1.Equals(lastName2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var lastName1 = LastName.Create("Smith");
        var lastName2 = LastName.Create("Smith");

        // Act & Assert
        lastName1.GetHashCode().ShouldBe(lastName2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string value = "Smith";
        var lastName = LastName.Create(value);

        // Act & Assert
        lastName.ToString().ShouldBe(value);
    }
}
