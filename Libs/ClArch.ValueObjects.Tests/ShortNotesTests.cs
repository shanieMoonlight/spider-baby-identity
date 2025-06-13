using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
public class ShortNotesTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validNotes = "This is a short note";

        // Act
        var notes = ShortNotes.Create(validNotes);

        // Assert
        notes.ShouldNotBeNull();
        notes.Value.ShouldBe(validNotes);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string notesWithSpaces = "  This is a short note  ";
        string expectedNotes = "This is a short note";

        // Act
        var notes = ShortNotes.Create(notesWithSpaces);

        // Assert
        notes.Value.ShouldBe(expectedNotes);
    }

    [Fact]
    public void Create_WithNullValue_ShouldThrowException()
    {
        // Arrange
        string? nullNotes = null;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            ShortNotes.Create(nullNotes))
            .Property.ShouldBe(nameof(ShortNotes));
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        string emptyNotes = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            ShortNotes.Create(emptyNotes))
            .Property.ShouldBe(nameof(ShortNotes));
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldThrowException()
    {
        // Arrange
        string whiteSpaceNotes = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            ShortNotes.Create(whiteSpaceNotes))
            .Property.ShouldBe(nameof(ShortNotes));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongNotes = new('a', ShortNotes.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            ShortNotes.Create(tooLongNotes))
            .Property.ShouldBe(nameof(ShortNotes));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthNotes = new('a', ShortNotes.MaxLength);

        // Act
        var notes = ShortNotes.Create(maxLengthNotes);

        // Assert
        notes.ShouldNotBeNull();
        notes.Value.ShouldBe(maxLengthNotes);
    }

    [Fact]
    public void MaxLength_ShouldHaveExpectedValue()
    {
        // Assert
        ShortNotes.MaxLength.ShouldBePositive();
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var notes1 = ShortNotes.Create("This is a short note");
        var notes2 = ShortNotes.Create("This is a short note");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var notes1 = ShortNotes.Create("This is a short note");
        var notes2 = ShortNotes.Create("This is a different note");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var notes1 = ShortNotes.Create("This is a SHORT note");
        var notes2 = ShortNotes.Create("This is a short note");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var notes1 = ShortNotes.Create("This is a short note");
        var notes2 = ShortNotes.Create("This is a short note");

        // Act & Assert
        notes1.GetHashCode().ShouldBe(notes2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string value = "This is a short note";
        var notes = ShortNotes.Create(value);

        // Act & Assert
        notes.ToString().ShouldBe(value);
    }
}
