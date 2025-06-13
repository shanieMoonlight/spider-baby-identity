using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class LongNotesNullableTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validNotes = "These are some valid long notes with detailed content.";

        // Act
        var notes = LongNotesNullable.Create(validNotes);

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
        var notes = LongNotesNullable.Create(notesWithSpaces);

        // Assert
        notes.Value.ShouldBe(expectedNotes);
    }

    [Fact]
    public void Create_WithNullValue_ShouldNotThrowException()
    {
        // Arrange
        string? nullNotes = null;

        // Act & Assert
        Should.NotThrow(() => LongNotesNullable.Create(nullNotes));
    }

    [Fact]
    public void Create_WithNullValue_ShouldHaveNullValue()
    {
        // Arrange
        string? nullNotes = null;

        // Act
        var notes = LongNotesNullable.Create(nullNotes);

        // Assert
        notes.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyString_ShouldNotThrowException()
    {
        // Arrange
        string emptyNotes = "";

        // Act & Assert
        Should.NotThrow(() => LongNotesNullable.Create(emptyNotes));
    }

    [Fact]
    public void Create_WithEmptyString_ShouldHaveNullValue()
    {
        // Arrange
        string emptyNotes = "";

        // Act
        var notes = LongNotesNullable.Create(emptyNotes);

        // Assert
        notes.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldHaveNullValue()
    {
        // Arrange
        string whiteSpaceNotes = "   ";

        // Act
        var notes = LongNotesNullable.Create(whiteSpaceNotes);

        // Assert
        notes.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongNotes = new('a', LongNotesNullable.MaxLength + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            LongNotesNullable.Create(tooLongNotes))
            .Property.ShouldBe(nameof(LongNotes));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthNotes = new('a', LongNotesNullable.MaxLength);

        // Act
        var notes = LongNotesNullable.Create(maxLengthNotes);

        // Assert
        notes.ShouldNotBeNull();
        notes.Value.ShouldBe(maxLengthNotes);
    }

    [Fact]
    public void MaxLength_ShouldMatchExpectedValue()
    {
        // Assert
        LongNotesNullable.MaxLength.ShouldBe(LongNotes.MaxLength);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var notes1 = LongNotesNullable.Create("These are some notes.");
        var notes2 = LongNotesNullable.Create("These are some notes.");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var notes1 = LongNotesNullable.Create(null);
        var notes2 = LongNotesNullable.Create(null);

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var notes1 = LongNotesNullable.Create(null);
        var notes2 = LongNotesNullable.Create("These are some notes.");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var notes1 = LongNotesNullable.Create("These are some notes.");
        var notes2 = LongNotesNullable.Create("These are different notes.");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var notes1 = LongNotesNullable.Create("These are SOME notes.");
        var notes2 = LongNotesNullable.Create("These are some notes.");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var notes1 = LongNotesNullable.Create("These are some notes.");
        var notes2 = LongNotesNullable.Create("These are some notes.");

        // Act & Assert
        notes1.GetHashCode().ShouldBe(notes2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var notes1 = LongNotesNullable.Create(null);
        var notes2 = LongNotesNullable.Create(null);

        // Act & Assert
        notes1.GetHashCode().ShouldBe(notes2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValue()
    {
        // Arrange
        string value = "These are some notes.";
        var notes = LongNotesNullable.Create(value);

        // Act & Assert
        notes.ToString().ShouldBe(value);
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var notes = LongNotesNullable.Create(null);

        // Act & Assert
        notes.ToString().ShouldBe(string.Empty);
    }
}
