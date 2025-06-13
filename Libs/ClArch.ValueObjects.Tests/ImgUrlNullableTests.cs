using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class ImgUrlNullableTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validUrl = "https://example.com/images/photo.jpg";

        // Act
        var imgUrl = ImgUrlNullable.Create(validUrl);

        // Assert
        imgUrl.ShouldNotBeNull();
        imgUrl.Value.ShouldBe(validUrl);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string urlWithSpaces = "  https://example.com/images/photo.jpg  ";
        string expectedUrl = "https://example.com/images/photo.jpg";

        // Act
        var imgUrl = ImgUrlNullable.Create(urlWithSpaces);

        // Assert
        imgUrl.Value.ShouldBe(expectedUrl);
    }

    [Fact]
    public void Create_WithNullValue_ShouldNotThrowException()
    {
        // Arrange
        string? nullUrl = null;

        // Act & Assert
        Should.NotThrow(() => ImgUrlNullable.Create(nullUrl));
    }

    [Fact]
    public void Create_WithNullValue_ShouldHaveNullValue()
    {
        // Arrange
        string? nullUrl = null;

        // Act
        var imgUrl = ImgUrlNullable.Create(nullUrl);

        // Assert
        imgUrl.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyString_ShouldNotThrowException()
    {
        // Arrange
        string emptyUrl = "";

        // Act & Assert
        Should.NotThrow(() => ImgUrlNullable.Create(emptyUrl));
    }

    [Fact]
    public void Create_WithEmptyString_ShouldHaveNullValue()
    {
        // Arrange
        string emptyUrl = "";

        // Act
        var imgUrl = ImgUrlNullable.Create(emptyUrl);

        // Assert
        imgUrl.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldHaveNullValue()
    {
        // Arrange
        string whiteSpaceUrl = "   ";

        // Act
        var imgUrl = ImgUrlNullable.Create(whiteSpaceUrl);

        // Assert
        imgUrl.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongUrl = "https://example.com/images/" + new string('a', ImgUrlNullable.MaxLength);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            ImgUrlNullable.Create(tooLongUrl))
            .Property.ShouldBe(nameof(ImgUrlNullable));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string prefix = "https://example.com/images/";
        string maxLengthUrl = prefix + new string('a', ImgUrlNullable.MaxLength - prefix.Length);

        // Act
        var imgUrl = ImgUrlNullable.Create(maxLengthUrl);

        // Assert
        imgUrl.ShouldNotBeNull();
        imgUrl.Value.ShouldBe(maxLengthUrl);
    }

    [Fact]
    public void MaxLength_ShouldMatchExpectedValue()
    {
        // Assert
        ImgUrlNullable.MaxLength.ShouldBe(ImgUrl.MaxLength);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var imgUrl1 = ImgUrlNullable.Create("https://example.com/images/photo.jpg");
        var imgUrl2 = ImgUrlNullable.Create("https://example.com/images/photo.jpg");

        // Act & Assert
        imgUrl1.Equals(imgUrl2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var imgUrl1 = ImgUrlNullable.Create(null);
        var imgUrl2 = ImgUrlNullable.Create(null);

        // Act & Assert
        imgUrl1.Equals(imgUrl2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var imgUrl1 = ImgUrlNullable.Create(null);
        var imgUrl2 = ImgUrlNullable.Create("https://example.com/images/photo.jpg");

        // Act & Assert
        imgUrl1.Equals(imgUrl2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var imgUrl1 = ImgUrlNullable.Create("https://example.com/images/photo1.jpg");
        var imgUrl2 = ImgUrlNullable.Create("https://example.com/images/photo2.jpg");

        // Act & Assert
        imgUrl1.Equals(imgUrl2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var imgUrl1 = ImgUrlNullable.Create("https://EXAMPLE.com/images/photo.jpg");
        var imgUrl2 = ImgUrlNullable.Create("https://example.com/images/photo.jpg");

        // Act & Assert
        imgUrl1.Equals(imgUrl2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var imgUrl1 = ImgUrlNullable.Create("https://example.com/images/photo.jpg");
        var imgUrl2 = ImgUrlNullable.Create("https://example.com/images/photo.jpg");

        // Act & Assert
        imgUrl1.GetHashCode().ShouldBe(imgUrl2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var imgUrl1 = ImgUrlNullable.Create(null);
        var imgUrl2 = ImgUrlNullable.Create(null);

        // Act & Assert
        imgUrl1.GetHashCode().ShouldBe(imgUrl2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValue()
    {
        // Arrange
        string value = "https://example.com/images/photo.jpg";
        var imgUrl = ImgUrlNullable.Create(value);

        // Act & Assert
        imgUrl.ToString().ShouldBe(value);
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var imgUrl = ImgUrlNullable.Create(null);

        // Act & Assert
        imgUrl.ToString().ShouldBe(string.Empty);
    }
}
