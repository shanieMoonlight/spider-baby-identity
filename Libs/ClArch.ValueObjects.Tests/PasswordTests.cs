using ClArch.ValueObjects.Exceptions;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
public class PasswordTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validPassword = "SecurePassword123!";

        // Act
        var password = Password.Create(validPassword);

        // Assert
        password.ShouldNotBeNull();
        password.Value.ShouldBe(validPassword);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string passwordWithSpaces = "  SecurePassword123!  ";
        string expectedPassword = "SecurePassword123!";

        // Act
        var password = Password.Create(passwordWithSpaces);

        // Assert
        password.Value.ShouldBe(expectedPassword);
    }

    [Fact]
    public void Create_WithNullValue_ShouldThrowException()
    {
        // Arrange
        string? nullPassword = null;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            Password.Create(nullPassword))
            .Property.ShouldBe(nameof(Password));
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        string emptyPassword = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            Password.Create(emptyPassword))
            .Property.ShouldBe(nameof(Password));
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldThrowException()
    {
        // Arrange
        string whiteSpacePassword = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            Password.Create(whiteSpacePassword))
            .Property.ShouldBe(nameof(Password));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongPassword = new('a', Password.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            Password.Create(tooLongPassword))
            .Property.ShouldBe(nameof(Password));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthPassword = new('a', Password.MaxLength);

        // Act
        var password = Password.Create(maxLengthPassword);

        // Assert
        password.ShouldNotBeNull();
        password.Value.ShouldBe(maxLengthPassword);
    }

    [Fact]
    public void MaxLength_ShouldHaveExpectedValue()
    {
        // Assert
        Password.MaxLength.ShouldBePositive();
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var password1 = Password.Create("SecurePassword123!");
        var password2 = Password.Create("SecurePassword123!");

        // Act & Assert
        password1.Equals(password2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var password1 = Password.Create("SecurePassword123!");
        var password2 = Password.Create("DifferentPassword456@");

        // Act & Assert
        password1.Equals(password2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeFalse()
    {
        // Arrange
        var password1 = Password.Create("SecurePassword123!");
        var password2 = Password.Create("SECUREPASSWORD123!");

        // Act & Assert
        // Password is case-sensitive (unlike invariant string value objects)
        password1.Equals(password2).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var password1 = Password.Create("SecurePassword123!");
        var password2 = Password.Create("SecurePassword123!");

        // Act & Assert
        password1.GetHashCode().ShouldBe(password2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string value = "SecurePassword123!";
        var password = Password.Create(value);

        // Act & Assert
        password.ToString().ShouldBe(value);
    }
}
