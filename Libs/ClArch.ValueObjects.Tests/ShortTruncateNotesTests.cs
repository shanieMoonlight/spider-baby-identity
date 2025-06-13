using ClArch.ValueObjects;
using ClArch.ValueObjects.Exceptions;
using Shouldly;
using Xunit;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
public class ShortTruncateNotesTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validNotes = "This is a short note";

        // Act
        var notes = ShortTruncateNotes.Create(validNotes);

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
        var notes = ShortTruncateNotes.Create(notesWithSpaces);

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
            ShortTruncateNotes.Create(nullNotes))
            .Property.ShouldBe(nameof(ShortNotes));
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldThrowException()
    {
        // Arrange
        string emptyNotes = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            ShortTruncateNotes.Create(emptyNotes))
            .Property.ShouldBe(nameof(ShortNotes));
    }

    [Fact]
    public void Create_WithWhiteSpaceValue_ShouldThrowException()
    {
        // Arrange
        string whiteSpaceNotes = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            ShortTruncateNotes.Create(whiteSpaceNotes))
            .Property.ShouldBe(nameof(ShortNotes));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldTruncateValue()
    {
        // Arrange
        string tooLongNotes = new('a', ShortTruncateNotes.MaxLength + 100);
        string expectedNotes = new string('a', ShortTruncateNotes.MaxLength - 3) + "...";

        // Act
        var notes = ShortTruncateNotes.Create(tooLongNotes);

        // Assert
        notes.Value.Length.ShouldBe(ShortTruncateNotes.MaxLength);
        notes.Value.ShouldBe(expectedNotes);
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthNotes = new('a', ShortTruncateNotes.MaxLength);

        // Act
        var notes = ShortTruncateNotes.Create(maxLengthNotes);

        // Assert
        notes.ShouldNotBeNull();
        notes.Value.ShouldBe(maxLengthNotes);
    }

    [Fact]
    public void MaxLength_ShouldMatchExpectedValue()
    {
        // Assert
        ShortTruncateNotes.MaxLength.ShouldBe(ShortNotes.MaxLength);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var notes1 = ShortTruncateNotes.Create("This is a short note");
        var notes2 = ShortTruncateNotes.Create("This is a short note");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var notes1 = ShortTruncateNotes.Create("This is a short note");
        var notes2 = ShortTruncateNotes.Create("This is a different note");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var notes1 = ShortTruncateNotes.Create("This is a SHORT note");
        var notes2 = ShortTruncateNotes.Create("This is a short note");

        // Act & Assert
        notes1.Equals(notes2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var notes1 = ShortTruncateNotes.Create("This is a short note");
        var notes2 = ShortTruncateNotes.Create("This is a short note");

        // Act & Assert
        notes1.GetHashCode().ShouldBe(notes2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        string value = "This is a short note";
        var notes = ShortTruncateNotes.Create(value);

        // Act & Assert
        notes.ToString().ShouldBe(value);
    }
}
