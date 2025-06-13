using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class PhoneTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validPhone = "+1-123-456-7890";

        // Act
        var phone = Phone.Create(validPhone);

        // Assert
        phone.ShouldNotBeNull();
        phone.Value.ShouldBe(validPhone);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string phoneWithSpaces = "  +1-123-456-7890  ";
        string expectedPhone = "+1-123-456-7890";

        // Act
        var phone = Phone.Create(phoneWithSpaces);

        // Assert
        phone.Value.ShouldBe(expectedPhone);
    }

    [Fact]
    public void Create_WithInvalidPhone_ShouldThrowException()
    {
        // Arrange
        string invalidPhone = "invalid-phone";

        // Act & Assert
        Should.Throw<InvalidPropertyException>(() => 
            Phone.Create(invalidPhone))
            .Property.ShouldBe(nameof(Phone));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongPhone = new('1', Phone.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            Phone.Create(tooLongPhone))
            .Property.ShouldBe(nameof(Phone));
    }

    [Fact]
    public void MaxLength_ShouldHaveExpectedValue()
    {
        // Assert
        Phone.MaxLength.ShouldBePositive();
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var phone1 = Phone.Create("+1-123-456-7890");
        var phone2 = Phone.Create("+1-123-456-7890");

        // Act & Assert
        phone1.Equals(phone2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var phone1 = Phone.Create("+1-123-456-7890");
        var phone2 = Phone.Create("+1-987-654-3210");

        // Act & Assert
        phone1.Equals(phone2).ShouldBeFalse();
    }


    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var phone1 = Phone.Create("+1-123-456-7890");
        var phone2 = Phone.Create("+1-123-456-7890");

        // Act & Assert
        phone1.GetHashCode().ShouldBe(phone2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string value = "+1-123-456-7890";
        var phone = Phone.Create(value);

        // Act & Assert
        phone.ToString().ShouldBe(value);
    }
}
