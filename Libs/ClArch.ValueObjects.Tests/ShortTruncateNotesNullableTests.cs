using ClArch.ValueObjects;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

public class ShortTruncateNotesNullableTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validNotes = "This is a short note";

        // Act
        var notes = ShortTruncateNotesNullable.Create(validNotes);

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
        var notes = ShortTruncateNotesNullable.Create(notesWithSpaces);

        // Assert
        notes.Value.ShouldBe(expectedNotes);
    }

    [Fact]
    public void Create_WithNullValue_ShouldNotThrowException()
    {
        // Arrange
        string? nullNotes = null;

        // Act & Assert
        Should.NotThrow(() => ShortTruncateNotesNullable.Create(nullNotes));
    }

    [Fact]
    public void Create_WithNullValue_ShouldHaveNullValue()
    {
        // Arrange
        string? nullNotes = null;

        // Act
        var notes = ShortTruncateNotesNullable.Create(nullNotes);

        // Assert
        notes.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyString_ShouldNotThrowException()
    {
        // Arrange
        string emptyNotes = "";

        // Act & Assert
        Should.NotThrow(() => ShortTruncateNotesNullable.Create(emptyNotes));
    }

    [Fact]
    public void Create_WithEmptyString_ShouldHaveNullValue()
    {
        // Arrange
        string emptyNotes = "";

        // Act
        var notes = ShortTruncateNotesNullable.Create(emptyNotes);

        // Assert
        notes.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldHaveNullValue()
    {
        // Arrange
        string whiteSpaceNotes = "   ";

        // Act
        var notes = ShortTruncateNotesNullable.Create(whiteSpaceNotes);

        // Assert
        notes.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldTruncateValue()
    {
        // Arrange
        string tooLongNotes = new('a', ShortTruncateNotesNullable.MaxLength + 100);
        string expectedNotes = new string ('a', ShortTruncateNotesNullable.MaxLength - 3) + "...";

        // Act
        var notes = ShortTruncateNotesNullable.Create(tooLongNotes);

        // Assert
        notes.Value?.Length.ShouldBe(ShortTruncateNotesNullable.MaxLength);
        notes.Value.ShouldBe(expectedNotes);
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthNotes = new('a', ShortTruncateNotesNullable.MaxLength);

        // Act
        var notes = ShortTruncateNotesNullable.Create(maxLengthNotes);

        // Assert
        notes.ShouldNotBeNull();
        notes.Value.ShouldBe(maxLengthNotes);
    }

    [Fact]
    public void MaxLength_ShouldMatchExpectedValue()
    {
        // Assert
        ShortTruncateNotesNullable.MaxLength.ShouldBe(ShortNotes.MaxLength);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var notes1 = ShortTruncateNotesNullable.Create("This is a short note");
        var notes2 = ShortTruncateNotesNullable.Create("This is a short note");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var notes1 = ShortTruncateNotesNullable.Create(null);
        var notes2 = ShortTruncateNotesNullable.Create(null);

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var notes1 = ShortTruncateNotesNullable.Create(null);
        var notes2 = ShortTruncateNotesNullable.Create("This is a short note");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var notes1 = ShortTruncateNotesNullable.Create("This is a short note");
        var notes2 = ShortTruncateNotesNullable.Create("This is a different note");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var notes1 = ShortTruncateNotesNullable.Create("This is a SHORT note");
        var notes2 = ShortTruncateNotesNullable.Create("This is a short note");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var notes1 = ShortTruncateNotesNullable.Create("This is a short note");
        var notes2 = ShortTruncateNotesNullable.Create("This is a short note");

        // Act & Assert
        notes1.GetHashCode().ShouldBe(notes2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var notes1 = ShortTruncateNotesNullable.Create(null);
        var notes2 = ShortTruncateNotesNullable.Create(null);

        // Act & Assert
        notes1.GetHashCode().ShouldBe(notes2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValue()
    {
        // Arrange
        string value = "This is a short note";
        var notes = ShortTruncateNotesNullable.Create(value);

        // Act & Assert
        notes.ToString().ShouldBe(value);
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var notes = ShortTruncateNotesNullable.Create(null);

        // Act & Assert
        notes.ToString().ShouldBe(string.Empty);
    }
}
