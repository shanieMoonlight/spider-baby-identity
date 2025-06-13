using ClArch.ValueObjects.Exceptions;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
public class LongTruncateNotesTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validNotes = "These are some valid long notes with detailed content.";

        // Act
        var notes = LongTruncateNotes.Create(validNotes);

        // Assert
        notes.ShouldNotBeNull();
        notes.Value.ShouldBe(validNotes);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string notesWithSpaces = "  These are some valid long notes with detailed content.  ";
        string expectedNotes = "These are some valid long notes with detailed content.";

        // Act
        var notes = LongTruncateNotes.Create(notesWithSpaces);

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
            LongTruncateNotes.Create(nullNotes))
            .Property.ShouldBe(nameof(LongNotes));
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        string emptyNotes = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            LongTruncateNotes.Create(emptyNotes))
            .Property.ShouldBe(nameof(LongNotes));
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldThrowException()
    {
        // Arrange
        string whiteSpaceNotes = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            LongTruncateNotes.Create(whiteSpaceNotes))
            .Property.ShouldBe(nameof(LongNotes));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldTruncateValue()
    {
        // Arrange
        string tooLongNotes = new('a', LongTruncateNotes.MaxLength + 100);
        string expectedNotes = new string('a', LongTruncateNotesNullable.MaxLength - 3) + "...";

        // Act
        var notes = LongTruncateNotes.Create(tooLongNotes);

        // Assert
        notes.Value.Length.ShouldBe(LongTruncateNotes.MaxLength);
        notes.Value.ShouldBe(expectedNotes);
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthNotes = new('a', LongTruncateNotes.MaxLength);

        // Act
        var notes = LongTruncateNotes.Create(maxLengthNotes);

        // Assert
        notes.ShouldNotBeNull();
        notes.Value.ShouldBe(maxLengthNotes);
    }

    [Fact]
    public void MaxLength_ShouldMatchExpectedValue()
    {
        // Assert
        LongTruncateNotes.MaxLength.ShouldBe(LongNotes.MaxLength);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var notes1 = LongTruncateNotes.Create("These are some notes.");
        var notes2 = LongTruncateNotes.Create("These are some notes.");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var notes1 = LongTruncateNotes.Create("These are some notes.");
        var notes2 = LongTruncateNotes.Create("These are different notes.");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var notes1 = LongTruncateNotes.Create("These are SOME notes.");
        var notes2 = LongTruncateNotes.Create("These are some notes.");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var notes1 = LongTruncateNotes.Create("These are some notes.");
        var notes2 = LongTruncateNotes.Create("These are some notes.");

        // Act & Assert
        notes1.GetHashCode().ShouldBe(notes2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string value = "These are some notes.";
        var notes = LongTruncateNotes.Create(value);

        // Act & Assert
        notes.ToString().ShouldBe(value);
    }
}
