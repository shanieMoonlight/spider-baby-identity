using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Setup;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class FileUrlNullableTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validUrl = "https://example.com/files/document.pdf";

        // Act
        var fileUrl = FileUrlNullable.Create(validUrl);

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
        var fileUrl = FileUrlNullable.Create(urlWithSpaces);

        // Assert
        fileUrl.Value.ShouldBe(expectedUrl);
    }

    [Fact]
    public void Create_WithNullValue_ShouldNotThrowException()
    {
        // Arrange
        string? nullUrl = null;

        // Act & Assert
        Should.NotThrow(() => FileUrlNullable.Create(nullUrl));
    }

    [Fact]
    public void Create_WithNullValue_ShouldHaveNullValue()
    {
        // Arrange
        string? nullUrl = null;

        // Act
        var fileUrl = FileUrlNullable.Create(nullUrl);

        // Assert
        fileUrl.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyString_ShouldNotThrowException()
    {
        // Arrange
        string emptyUrl = "";

        // Act & Assert
        Should.NotThrow(() => FileUrlNullable.Create(emptyUrl));
    }

    [Fact]
    public void Create_WithEmptyString_ShouldHaveNullValue()
    {
        // Arrange
        string emptyUrl = "";

        // Act
        var fileUrl = FileUrlNullable.Create(emptyUrl);

        // Assert
        fileUrl.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldHaveNullValue()
    {
        // Arrange
        string whiteSpaceUrl = "   ";

        // Act
        var fileUrl = FileUrlNullable.Create(whiteSpaceUrl);

        // Assert
        fileUrl.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongUrl = "https://example.com/files/" + new string('a', FileUrlNullable.MaxLength);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            FileUrlNullable.Create(tooLongUrl))
            .Property.ShouldBe(nameof(FileUrlNullable));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string prefix = "https://example.com/files/";
        string maxLengthUrl = prefix + new string('a', FileUrlNullable.MaxLength - prefix.Length);

        // Act
        var fileUrl = FileUrlNullable.Create(maxLengthUrl);

        // Assert
        fileUrl.ShouldNotBeNull();
        fileUrl.Value.ShouldBe(maxLengthUrl);
    }        [Fact]
    public void MaxLength_ShouldMatchExpectedValue()
    {
        // Assert
        FileUrlNullable.MaxLength.ShouldBe(FileUrl.MaxLength);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var fileUrl1 = FileUrlNullable.Create("https://example.com/files/document.pdf");
        var fileUrl2 = FileUrlNullable.Create("https://example.com/files/document.pdf");

        // Act & Assert
        fileUrl1.Equals(fileUrl2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var fileUrl1 = FileUrlNullable.Create(null);
        var fileUrl2 = FileUrlNullable.Create(null);

        // Act & Assert
        fileUrl1.Equals(fileUrl2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var fileUrl1 = FileUrlNullable.Create(null);
        var fileUrl2 = FileUrlNullable.Create("https://example.com/files/document.pdf");

        // Act & Assert
        fileUrl1.Equals(fileUrl2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var fileUrl1 = FileUrlNullable.Create("https://example.com/files/document1.pdf");
        var fileUrl2 = FileUrlNullable.Create("https://example.com/files/document2.pdf");

        // Act & Assert
        fileUrl1.Equals(fileUrl2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var fileUrl1 = FileUrlNullable.Create("https://EXAMPLE.com/files/document.pdf");
        var fileUrl2 = FileUrlNullable.Create("https://example.com/files/document.pdf");

        // Act & Assert
        fileUrl1.Equals(fileUrl2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var fileUrl1 = FileUrlNullable.Create("https://example.com/files/document.pdf");
        var fileUrl2 = FileUrlNullable.Create("https://example.com/files/document.pdf");

        // Act & Assert
        fileUrl1.GetHashCode().ShouldBe(fileUrl2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var fileUrl1 = FileUrlNullable.Create(null);
        var fileUrl2 = FileUrlNullable.Create(null);

        // Act & Assert
        fileUrl1.GetHashCode().ShouldBe(fileUrl2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValue()
    {
        // Arrange
        string value = "https://example.com/files/document.pdf";
        var fileUrl = FileUrlNullable.Create(value);

        // Act & Assert
        fileUrl.ToString().ShouldBe(value);
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var fileUrl = FileUrlNullable.Create(null);

        // Act & Assert
        fileUrl.ToString().ShouldBe(string.Empty);
    }
}
