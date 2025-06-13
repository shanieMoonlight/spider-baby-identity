using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Setup;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class EmailAddressNullableTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldReturnInstance()
    {
        // Arrange
        string validEmail = "test@example.com";

        // Act
        var email = EmailAddressNullable.Create(validEmail);

        // Assert
        email.ShouldNotBeNull();
        email.Value.ShouldBe(validEmail);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnInstanceWithNullValue()
    {
        // Arrange
        string nullEmail = null;

        // Act
        var email = EmailAddressNullable.Create(nullEmail);

        // Assert
        email.ShouldNotBeNull();
        email.Value.ShouldBeNull();
    }        [Fact]
    public void Create_WithEmptyString_ShouldNotThrowException()
    {
        // Arrange
        string emptyEmail = "";

        // Act & Assert
        // Empty string should be considered valid for nullable email
        Should.NotThrow(() => EmailAddressNullable.Create(emptyEmail));
    }        [Fact]
    public void Create_WithWhitespaceOnly_ShouldNotThrowException()
    {
        // Arrange
        string whitespaceEmail = "   ";

        // Act & Assert
        // Whitespace should be considered valid for nullable email
        Should.NotThrow(() => EmailAddressNullable.Create(whitespaceEmail));
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string emailWithSpaces = "  test@example.com  ";

        // Act
        var email = EmailAddressNullable.Create(emailWithSpaces);

        // Assert
        email.Value.ShouldBe("test@example.com");
    }

    [Fact]
    public void Create_WithUppercaseLetters_ShouldConvertToLowercase()
    {
        // Arrange
        string uppercaseEmail = "TEST@EXAMPLE.COM";

        // Act
        var email = EmailAddressNullable.Create(uppercaseEmail);

        // Assert
        email.Value.ShouldBe("test@example.com");
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldThrowException()
    {
        // Arrange
        string invalidEmail = "not-an-email";

        // Act & Assert
        Should.Throw<InvalidPropertyException>(() => 
            EmailAddressNullable.Create(invalidEmail))
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
            EmailAddressNullable.Create(tooLongEmail))
            .Property.ShouldBe(nameof(EmailAddress));
    }

    [Fact]
    public void MaxLength_ShouldMatchDefaultValue()
    {
        // Arrange & Act & Assert
        EmailAddressNullable.MaxLength.ShouldBe(ValueObjectsDefaultValues.MAX_LENGTH_EMAIL);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var email1 = EmailAddressNullable.Create("test@example.com");
        var email2 = EmailAddressNullable.Create("test@example.com");

        // Act & Assert
        email1.Equals(email2).ShouldBeTrue();
        (email1 == email2).ShouldBeTrue();
        (email1 != email2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var email1 = EmailAddressNullable.Create("test@example.com");
        var email2 = EmailAddressNullable.Create("other@example.com");

        // Act & Assert
        email1.Equals(email2).ShouldBeFalse();
        (email1 == email2).ShouldBeFalse();
        (email1 != email2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var email1 = EmailAddressNullable.Create(null);
        var email2 = EmailAddressNullable.Create(null);

        // Act & Assert
        email1.Equals(email2).ShouldBeTrue();
        (email1 == email2).ShouldBeTrue();
        (email1 != email2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var email1 = EmailAddressNullable.Create(null);
        var email2 = EmailAddressNullable.Create("test@example.com");

        // Act & Assert
        email1.Equals(email2).ShouldBeFalse();
        (email1 == email2).ShouldBeFalse();
        (email1 != email2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var email1 = EmailAddressNullable.Create("test@example.com");
        var email2 = EmailAddressNullable.Create("test@example.com");

        // Act & Assert
        email1.GetHashCode().ShouldBe(email2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var email1 = EmailAddressNullable.Create(null);
        var email2 = EmailAddressNullable.Create(null);

        // Act & Assert
        email1.GetHashCode().ShouldBe(email2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValue()
    {
        // Arrange
        var email = EmailAddressNullable.Create("test@example.com");

        // Act & Assert
        email.ToString().ShouldBe("test@example.com");
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var email = EmailAddressNullable.Create(null);

        // Act & Assert
        email.ToString().ShouldBe(string.Empty);
    }
}
