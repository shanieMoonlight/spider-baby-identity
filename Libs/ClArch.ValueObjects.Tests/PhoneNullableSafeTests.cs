using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class PhoneNullableSafeTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validPhone = "+1-123-456-7890";

        // Act
        var phone = PhoneNullableSafe.Create(validPhone);

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
        var phone = PhoneNullableSafe.Create(phoneWithSpaces);

        // Assert
        phone.Value.ShouldBe(expectedPhone);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnInstanceWithNullValue()
    {
        // Arrange
        string? nullPhone = null;

        // Act
        var phone = PhoneNullableSafe.Create(nullPhone);

        // Assert
        phone.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyString_ShouldReturnInstanceWithNullValue()
    {
        // Arrange
        string emptyPhone = "";

        // Act
        var phone = PhoneNullableSafe.Create(emptyPhone);

        // Assert
        phone.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithInvalidPhone_ShouldReturnInstanceWithNullValue()
    {
        // Arrange
        string invalidPhone = "invalid-phone";

        // Act
        var phone = PhoneNullableSafe.Create(invalidPhone);

        // Assert
        phone.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongPhone = new('1', PhoneNullableSafe.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            PhoneNullableSafe.Create(tooLongPhone))
            .Property.ShouldBe(nameof(Phone));
    }

    [Fact]
    public void MaxLength_ShouldMatchExpectedValue()
    {
        // Assert
        PhoneNullableSafe.MaxLength.ShouldBe(Phone.MaxLength);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var validPhone = "+1-123-456-7890";
        
        //// Skip this test if the PhoneValidator rejects this format
        //if (!PhoneValidator.IsValid(validPhone))
        //{
        //    Skip.If(true, "Test skipped because PhoneValidator doesn't accept the test format");
        //    return;
        //}
        
        var phone1 = PhoneNullableSafe.Create(validPhone);
        var phone2 = PhoneNullableSafe.Create(validPhone);

        // Act & Assert
        phone1.Equals(phone2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var phone1 = PhoneNullableSafe.Create(null);
        var phone2 = PhoneNullableSafe.Create(null);

        // Act & Assert
        phone1.Equals(phone2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var validPhone = "+1-123-456-7890";
        
        //// Skip this test if the PhoneValidator rejects this format
        //if (!PhoneValidator.IsValid(validPhone))
        //{
        //    Skip.If(true, "Test skipped because PhoneValidator doesn't accept the test format");
        //    return;
        //}
        
        var phone1 = PhoneNullableSafe.Create(null);
        var phone2 = PhoneNullableSafe.Create(validPhone);

        // Act & Assert
        phone1.Equals(phone2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_InvalidPhoneEqualsNull_ShouldBeTrue()
    {
        // Arrange
        var phone1 = PhoneNullableSafe.Create("invalid-phone");
        var phone2 = PhoneNullableSafe.Create(null);

        // Act & Assert
        phone1.Equals(phone2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var validPhone = "+1-123-456-7890";
        
        //// Skip this test if the PhoneValidator rejects this format
        //if (!PhoneValidator.IsValid(validPhone))
        //{
        //    Skip.If(true, "Test skipped because PhoneValidator doesn't accept the test format");
        //    return;
        //}
        
        var phone1 = PhoneNullableSafe.Create(validPhone);
        var phone2 = PhoneNullableSafe.Create(validPhone);

        // Act & Assert
        phone1.GetHashCode().ShouldBe(phone2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var phone1 = PhoneNullableSafe.Create(null);
        var phone2 = PhoneNullableSafe.Create(null);

        // Act & Assert
        phone1.GetHashCode().ShouldBe(phone2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValue()
    {
        // Arrange
        var validPhone = "+1-123-456-7890";
        
        //// Skip this test if the PhoneValidator rejects this format
        //if (!PhoneValidator.IsValid(validPhone))
        //{
        //    Skip.If(true, "Test skipped because PhoneValidator doesn't accept the test format");
        //    return;
        //}
        
        var phone = PhoneNullableSafe.Create(validPhone);

        // Act & Assert
        phone.ToString().ShouldBe(validPhone);
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var phone = PhoneNullableSafe.Create(null);

        // Act & Assert
        phone.ToString().ShouldBe(string.Empty);
    }
}
