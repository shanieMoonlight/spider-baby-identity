using ClArch.ValueObjects.Exceptions;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
public class NameTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validName = "Product Name";

        // Act
        var name = Name.Create(validName);

        // Assert
        name.ShouldNotBeNull();
        name.Value.ShouldBe(validName);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string nameWithSpaces = "  Product Name  ";
        string expectedName = "Product Name";

        // Act
        var name = Name.Create(nameWithSpaces);

        // Assert
        name.Value.ShouldBe(expectedName);
    }

    [Fact]
    public void Create_WithNullValue_ShouldThrowException()
    {
        // Arrange
        string? nullName = null;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            Name.Create(nullName))
            .Property.ShouldBe(nameof(Name));
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        string emptyName = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            Name.Create(emptyName))
            .Property.ShouldBe(nameof(Name));
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldThrowException()
    {
        // Arrange
        string whiteSpaceName = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            Name.Create(whiteSpaceName))
            .Property.ShouldBe(nameof(Name));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongName = new('a', Name.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            Name.Create(tooLongName))
            .Property.ShouldBe(nameof(Name));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthName = new('a', Name.MaxLength);

        // Act
        var name = Name.Create(maxLengthName);

        // Assert
        name.ShouldNotBeNull();
        name.Value.ShouldBe(maxLengthName);
    }

    [Fact]
    public void MaxLength_ShouldHaveExpectedValue()
    {
        // Assert
        Name.MaxLength.ShouldBePositive();
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var name1 = Name.Create("Product Name");
        var name2 = Name.Create("Product Name");

        // Act & Assert
        name1.Equals(name2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var name1 = Name.Create("Product A");
        var name2 = Name.Create("Product B");

        // Act & Assert
        name1.Equals(name2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var name1 = Name.Create("Product Name");
        var name2 = Name.Create("PRODUCT NAME");

        // Act & Assert
        name1.Equals(name2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var name1 = Name.Create("Product Name");
        var name2 = Name.Create("Product Name");

        // Act & Assert
        name1.GetHashCode().ShouldBe(name2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string value = "Product Name";
        var name = Name.Create(value);

        // Act & Assert
        name.ToString().ShouldBe(value);
    }
}
