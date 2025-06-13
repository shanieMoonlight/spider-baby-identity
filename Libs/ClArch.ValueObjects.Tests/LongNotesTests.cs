using ClArch.ValueObjects.Exceptions;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
public class LongNotesTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validNotes = "These are some valid long notes with detailed content.";

        // Act
        var notes = LongNotes.Create(validNotes);

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
        var notes = LongNotes.Create(notesWithSpaces);

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
            LongNotes.Create(nullNotes))
            .Property.ShouldBe(nameof(LongNotes));
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        string emptyNotes = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            LongNotes.Create(emptyNotes))
            .Property.ShouldBe(nameof(LongNotes));
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldThrowException()
    {
        // Arrange
        string whiteSpaceNotes = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            LongNotes.Create(whiteSpaceNotes))
            .Property.ShouldBe(nameof(LongNotes));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongNotes = new('a', LongNotes.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            LongNotes.Create(tooLongNotes))
            .Property.ShouldBe(nameof(LongNotes));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthNotes = new('a', LongNotes.MaxLength);

        // Act
        var notes = LongNotes.Create(maxLengthNotes);

        // Assert
        notes.ShouldNotBeNull();
        notes.Value.ShouldBe(maxLengthNotes);
    }

    [Fact]
    public void MaxLength_ShouldHaveExpectedValue()
    {
        // Assert
        LongNotes.MaxLength.ShouldBePositive();
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var notes1 = LongNotes.Create("These are some notes.");
        var notes2 = LongNotes.Create("These are some notes.");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var notes1 = LongNotes.Create("These are some notes.");
        var notes2 = LongNotes.Create("These are different notes.");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var notes1 = LongNotes.Create("These are SOME notes.");
        var notes2 = LongNotes.Create("These are some notes.");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var notes1 = LongNotes.Create("These are some notes.");
        var notes2 = LongNotes.Create("These are some notes.");

        // Act & Assert
        notes1.GetHashCode().ShouldBe(notes2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string value = "These are some notes.";
        var notes = LongNotes.Create(value);

        // Act & Assert
        notes.ToString().ShouldBe(value);
    }
}
