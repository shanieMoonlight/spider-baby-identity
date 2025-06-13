using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Setup;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class FirstNameNullableTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validName = "John";

        // Act
        var firstName = FirstNameNullable.Create(validName);

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
        var firstName = FirstNameNullable.Create(nameWithSpaces);

        // Assert
        firstName.Value.ShouldBe(expectedName);
    }

    [Fact]
    public void Create_WithNullValue_ShouldNotThrowException()
    {
        // Arrange
        string? nullName = null;

        // Act & Assert
        Should.NotThrow(() => FirstNameNullable.Create(nullName));
    }

    [Fact]
    public void Create_WithNullValue_ShouldHaveNullValue()
    {
        // Arrange
        string? nullName = null;

        // Act
        var firstName = FirstNameNullable.Create(nullName);

        // Assert
        firstName.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyString_ShouldNotThrowException()
    {
        // Arrange
        string emptyName = "";

        // Act & Assert
        Should.NotThrow(() => FirstNameNullable.Create(emptyName));
    }

    [Fact]
    public void Create_WithEmptyString_ShouldHaveNullValue()
    {
        // Arrange
        string emptyName = "";

        // Act
        var firstName = FirstNameNullable.Create(emptyName);

        // Assert
        firstName.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldHaveNullValue()
    {
        // Arrange
        string whiteSpaceName = "   ";

        // Act
        var firstName = FirstNameNullable.Create(whiteSpaceName);

        // Assert
        firstName.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongName = new('a', FirstNameNullable.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            FirstNameNullable.Create(tooLongName))
            .Property.ShouldBe(nameof(FirstName));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthName = new('a', FirstNameNullable.MaxLength);

        // Act
        var firstName = FirstNameNullable.Create(maxLengthName);

        // Assert
        firstName.ShouldNotBeNull();
        firstName.Value.ShouldBe(maxLengthName);
    }        [Fact]
    public void MaxLength_ShouldMatchExpectedValue()
    {
        // Assert
        FirstNameNullable.MaxLength.ShouldBe(FirstName.MaxLength);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var firstName1 = FirstNameNullable.Create("John");
        var firstName2 = FirstNameNullable.Create("John");

        // Act & Assert
        firstName1.Equals(firstName2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var firstName1 = FirstNameNullable.Create(null);
        var firstName2 = FirstNameNullable.Create(null);

        // Act & Assert
        firstName1.Equals(firstName2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var firstName1 = FirstNameNullable.Create(null);
        var firstName2 = FirstNameNullable.Create("John");

        // Act & Assert
        firstName1.Equals(firstName2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var firstName1 = FirstNameNullable.Create("John");
        var firstName2 = FirstNameNullable.Create("Jane");

        // Act & Assert
        firstName1.Equals(firstName2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var firstName1 = FirstNameNullable.Create("John");
        var firstName2 = FirstNameNullable.Create("JOHN");

        // Act & Assert
        firstName1.Equals(firstName2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var firstName1 = FirstNameNullable.Create("John");
        var firstName2 = FirstNameNullable.Create("John");

        // Act & Assert
        firstName1.GetHashCode().ShouldBe(firstName2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var firstName1 = FirstNameNullable.Create(null);
        var firstName2 = FirstNameNullable.Create(null);

        // Act & Assert
        firstName1.GetHashCode().ShouldBe(firstName2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValue()
    {
        // Arrange
        string value = "John";
        var firstName = FirstNameNullable.Create(value);

        // Act & Assert
        firstName.ToString().ShouldBe(value);
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var firstName = FirstNameNullable.Create(null);

        // Act & Assert
        firstName.ToString().ShouldBe(string.Empty);
    }
}
