using ClArch.ValueObjects.Exceptions;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
public class FileUrlTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validUrl = "https://example.com/files/document.pdf";

        // Act
        var fileUrl = FileUrl.Create(validUrl);

        // Assert
        fileUrl.ShouldNotBeNull();
        fileUrl.Value.ShouldBe(validUrl);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string urlWithSpaces = "  https://example.com/files/document.pdf  ";
        string expectedUrl = "https://example.com/files/document.pdf";

        // Act
        var fileUrl = FileUrl.Create(urlWithSpaces);

        // Assert
        fileUrl.Value.ShouldBe(expectedUrl);
    }

    [Fact]
    public void Create_WithNullValue_ShouldThrowException()
    {
        // Arrange
        string? nullUrl = null;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            FileUrl.Create(nullUrl))
            .Property.ShouldBe(nameof(FileUrl));
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        string emptyUrl = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            FileUrl.Create(emptyUrl))
            .Property.ShouldBe(nameof(FileUrl));
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldThrowException()
    {
        // Arrange
        string whiteSpaceUrl = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            FileUrl.Create(whiteSpaceUrl))
            .Property.ShouldBe(nameof(FileUrl));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongUrl = "https://example.com/files/" + new string('a', FileUrl.MaxLength);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            FileUrl.Create(tooLongUrl))
            .Property.ShouldBe(nameof(FileUrlNullable));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string prefix = "https://example.com/files/";
        string maxLengthUrl = prefix + new string('a', FileUrl.MaxLength - prefix.Length);

        // Act
        var fileUrl = FileUrl.Create(maxLengthUrl);

        // Assert
        fileUrl.ShouldNotBeNull();
        fileUrl.Value.ShouldBe(maxLengthUrl);
    }        [Fact]
    public void MaxLength_ShouldHaveExpectedValue()
    {
        // Assert
        FileUrl.MaxLength.ShouldBePositive();
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var fileUrl1 = FileUrl.Create("https://example.com/files/document.pdf");
        var fileUrl2 = FileUrl.Create("https://example.com/files/document.pdf");

        // Act & Assert
        fileUrl1.Equals(fileUrl2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var fileUrl1 = FileUrl.Create("https://example.com/files/document1.pdf");
        var fileUrl2 = FileUrl.Create("https://example.com/files/document2.pdf");

        // Act & Assert
        fileUrl1.Equals(fileUrl2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var fileUrl1 = FileUrl.Create("https://EXAMPLE.com/files/document.pdf");
        var fileUrl2 = FileUrl.Create("https://example.com/files/document.pdf");

        // Act & Assert
        fileUrl1.Equals(fileUrl2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var fileUrl1 = FileUrl.Create("https://example.com/files/document.pdf");
        var fileUrl2 = FileUrl.Create("https://example.com/files/document.pdf");

        // Act & Assert
        fileUrl1.GetHashCode().ShouldBe(fileUrl2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string value = "https://example.com/files/document.pdf";
        var fileUrl = FileUrl.Create(value);

        // Act & Assert
        fileUrl.ToString().ShouldBe(value);
    }
}
