using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Setup;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class FileNameWithoutExtensionTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validFileName = "document";

        // Act
        var fileName = FileNameWithoutExtension.Create(validFileName);

        // Assert
        fileName.ShouldNotBeNull();
        fileName.Value.ShouldBe(validFileName);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string fileNameWithSpaces = "  document  ";

        // Act
        var fileName = FileNameWithoutExtension.Create(fileNameWithSpaces);

        // Assert
        fileName.Value.ShouldBe("document");
    }

    [Fact]
    public void Create_WithGuid_ShouldReturnInstance()
    {
        // Arrange
        Guid guid = Guid.NewGuid();

        // Act
        var fileName = FileNameWithoutExtension.Create(guid);

        // Assert
        fileName.ShouldNotBeNull();
        fileName.Value.ShouldBe(guid.ToString());
    }

    [Fact]
    public void Create_WithEmptyString_ShouldReturnInstance()
    {
        // Arrange
        string emptyFileName = "";

        // Act
        var fileName = FileNameWithoutExtension.Create(emptyFileName);

        // Assert
        fileName.ShouldNotBeNull();
        fileName.Value.ShouldBe(emptyFileName);
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongFileName = new('A', FileNameWithoutExtension.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            FileNameWithoutExtension.Create(tooLongFileName))
            .Property.ShouldBe(nameof(FileNameWithoutExtension));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthFileName = new('A', FileNameWithoutExtension.MaxLength);

        // Act
        var fileName = FileNameWithoutExtension.Create(maxLengthFileName);

        // Assert
        fileName.ShouldNotBeNull();
        fileName.Value.ShouldBe(maxLengthFileName);
    }

    [Fact]
    public void MaxLength_ShouldMatchDefaultValue()
    {
        // Assert
        FileNameWithoutExtension.MaxLength.ShouldBe(ValueObjectsSettings.MaxLengthFileName);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var fileName1 = FileNameWithoutExtension.Create("document");
        var fileName2 = FileNameWithoutExtension.Create("document");

        // Act & Assert
        fileName1.Equals(fileName2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var fileName1 = FileNameWithoutExtension.Create("document1");
        var fileName2 = FileNameWithoutExtension.Create("document2");

        // Act & Assert
        fileName1.Equals(fileName2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var fileName1 = FileNameWithoutExtension.Create("Document");
        var fileName2 = FileNameWithoutExtension.Create("document");

        // Act & Assert
        fileName1.Equals(fileName2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var fileName1 = FileNameWithoutExtension.Create("document");
        var fileName2 = FileNameWithoutExtension.Create("document");

        // Act & Assert
        fileName1.GetHashCode().ShouldBe(fileName2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string fileName = "document";
        var fileNameObj = FileNameWithoutExtension.Create(fileName);

        // Act & Assert
        fileNameObj.ToString().ShouldBe(fileName);
    }
}
