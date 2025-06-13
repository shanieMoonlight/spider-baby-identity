using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class LastNameNullableTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validName = "Smith";

        // Act
        var lastName = LastNameNullable.Create(validName);

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
        var lastName = LastNameNullable.Create(nameWithSpaces);

        // Assert
        lastName.Value.ShouldBe(expectedName);
    }

    [Fact]
    public void Create_WithNullValue_ShouldNotThrowException()
    {
        // Arrange
        string? nullName = null;

        // Act & Assert
        Should.NotThrow(() => LastNameNullable.Create(nullName));
    }

    [Fact]
    public void Create_WithNullValue_ShouldHaveNullValue()
    {
        // Arrange
        string? nullName = null;

        // Act
        var lastName = LastNameNullable.Create(nullName);

        // Assert
        lastName.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyString_ShouldNotThrowException()
    {
        // Arrange
        string emptyName = "";

        // Act & Assert
        Should.NotThrow(() => LastNameNullable.Create(emptyName));
    }

    [Fact]
    public void Create_WithEmptyString_ShouldHaveNullValue()
    {
        // Arrange
        string emptyName = "";

        // Act
        var lastName = LastNameNullable.Create(emptyName);

        // Assert
        lastName.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldHaveNullValue()
    {
        // Arrange
        string whiteSpaceName = "   ";

        // Act
        var lastName = LastNameNullable.Create(whiteSpaceName);

        // Assert
        lastName.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongName = new('a', LastNameNullable.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            LastNameNullable.Create(tooLongName))
            .Property.ShouldBe(nameof(LastName));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthName = new('a', LastNameNullable.MaxLength);

        // Act
        var lastName = LastNameNullable.Create(maxLengthName);

        // Assert
        lastName.ShouldNotBeNull();
        lastName.Value.ShouldBe(maxLengthName);
    }

    [Fact]
    public void MaxLength_ShouldMatchExpectedValue()
    {
        // Assert
        LastNameNullable.MaxLength.ShouldBe(LastName.MaxLength);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var lastName1 = LastNameNullable.Create("Smith");
        var lastName2 = LastNameNullable.Create("Smith");

        // Act & Assert
        lastName1.Equals(lastName2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var lastName1 = LastNameNullable.Create(null);
        var lastName2 = LastNameNullable.Create(null);

        // Act & Assert
        lastName1.Equals(lastName2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var lastName1 = LastNameNullable.Create(null);
        var lastName2 = LastNameNullable.Create("Smith");

        // Act & Assert
        lastName1.Equals(lastName2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var lastName1 = LastNameNullable.Create("Smith");
        var lastName2 = LastNameNullable.Create("Jones");

        // Act & Assert
        lastName1.Equals(lastName2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var lastName1 = LastNameNullable.Create("Smith");
        var lastName2 = LastNameNullable.Create("SMITH");

        // Act & Assert
        lastName1.Equals(lastName2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var lastName1 = LastNameNullable.Create("Smith");
        var lastName2 = LastNameNullable.Create("Smith");

        // Act & Assert
        lastName1.GetHashCode().ShouldBe(lastName2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var lastName1 = LastNameNullable.Create(null);
        var lastName2 = LastNameNullable.Create(null);

        // Act & Assert
        lastName1.GetHashCode().ShouldBe(lastName2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValue()
    {
        // Arrange
        string value = "Smith";
        var lastName = LastNameNullable.Create(value);

        // Act & Assert
        lastName.ToString().ShouldBe(value);
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var lastName = LastNameNullable.Create(null);

        // Act & Assert
        lastName.ToString().ShouldBe(string.Empty);
    }
}
