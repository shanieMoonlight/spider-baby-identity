using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Setup;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class AddressLineTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validAddress = "123 Main Street";

        // Act
        var addressLine = AddressLine.Create(validAddress);

        // Assert
        addressLine.ShouldNotBeNull();
        addressLine.Value.ShouldBe(validAddress);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string addressWithSpaces = "  123 Main Street  ";

        // Act
        var addressLine = AddressLine.Create(addressWithSpaces);

        // Assert
        addressLine.Value.ShouldBe("123 Main Street");
    }

    [Fact]
    public void Create_WithNullValue_ShouldThrowException()
    {
        // Arrange
        string nullAddress = null;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            AddressLine.Create(nullAddress))
            .Property.ShouldBe(nameof(AddressLine));
    }

    [Fact]
    public void Create_WithEmptyString_ShouldThrowException()
    {
        // Arrange
        string emptyAddress = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            AddressLine.Create(emptyAddress))
            .Property.ShouldBe(nameof(AddressLine));
    }

    [Fact]
    public void Create_WithWhitespaceOnly_ShouldThrowException()
    {
        // Arrange
        string whitespaceAddress = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            AddressLine.Create(whitespaceAddress))
            .Property.ShouldBe(nameof(AddressLine));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongAddress = new('A', ValueObjectsDefaultValues.MAX_LENGTH_ADDRESS_LINE + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            AddressLine.Create(tooLongAddress))
            .Property.ShouldBe(nameof(AddressLine));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthAddress = new('A', ValueObjectsDefaultValues.MAX_LENGTH_ADDRESS_LINE);

        // Act
        var addressLine = AddressLine.Create(maxLengthAddress);

        // Assert
        addressLine.ShouldNotBeNull();
        addressLine.Value.ShouldBe(maxLengthAddress);
    }

    [Fact]
    public void MaxLength_ShouldMatchDefaultValue()
    {
        // Arrange & Act & Assert
        AddressLine.MaxLength.ShouldBe(ValueObjectsDefaultValues.MAX_LENGTH_ADDRESS_LINE);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var address1 = AddressLine.Create("123 Main Street");
        var address2 = AddressLine.Create("123 Main Street");

        // Act & Assert
        address1.Equals(address2).ShouldBeTrue();
        (address1 == address2).ShouldBeTrue();
        (address1 != address2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var address1 = AddressLine.Create("123 Main Street");
        var address2 = AddressLine.Create("456 Elm Avenue");

        // Act & Assert
        address1.Equals(address2).ShouldBeFalse();
        (address1 == address2).ShouldBeFalse();
        (address1 != address2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var address1 = AddressLine.Create("123 Main Street");
        var address2 = AddressLine.Create("123 MAIN STREET");

        // Act & Assert
        // StrValueObject uses case-insensitive comparison
        address1.Equals(address2).ShouldBeTrue();
        (address1 == address2).ShouldBeTrue();
        (address1 != address2).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var address1 = AddressLine.Create("123 Main Street");
        var address2 = AddressLine.Create("123 Main Street");

        // Act & Assert
        address1.GetHashCode().ShouldBe(address2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var address = AddressLine.Create("123 Main Street");

        // Act & Assert
        address.ToString().ShouldBe("123 Main Street");
    }
}
