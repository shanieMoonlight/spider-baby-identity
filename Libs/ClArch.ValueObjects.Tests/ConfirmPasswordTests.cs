using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Setup;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

public class ConfirmPasswordTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validPassword = "Password123!";

        // Act
        var confirmPassword = ConfirmPassword.Create(validPassword);

        // Assert
        confirmPassword.ShouldNotBeNull();
        confirmPassword.Value.ShouldBe(validPassword);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string passwordWithSpaces = "  Password123!  ";

        // Act
        var confirmPassword = ConfirmPassword.Create(passwordWithSpaces);

        // Assert
        confirmPassword.Value.ShouldBe("Password123!");
    }

    [Fact]
    public void Create_WithNullValue_ShouldThrowException()
    {
        // Arrange
        string nullPassword = null;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            ConfirmPassword.Create(nullPassword))
            .Property.ShouldBe(nameof(ConfirmPassword));
    }

    [Fact]
    public void Create_WithEmptyString_ShouldThrowException()
    {
        // Arrange
        string emptyPassword = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            ConfirmPassword.Create(emptyPassword))
            .Property.ShouldBe(nameof(ConfirmPassword));
    }

    [Fact]
    public void Create_WithWhitespaceOnly_ShouldThrowException()
    {
        // Arrange
        string whitespacePassword = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            ConfirmPassword.Create(whitespacePassword))
            .Property.ShouldBe(nameof(ConfirmPassword));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongPassword = new('A', ValueObjectsDefaultValues.MAX_LENGTH_PWD + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            ConfirmPassword.Create(tooLongPassword))
            .Property.ShouldBe(nameof(ConfirmPassword));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthPassword = new('A', ValueObjectsDefaultValues.MAX_LENGTH_PWD);

        // Act
        var confirmPassword = ConfirmPassword.Create(maxLengthPassword);

        // Assert
        confirmPassword.ShouldNotBeNull();
        confirmPassword.Value.ShouldBe(maxLengthPassword);
    }

    [Fact]
    public void MaxLength_ShouldMatchDefaultValue()
    {
        // Arrange & Act & Assert
        ConfirmPassword.MaxLength.ShouldBe(ValueObjectsDefaultValues.MAX_LENGTH_PWD);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var password1 = ConfirmPassword.Create("Password123!");
        var password2 = ConfirmPassword.Create("Password123!");

        // Act & Assert
        password1.Equals(password2).ShouldBeTrue();
        (password1 == password2).ShouldBeTrue();
        (password1 != password2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var password1 = ConfirmPassword.Create("Password123!");
        var password2 = ConfirmPassword.Create("DifferentPassword456!");

        // Act & Assert
        password1.Equals(password2).ShouldBeFalse();
        (password1 == password2).ShouldBeFalse();
        (password1 != password2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeFalse()
    {
        // Arrange
        var password1 = ConfirmPassword.Create("Password123!");
        var password2 = ConfirmPassword.Create("PASSWORD123!");

        // Act & Assert
        // StrValueObject uses case-insensitive comparison
        password1.Equals(password2).ShouldBeFalse();
        (password1 == password2).ShouldBeFalse();
        (password1 != password2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var password1 = ConfirmPassword.Create("Password123!");
        var password2 = ConfirmPassword.Create("Password123!");

        // Act & Assert
        password1.GetHashCode().ShouldBe(password2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var password = ConfirmPassword.Create("Password123!");

        // Act & Assert
        password.ToString().ShouldBe("Password123!");
    }
}
