using ClArch.ValueObjects.Exceptions;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
public class UsernameTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validUsername = "johndoe123";

        // Act
        var username = Username.Create(validUsername);

        // Assert
        username.ShouldNotBeNull();
        username.Value.ShouldBe(validUsername);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string usernameWithSpaces = "  johndoe123  ";
        string expectedUsername = "johndoe123";

        // Act
        var username = Username.Create(usernameWithSpaces);

        // Assert
        username.Value.ShouldBe(expectedUsername);
    }

    [Fact]
    public void Create_WithNullValue_ShouldThrowException()
    {
        // Arrange
        string? nullUsername = null;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            Username.Create(nullUsername))
            .Property.ShouldBe(nameof(Username));
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        string emptyUsername = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            Username.Create(emptyUsername))
            .Property.ShouldBe(nameof(Username));
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldThrowException()
    {
        // Arrange
        string whiteSpaceUsername = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            Username.Create(whiteSpaceUsername))
            .Property.ShouldBe(nameof(Username));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongUsername = new('a', Username.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            Username.Create(tooLongUsername))
            .Property.ShouldBe(nameof(Username));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthUsername = new('a', Username.MaxLength);

        // Act
        var username = Username.Create(maxLengthUsername);

        // Assert
        username.ShouldNotBeNull();
        username.Value.ShouldBe(maxLengthUsername);
    }

    [Fact]
    public void MaxLength_ShouldHaveExpectedValue()
    {
        // Assert
        Username.MaxLength.ShouldBePositive();
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var username1 = Username.Create("johndoe123");
        var username2 = Username.Create("johndoe123");

        // Act & Assert
        username1.Equals(username2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var username1 = Username.Create("johndoe123");
        var username2 = Username.Create("janedoe456");

        // Act & Assert
        username1.Equals(username2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var username1 = Username.Create("JohnDoe123");
        var username2 = Username.Create("johndoe123");

        // Act & Assert
        username1.Equals(username2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var username1 = Username.Create("johndoe123");
        var username2 = Username.Create("johndoe123");

        // Act & Assert
        username1.GetHashCode().ShouldBe(username2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string value = "johndoe123";
        var username = Username.Create(value);

        // Act & Assert
        username.ToString().ShouldBe(value);
    }
}
