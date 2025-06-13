using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
public class ImgUrlTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validUrl = "https://example.com/images/photo.jpg";

        // Act
        var imgUrl = ImgUrl.Create(validUrl);

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
        var imgUrl = ImgUrl.Create(urlWithSpaces);

        // Assert
        imgUrl.Value.ShouldBe(expectedUrl);
    }

    [Fact]
    public void Create_WithNullValue_ShouldThrowException()
    {
        // Arrange
        string? nullUrl = null;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            ImgUrl.Create(nullUrl))
            .Property.ShouldBe(nameof(ImgUrl));
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        string emptyUrl = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            ImgUrl.Create(emptyUrl))
            .Property.ShouldBe(nameof(ImgUrl));
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldThrowException()
    {
        // Arrange
        string whiteSpaceUrl = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            ImgUrl.Create(whiteSpaceUrl))
            .Property.ShouldBe(nameof(ImgUrl));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongUrl = "https://example.com/images/" + new string('a', ImgUrl.MaxLength);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            ImgUrl.Create(tooLongUrl))
            .Property.ShouldBe(nameof(ImgUrlNullable));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string prefix = "https://example.com/images/";
        string maxLengthUrl = prefix + new string('a', ImgUrl.MaxLength - prefix.Length);

        // Act
        var imgUrl = ImgUrl.Create(maxLengthUrl);

        // Assert
        imgUrl.ShouldNotBeNull();
        imgUrl.Value.ShouldBe(maxLengthUrl);
    }

    [Fact]
    public void MaxLength_ShouldHaveExpectedValue()
    {
        // Assert
        ImgUrl.MaxLength.ShouldBePositive();
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var imgUrl1 = ImgUrl.Create("https://example.com/images/photo.jpg");
        var imgUrl2 = ImgUrl.Create("https://example.com/images/photo.jpg");

        // Act & Assert
        imgUrl1.Equals(imgUrl2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var imgUrl1 = ImgUrl.Create("https://example.com/images/photo1.jpg");
        var imgUrl2 = ImgUrl.Create("https://example.com/images/photo2.jpg");

        // Act & Assert
        imgUrl1.Equals(imgUrl2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var imgUrl1 = ImgUrl.Create("https://EXAMPLE.com/images/photo.jpg");
        var imgUrl2 = ImgUrl.Create("https://example.com/images/photo.jpg");

        // Act & Assert
        imgUrl1.Equals(imgUrl2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var imgUrl1 = ImgUrl.Create("https://example.com/images/photo.jpg");
        var imgUrl2 = ImgUrl.Create("https://example.com/images/photo.jpg");

        // Act & Assert
        imgUrl1.GetHashCode().ShouldBe(imgUrl2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string value = "https://example.com/images/photo.jpg";
        var imgUrl = ImgUrl.Create(value);

        // Act & Assert
        imgUrl.ToString().ShouldBe(value);
    }
}
