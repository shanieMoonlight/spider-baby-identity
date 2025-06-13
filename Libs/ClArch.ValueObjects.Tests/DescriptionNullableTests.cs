using ClArch.ValueObjects.Exceptions;
using ClArch.ValueObjects.Setup;
using Shouldly;

namespace ClArch.ValueObjects.Tests;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class DescriptionNullableTests
{
    [Fact]
    public void Create_WithValidValue_ShouldReturnInstance()
    {
        // Arrange
        string validDescription = "This is a valid description text";

        // Act
        var description = DescriptionNullable.Create(validDescription);

        // Assert
        description.ShouldNotBeNull();
        description.Value.ShouldBe(validDescription);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnInstanceWithNullValue()
    {
        // Arrange
        string nullDescription = null;

        // Act
        var description = DescriptionNullable.Create(nullDescription);

        // Assert
        description.ShouldNotBeNull();
        description.Value.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyString_ShouldReturnInstanceWithEmptyString()
    {
        // Arrange
        string emptyDescription = "";

        // Act
        var description = DescriptionNullable.Create(emptyDescription);

        // Assert
        description.Value.ShouldBe("");
    }

    [Fact]
    public void Create_WithWhitespaceOnly_ShouldReturnTrimmedEmptyString()
    {
        // Arrange
        string whitespaceDescription = "   ";

        // Act
        var description = DescriptionNullable.Create(whitespaceDescription);

        // Assert
        description.Value.ShouldBe("");
    }

    [Fact]
    public void Create_WithSpaces_ShouldTrimValue()
    {
        // Arrange
        string descriptionWithSpaces = "  This is a valid description text  ";

        // Act
        var description = DescriptionNullable.Create(descriptionWithSpaces);

        // Assert
        description.Value.ShouldBe("This is a valid description text");
    }

    [Fact]
    public void Create_WithTooLongValue_ShouldThrowException()
    {
        // Arrange
        string tooLongDescription = new('A', ValueObjectsDefaultValues.MAX_LENGTH_DESCRIPTION + 1);

        // Act & Assert
        Should.Throw<StringTooLongPropertyException>(() => 
            DescriptionNullable.Create(tooLongDescription))
            .Property.ShouldBe(nameof(Description));
    }

    [Fact]
    public void Create_WithMaxLengthValue_ShouldReturnInstance()
    {
        // Arrange
        string maxLengthDescription = new('A', ValueObjectsDefaultValues.MAX_LENGTH_DESCRIPTION);

        // Act
        var description = DescriptionNullable.Create(maxLengthDescription);

        // Assert
        description.ShouldNotBeNull();
        description.Value.ShouldBe(maxLengthDescription);
    }

    [Fact]
    public void MaxLength_ShouldMatchDefaultValue()
    {
        // Arrange & Act & Assert
        DescriptionNullable.MaxLength.ShouldBe(ValueObjectsDefaultValues.MAX_LENGTH_DESCRIPTION);
    }

    [Fact]
    public void Equals_SameValue_ShouldBeTrue()
    {
        // Arrange
        var desc1 = DescriptionNullable.Create("Test description");
        var desc2 = DescriptionNullable.Create("Test description");

        // Act & Assert
        desc1.Equals(desc2).ShouldBeTrue();
        (desc1 == desc2).ShouldBeTrue();
        (desc1 != desc2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldBeFalse()
    {
        // Arrange
        var desc1 = DescriptionNullable.Create("Test description");
        var desc2 = DescriptionNullable.Create("Different description");

        // Act & Assert
        desc1.Equals(desc2).ShouldBeFalse();
        (desc1 == desc2).ShouldBeFalse();
        (desc1 != desc2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_DifferentCase_ShouldBeTrue()
    {
        // Arrange
        var desc1 = DescriptionNullable.Create("Test description");
        var desc2 = DescriptionNullable.Create("TEST DESCRIPTION");

        // Act & Assert
        // NullableStrValueObject uses case-insensitive comparison
        desc1.Equals(desc2).ShouldBeTrue();
        (desc1 == desc2).ShouldBeTrue();
        (desc1 != desc2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_BothNull_ShouldBeTrue()
    {
        // Arrange
        var desc1 = DescriptionNullable.Create(null);
        var desc2 = DescriptionNullable.Create(null);

        // Act & Assert
        desc1.Equals(desc2).ShouldBeTrue();
        (desc1 == desc2).ShouldBeTrue();
        (desc1 != desc2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_OneNull_ShouldBeFalse()
    {
        // Arrange
        var desc1 = DescriptionNullable.Create(null);
        var desc2 = DescriptionNullable.Create("Test description");

        // Act & Assert
        desc1.Equals(desc2).ShouldBeFalse();
        (desc1 == desc2).ShouldBeFalse();
        (desc1 != desc2).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldBeEqual()
    {
        // Arrange
        var desc1 = DescriptionNullable.Create("Test description");
        var desc2 = DescriptionNullable.Create("Test description");

        // Act & Assert
        desc1.GetHashCode().ShouldBe(desc2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_BothNull_ShouldBeEqual()
    {
        // Arrange
        var desc1 = DescriptionNullable.Create(null);
        var desc2 = DescriptionNullable.Create(null);

        // Act & Assert
        desc1.GetHashCode().ShouldBe(desc2.GetHashCode());
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnValue()
    {
        // Arrange
        var description = DescriptionNullable.Create("Test description");

        // Act & Assert
        description.ToString().ShouldBe("Test description");
    }

    [Fact]
    public void ToString_WithNull_ShouldReturnEmptyString()
    {
        // Arrange
        var description = DescriptionNullable.Create(null);

        // Act & Assert
        description.ToString().ShouldBe(string.Empty);
    }
}
