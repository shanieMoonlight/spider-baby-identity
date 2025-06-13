using ClArch.ValueObjects.Setup;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class FileContentTypeTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validContentType = "image/jpeg";

        // Act
        var contentType = FileContentType.Create(validContentType);

        // Assert
        contentType.ShouldNotBeNull();
        contentType.Value.ShouldBe(validContentType);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string contentTypeWithSpaces = "  image/jpeg  ";

        // Act
        var contentType = FileContentType.Create(contentTypeWithSpaces);

        // Assert
        contentType.Value.ShouldBe("image/jpeg");
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnUnknown()
    {
        // Arrange
        string nullContentType = null;

        // Act
        var contentType = FileContentType.Create(nullContentType);

        // Assert
        contentType.ShouldNotBeNull();
        contentType.Value.ShouldBe(ValueObjectsDefaultValues.FILE_CONTENT_TYPE);
    }

    [Fact]
    public void Create_WithEmptyString_ShouldTreatAsIs()
    {
        // Arrange
        string emptyContentType = "";

        // Act
        var contentType = FileContentType.Create(emptyContentType);

        // Assert
        contentType.Value.ShouldBe("");
    }

    [Fact]
    public void Unknown_ShouldReturnUnknownContentType()
    {
        // Act
        var contentType = FileContentType.Unknown();

        // Assert
        contentType.ShouldNotBeNull();
        contentType.Value.ShouldBe(ValueObjectsDefaultValues.FILE_CONTENT_TYPE);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var contentType1 = FileContentType.Create("image/jpeg");
        var contentType2 = FileContentType.Create("image/jpeg");

        // Act & Assert
        contentType1.Equals(contentType2).ShouldBeTrue();
        (contentType1 == contentType2).ShouldBeTrue();
        (contentType1 != contentType2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var contentType1 = FileContentType.Create("image/jpeg");
        var contentType2 = FileContentType.Create("image/png");

        // Act & Assert
        contentType1.Equals(contentType2).ShouldBeFalse();
        (contentType1 == contentType2).ShouldBeFalse();
        (contentType1 != contentType2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var contentType1 = FileContentType.Create("image/jpeg");
        var contentType2 = FileContentType.Create("IMAGE/JPEG");

        // Act & Assert
        // StrValueObject uses case-insensitive comparison
        contentType1.Equals(contentType2).ShouldBeTrue();
        (contentType1 == contentType2).ShouldBeTrue();
        (contentType1 != contentType2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_UnknownValues_ShouldBeEqual()
    {
        // Arrange
        var contentType1 = FileContentType.Unknown();
        var contentType2 = FileContentType.Create(null);

        // Act & Assert
        contentType1.Equals(contentType2).ShouldBeTrue();
        (contentType1 == contentType2).ShouldBeTrue();
        (contentType1 != contentType2).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var contentType1 = FileContentType.Create("image/jpeg");
        var contentType2 = FileContentType.Create("image/jpeg");

        // Act & Assert
        contentType1.GetHashCode().ShouldBe(contentType2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var contentType = FileContentType.Create("image/jpeg");

        // Act & Assert
        contentType.ToString().ShouldBe("image/jpeg");
    }
}
