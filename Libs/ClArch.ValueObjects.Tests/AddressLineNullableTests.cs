using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Setup;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;


#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class AddressLineNullableTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validAddress = "123 Main Street";

        // Act
        var addressLine = AddressLineNullable.Create(validAddress);

        // Assert
        addressLine.ShouldNotBeNull();
        addressLine.Value.ShouldBe(validAddress);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnInstanceWithNullValue()
    {
        // Arrange
        string nullAddress = null;

        // Act
        var addressLine = AddressLineNullable.Create(nullAddress);

        // Assert
        addressLine.ShouldNotBeNull();
        addressLine.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyString_ShouldReturnInstanceWithEmptyString()
    {
        // Arrange
        string emptyAddress = "";

        // Act
        var addressLine = AddressLineNullable.Create(emptyAddress);

        // Assert
        addressLine.Value.ShouldBe("");
    }

    [Fact]
    public void Create_WithWhitespaceOnly_ShouldReturnTrimmedEmptyString()
    {
        // Arrange
        string whitespaceAddress = "   ";

        // Act
        var addressLine = AddressLineNullable.Create(whitespaceAddress);

        // Assert
        addressLine.Value.ShouldBe("");
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string addressWithSpaces = "  123 Main Street  ";

        // Act
        var addressLine = AddressLineNullable.Create(addressWithSpaces);

        // Assert
        addressLine.Value.ShouldBe("123 Main Street");
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongAddress = new('A', ValueObjectsDefaultValues.MAX_LENGTH_ADDRESS_LINE + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            AddressLineNullable.Create(tooLongAddress))
            .Property.ShouldBe(nameof(AddressLine));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthAddress = new('A', ValueObjectsDefaultValues.MAX_LENGTH_ADDRESS_LINE);

        // Act
        var addressLine = AddressLineNullable.Create(maxLengthAddress);

        // Assert
        addressLine.ShouldNotBeNull();
        addressLine.Value.ShouldBe(maxLengthAddress);
    }

    [Fact]
    public void MaxLength_ShouldMatchDefaultValue()
    {
        // Arrange & Act & Assert
        AddressLineNullable.MaxLength.ShouldBe(ValueObjectsDefaultValues.MAX_LENGTH_ADDRESS_LINE);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var address1 = AddressLineNullable.Create("123 Main Street");
        var address2 = AddressLineNullable.Create("123 Main Street");

        // Act & Assert
        address1.Equals(address2).ShouldBeTrue();
        (address1 == address2).ShouldBeTrue();
        (address1 != address2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var address1 = AddressLineNullable.Create("123 Main Street");
        var address2 = AddressLineNullable.Create("456 Elm Avenue");

        // Act & Assert
        address1.Equals(address2).ShouldBeFalse();
        (address1 == address2).ShouldBeFalse();
        (address1 != address2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var address1 = AddressLineNullable.Create("123 Main Street");
        var address2 = AddressLineNullable.Create("123 MAIN STREET");

        // Act & Assert
        // NullableStrValueObject uses case-insensitive comparison
        address1.Equals(address2).ShouldBeTrue();
        (address1 == address2).ShouldBeTrue();
        (address1 != address2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var address1 = AddressLineNullable.Create(null);
        var address2 = AddressLineNullable.Create(null);

        // Act & Assert
        address1.Equals(address2).ShouldBeTrue();
        (address1 == address2).ShouldBeTrue();
        (address1 != address2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var address1 = AddressLineNullable.Create(null);
        var address2 = AddressLineNullable.Create("123 Main Street");

        // Act & Assert
        address1.Equals(address2).ShouldBeFalse();
        (address1 == address2).ShouldBeFalse();
        (address1 != address2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var address1 = AddressLineNullable.Create("123 Main Street");
        var address2 = AddressLineNullable.Create("123 Main Street");

        // Act & Assert
        address1.GetHashCode().ShouldBe(address2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var address1 = AddressLineNullable.Create(null);
        var address2 = AddressLineNullable.Create(null);

        // Act & Assert
        address1.GetHashCode().ShouldBe(address2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValue()
    {
        // Arrange
        var address = AddressLineNullable.Create("123 Main Street");

        // Act & Assert
        address.ToString().ShouldBe("123 Main Street");
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var address = AddressLineNullable.Create(null);

        // Act & Assert
        address.ToString().ShouldBe(string.Empty);
    }
}
