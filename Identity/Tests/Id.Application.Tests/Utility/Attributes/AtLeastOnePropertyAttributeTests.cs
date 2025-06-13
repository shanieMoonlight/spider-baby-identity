using ID.Application.Utility.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ID.Application.Tests.Utility.Attributes;

public class AtLeastOnePropertyAttributeTests
{
    [Fact]
    public void Validation_Fails_When_All_Properties_Empty()
    {
        // Arrange
        var testObject = new TestClass
        {
            PropertyA = null,
            PropertyB = string.Empty,
            PropertyC = "   " // whitespace
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(
            testObject,
            new ValidationContext(testObject),
            validationResults,
            validateAllProperties: true);

        // Assert
        Assert.False(isValid);
        Assert.Single(validationResults);
        Assert.Equal("You must supply at least one of [PropertyA, PropertyB, PropertyC]", validationResults[0].ErrorMessage);
    }


    //------------------------------------//


    [Theory]
    [InlineData("value", null, null)]
    [InlineData(null, "value", null)]
    [InlineData(null, null, "value")]
    [InlineData("value1", "value2", "value3")]
    public void Validation_Passes_When_AtLeastOne_Property_HasValue(string propA, string propB, string propC)
    {
        // Arrange
        var testObject = new TestClass
        {
            PropertyA = propA,
            PropertyB = propB,
            PropertyC = propC
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(
            testObject,
            new ValidationContext(testObject),
            validationResults,
            validateAllProperties: true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }


    //------------------------------------//


    [AtLeastOneProperty(nameof(PropertyA), nameof(PropertyB), nameof(PropertyC))]
    private class TestClass
    {
        public string? PropertyA { get; set; }
        public string? PropertyB { get; set; }
        public string? PropertyC { get; set; }
    }

}//Cls