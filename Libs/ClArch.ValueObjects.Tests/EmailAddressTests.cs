using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Setup;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class EmailAddressTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldReturnInstance()
    {
        // Arrange
        string validEmail = "test@example.com";

        // Act
        var email = EmailAddress.Create(validEmail);

        // Assert
        email.ShouldNotBeNull();
        email.Value.ShouldBe(validEmail);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string emailWithSpaces = "  test@example.com  ";

        // Act
        var email = EmailAddress.Create(emailWithSpaces);

        // Assert
        email.Value.ShouldBe("test@example.com");
    }

    [Fact]
    public void Create_WithUppercaseLetters_ShouldConvertToLowercase()
    {
        // Arrange
        string uppercaseEmail = "TEST@EXAMPLE.COM";

        // Act
        var email = EmailAddress.Create(uppercaseEmail);

        // Assert
        email.Value.ShouldBe("test@example.com");
    }

    [Fact]
    public void Create_WithNullValue_ShouldThrowException()
    {
        // Arrange
        string nullEmail = null;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            EmailAddress.Create(nullEmail))
            .Property.ShouldBe(nameof(EmailAddress));
    }

    [Fact]
    public void Create_WithEmptyString_ShouldThrowException()
    {
        // Arrange
        string emptyEmail = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            EmailAddress.Create(emptyEmail))
            .Property.ShouldBe(nameof(EmailAddress));
    }

    [Fact]
    public void Create_WithWhitespaceOnly_ShouldThrowException()
    {
        // Arrange
        string whitespaceEmail = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            EmailAddress.Create(whitespaceEmail))
            .Property.ShouldBe(nameof(EmailAddress));
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldThrowException()
    {
        // Arrange
        string invalidEmail = "not-an-email";

        // Act & Assert
        Should.Throw<InvalidPropertyException>(() => 
            EmailAddress.Create(invalidEmail))
            .Property.ShouldBe(nameof(EmailAddressNullable));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongEmail = $"{new string('a', ValueObjectsDefaultValues.MAX_LENGTH_EMAIL - 10)}@example.com";
        if (tooLongEmail.Length <= ValueObjectsDefaultValues.MAX_LENGTH_EMAIL)
        {
            tooLongEmail = $"{new string('a', ValueObjectsDefaultValues.MAX_LENGTH_EMAIL)}@example.com";
        }

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            EmailAddress.Create(tooLongEmail))
            .Property.ShouldBe(nameof(EmailAddress));
    }

    [Fact]
    public void MaxLength_ShouldMatchDefaultValue()
    {
        // Arrange & Act & Assert
        EmailAddress.MaxLength.ShouldBe(ValueObjectsDefaultValues.MAX_LENGTH_EMAIL);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var email1 = EmailAddress.Create("test@example.com");
        var email2 = EmailAddress.Create("test@example.com");

        // Act & Assert
        email1.Equals(email2).ShouldBeTrue();
        (email1 == email2).ShouldBeTrue();
        (email1 != email2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var email1 = EmailAddress.Create("test@example.com");
        var email2 = EmailAddress.Create("other@example.com");

        // Act & Assert
        email1.Equals(email2).ShouldBeFalse();
        (email1 == email2).ShouldBeFalse();
        (email1 != email2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var email1 = EmailAddress.Create("test@example.com");
        var email2 = EmailAddress.Create("TEST@EXAMPLE.COM");

        // Act & Assert
        // Both will be converted to lowercase in Create method
        email1.Equals(email2).ShouldBeTrue();
        (email1 == email2).ShouldBeTrue();
        (email1 != email2).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var email1 = EmailAddress.Create("test@example.com");
        var email2 = EmailAddress.Create("test@example.com");

        // Act & Assert
        email1.GetHashCode().ShouldBe(email2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var email = EmailAddress.Create("test@example.com");

        // Act & Assert
        email.ToString().ShouldBe("test@example.com");
    }
}
