using ClArch.ValueObjects.Exceptions;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
public class FirstNameTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validName = "John";

        // Act
        var firstName = FirstName.Create(validName);

        // Assert
        firstName.ShouldNotBeNull();
        firstName.Value.ShouldBe(validName);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string nameWithSpaces = "  John  ";
        string expectedName = "John";

        // Act
        var firstName = FirstName.Create(nameWithSpaces);

        // Assert
        firstName.Value.ShouldBe(expectedName);
    }

    [Fact]
    public void Create_WithNullValue_ShouldThrowException()
    {
        // Arrange
        string? nullName = null;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            FirstName.Create(nullName))
            .Property.ShouldBe(nameof(FirstName));
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        string emptyName = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            FirstName.Create(emptyName))
            .Property.ShouldBe(nameof(FirstName));
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldThrowException()
    {
        // Arrange
        string whiteSpaceName = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            FirstName.Create(whiteSpaceName))
            .Property.ShouldBe(nameof(FirstName));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongName = new('a', FirstName.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            FirstName.Create(tooLongName))
            .Property.ShouldBe(nameof(FirstName));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthName = new('a', FirstName.MaxLength);

        // Act
        var firstName = FirstName.Create(maxLengthName);

        // Assert
        firstName.ShouldNotBeNull();
        firstName.Value.ShouldBe(maxLengthName);
    }        [Fact]
    public void MaxLength_ShouldHaveExpectedValue()
    {
        // Assert
        FirstName.MaxLength.ShouldBePositive();
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var firstName1 = FirstName.Create("John");
        var firstName2 = FirstName.Create("John");

        // Act & Assert
        firstName1.Equals(firstName2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var firstName1 = FirstName.Create("John");
        var firstName2 = FirstName.Create("Jane");

        // Act & Assert
        firstName1.Equals(firstName2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var firstName1 = FirstName.Create("John");
        var firstName2 = FirstName.Create("JOHN");

        // Act & Assert
        firstName1.Equals(firstName2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var firstName1 = FirstName.Create("John");
        var firstName2 = FirstName.Create("John");

        // Act & Assert
        firstName1.GetHashCode().ShouldBe(firstName2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string value = "John";
        var firstName = FirstName.Create(value);

        // Act & Assert
        firstName.ToString().ShouldBe(value);
    }
}
