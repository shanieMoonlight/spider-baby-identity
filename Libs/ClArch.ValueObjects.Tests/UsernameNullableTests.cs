using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class UsernameNullableTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validUsername = "johndoe123";

        // Act
        var username = UsernameNullable.Create(validUsername);

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
        var username = UsernameNullable.Create(usernameWithSpaces);

        // Assert
        username.Value.ShouldBe(expectedUsername);
    }

    [Fact]
    public void Create_WithNullValue_ShouldNotThrowException()
    {
        // Arrange
        string? nullUsername = null;

        // Act & Assert
        Should.NotThrow(() => UsernameNullable.Create(nullUsername));
    }

    [Fact]
    public void Create_WithNullValue_ShouldHaveNullValue()
    {
        // Arrange
        string? nullUsername = null;

        // Act
        var username = UsernameNullable.Create(nullUsername);

        // Assert
        username.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyString_ShouldNotThrowException()
    {
        // Arrange
        string emptyUsername = "";

        // Act & Assert
        Should.NotThrow(() => UsernameNullable.Create(emptyUsername));
    }

    [Fact]
    public void Create_WithEmptyString_ShouldHaveNullValue()
    {
        // Arrange
        string emptyUsername = "";

        // Act
        var username = UsernameNullable.Create(emptyUsername);

        // Assert
        username.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldHaveNullValue()
    {
        // Arrange
        string whiteSpaceUsername = "   ";

        // Act
        var username = UsernameNullable.Create(whiteSpaceUsername);

        // Assert
        username.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongUsername = new('a', UsernameNullable.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            UsernameNullable.Create(tooLongUsername))
            .Property.ShouldBe(nameof(Username));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthUsername = new('a', UsernameNullable.MaxLength);

        // Act
        var username = UsernameNullable.Create(maxLengthUsername);

        // Assert
        username.ShouldNotBeNull();
        username.Value.ShouldBe(maxLengthUsername);
    }

    [Fact]
    public void MaxLength_ShouldMatchExpectedValue()
    {
        // Assert
        UsernameNullable.MaxLength.ShouldBe(Username.MaxLength);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var username1 = UsernameNullable.Create("johndoe123");
        var username2 = UsernameNullable.Create("johndoe123");

        // Act & Assert
        username1.Equals(username2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var username1 = UsernameNullable.Create(null);
        var username2 = UsernameNullable.Create(null);

        // Act & Assert
        username1.Equals(username2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var username1 = UsernameNullable.Create(null);
        var username2 = UsernameNullable.Create("johndoe123");

        // Act & Assert
        username1.Equals(username2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var username1 = UsernameNullable.Create("johndoe123");
        var username2 = UsernameNullable.Create("janedoe456");

        // Act & Assert
        username1.Equals(username2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var username1 = UsernameNullable.Create("JohnDoe123");
        var username2 = UsernameNullable.Create("johndoe123");

        // Act & Assert
        username1.Equals(username2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var username1 = UsernameNullable.Create("johndoe123");
        var username2 = UsernameNullable.Create("johndoe123");

        // Act & Assert
        username1.GetHashCode().ShouldBe(username2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var username1 = UsernameNullable.Create(null);
        var username2 = UsernameNullable.Create(null);

        // Act & Assert
        username1.GetHashCode().ShouldBe(username2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValue()
    {
        // Arrange
        string value = "johndoe123";
        var username = UsernameNullable.Create(value);

        // Act & Assert
        username.ToString().ShouldBe(value);
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var username = UsernameNullable.Create(null);

        // Act & Assert
        username.ToString().ShouldBe(string.Empty);
    }
}
