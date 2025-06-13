using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Setup;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class DescriptionTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validDescription = "This is a valid description text";

        // Act
        var description = Description.Create(validDescription);

        // Assert
        description.ShouldNotBeNull();
        description.Value.ShouldBe(validDescription);
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string descriptionWithSpaces = "  This is a valid description text  ";

        // Act
        var description = Description.Create(descriptionWithSpaces);

        // Assert
        description.Value.ShouldBe("This is a valid description text");
    }

    [Fact]
    public void Create_WithNullValue_ShouldThrowException()
    {
        // Arrange
        string nullDescription = null;

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            Description.Create(nullDescription))
            .Property.ShouldBe(nameof(Description));
    }

    [Fact]
    public void Create_WithEmptyString_ShouldThrowException()
    {
        // Arrange
        string emptyDescription = "";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            Description.Create(emptyDescription))
            .Property.ShouldBe(nameof(Description));
    }

    [Fact]
    public void Create_WithWhitespaceOnly_ShouldThrowException()
    {
        // Arrange
        string whitespaceDescription = "   ";

        // Act & Assert
        Should.Throw<IsRequiredPropertyException>(() => 
            Description.Create(whitespaceDescription))
            .Property.ShouldBe(nameof(Description));
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongDescription = new('A', ValueObjectsDefaultValues.MAX_LENGTH_DESCRIPTION + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            Description.Create(tooLongDescription))
            .Property.ShouldBe(nameof(Description));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthDescription = new('A', ValueObjectsDefaultValues.MAX_LENGTH_DESCRIPTION);

        // Act
        var description = Description.Create(maxLengthDescription);

        // Assert
        description.ShouldNotBeNull();
        description.Value.ShouldBe(maxLengthDescription);
    }

    [Fact]
    public void MaxLength_ShouldMatchDefaultValue()
    {
        // Arrange & Act & Assert
        Description.MaxLength.ShouldBe(ValueObjectsDefaultValues.MAX_LENGTH_DESCRIPTION);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var desc1 = Description.Create("Test description");
        var desc2 = Description.Create("Test description");

        // Act & Assert
        desc1.Equals(desc2).ShouldBeTrue();
        (desc1 == desc2).ShouldBeTrue();
        (desc1 != desc2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var desc1 = Description.Create("Test description");
        var desc2 = Description.Create("Different description");

        // Act & Assert
        desc1.Equals(desc2).ShouldBeFalse();
        (desc1 == desc2).ShouldBeFalse();
        (desc1 != desc2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var desc1 = Description.Create("Test description");
        var desc2 = Description.Create("TEST DESCRIPTION");

        // Act & Assert
        // StrValueObject uses case-insensitive comparison
        desc1.Equals(desc2).ShouldBeTrue();
        (desc1 == desc2).ShouldBeTrue();
        (desc1 != desc2).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var desc1 = Description.Create("Test description");
        var desc2 = Description.Create("Test description");

        // Act & Assert
        desc1.GetHashCode().ShouldBe(desc2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var description = Description.Create("Test description");

        // Act & Assert
        description.ToString().ShouldBe("Test description");
    }
}
