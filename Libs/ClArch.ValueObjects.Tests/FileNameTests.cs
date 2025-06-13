using ClArch.ValueObjects;
using Shouldly;
using Xunit;
using System;

namespace ClArch.ValueObjects.Tests;

public class FileNameTests
{
    [Fact]
    public void Create_WithValidString_ShouldReturnInstance()
    {
        // Arrange
        string validFileName = "document.pdf";

        // Act
        var fileName = FileName.Create(validFileName);

        // Assert
        fileName.ShouldNotBeNull();
        fileName.Value.ShouldBe(validFileName);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string fileNameWithSpaces = "  document.pdf  ";

        // Act
        var fileName = FileName.Create(fileNameWithSpaces);

        // Assert
        fileName.Value.ShouldBe("document.pdf");
    }

    [Fact]
    public void Create_WithEmptyString_ShouldAcceptIt()
    {
        // Arrange
        string emptyFileName = "";

        // Act
        var fileName = FileName.Create(emptyFileName);

        // Assert
        fileName.Value.ShouldBe("");
    }

    [Fact]
    public void Create_WithGuid_ShouldReturnInstanceWithStringifiedGuid()
    {
        // Arrange
        Guid guid = Guid.NewGuid();

        // Act
        var fileName = FileName.Create(guid);

        // Assert
        fileName.ShouldNotBeNull();
        fileName.Value.ShouldBe(guid.ToString());
    }

    [Fact]
    public void Create_WithEmptyGuid_ShouldReturnInstanceWithEmptyGuidString()
    {
        // Arrange
        Guid emptyGuid = Guid.Empty;

        // Act
        var fileName = FileName.Create(emptyGuid);

        // Assert
        fileName.Value.ShouldBe(emptyGuid.ToString());
        fileName.Value.ShouldBe("00000000-0000-0000-0000-000000000000");
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var fileName1 = FileName.Create("document.pdf");
        var fileName2 = FileName.Create("document.pdf");

        // Act & Assert
        fileName1.Equals(fileName2).ShouldBeTrue();
        (fileName1 == fileName2).ShouldBeTrue();
        (fileName1 != fileName2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var fileName1 = FileName.Create("document.pdf");
        var fileName2 = FileName.Create("image.jpg");

        // Act & Assert
        fileName1.Equals(fileName2).ShouldBeFalse();
        (fileName1 == fileName2).ShouldBeFalse();
        (fileName1 != fileName2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var fileName1 = FileName.Create("document.pdf");
        var fileName2 = FileName.Create("DOCUMENT.PDF");

        // Act & Assert
        // StrValueObject uses case-insensitive comparison
        fileName1.Equals(fileName2).ShouldBeTrue();
        (fileName1 == fileName2).ShouldBeTrue();
        (fileName1 != fileName2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_BothFromSameGuid_ShouldBeTrue()
    {
        // Arrange
        Guid guid = Guid.NewGuid();
        var fileName1 = FileName.Create(guid);
        var fileName2 = FileName.Create(guid);

        // Act & Assert
        fileName1.Equals(fileName2).ShouldBeTrue();
        (fileName1 == fileName2).ShouldBeTrue();
        (fileName1 != fileName2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_FromDifferentGuids_ShouldBeFalse()
    {
        // Arrange
        var fileName1 = FileName.Create(Guid.NewGuid());
        var fileName2 = FileName.Create(Guid.NewGuid());

        // Act & Assert
        fileName1.Equals(fileName2).ShouldBeFalse();
        (fileName1 == fileName2).ShouldBeFalse();
        (fileName1 != fileName2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var fileName1 = FileName.Create("document.pdf");
        var fileName2 = FileName.Create("document.pdf");

        // Act & Assert
        fileName1.GetHashCode().ShouldBe(fileName2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var fileName = FileName.Create("document.pdf");

        // Act & Assert
        fileName.ToString().ShouldBe("document.pdf");
    }

    [Fact]
    public void ToString_FromGuid_ShouldReturnStringifiedGuid()
    {
        // Arrange
        Guid guid = Guid.NewGuid();
        var fileName = FileName.Create(guid);

        // Act & Assert
        fileName.ToString().ShouldBe(guid.ToString());
    }
}
