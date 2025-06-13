namespace StringHelpers.Tests;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class ParsingHelpersTests
{
    [Fact]
    public void RemoveWhitespaces_WithNullInput_ReturnsEmptyString()
    {
        // Arrange
        string input = null;

        // Act
        var result = ParsingHelpers.RemoveWhitespaces(input);

        // Assert
        Assert.Equal("", result);
    }



    [Fact]
    public void RemoveWhitespaces_WithMixedWhitespace_RemovesAllWhitespace()
    {
        // Arrange
        string input = "This has spaces\tand\ttabs\nand\nnewlines";

        // Act
        var result = ParsingHelpers.RemoveWhitespaces(input);

        // Assert
        Assert.Equal("Thishasspacesandtabsandnewlines", result);
    }



    [Fact]
    public void RemoveAllNonDigits_WithMixedContent_ReturnsOnlyDigits()
    {
        // Arrange
        string input = "Phone: (123) 456-7890";

        // Act
        var result = ParsingHelpers.RemoveAllNonDigits(input);

        // Assert
        Assert.Equal("1234567890", result);
    }



    [Fact]
    public void IsExactly_X_Digits_WithCorrectInput_ReturnsTrue()
    {
        // Arrange
        string input = "12345";

        // Act
        var result = ParsingHelpers.IsExactly_X_Digits(input, 5);

        // Assert
        Assert.True(result);
    }



    [Fact]
    public void CleanPhoneNumber_WithFormattedNumber_ReturnsDigitsOnly()
    {
        // Arrange
        string input = "(555) 123-4567";

        // Act
        var result = ParsingHelpers.CleanPhoneNumber(input);

        // Assert
        Assert.Equal("5551234567", result);
    }


    [Fact]
    public void RemoveQuotes_WithNestedQuotes_RemovesAllLayers()
    {
        // Arrange
        string input = "'\"nested quotes\"'";

        // Act
        var result = ParsingHelpers.RemoveQuotes(input);

        // Assert
        Assert.Equal("nested quotes", result);
    }



    // Using xUnit's Theory for parameterized tests
    [Theory]
    [InlineData("12345", 5, true)]
    [InlineData("123", 5, false)]
    [InlineData("1234A", 5, false)]
    [InlineData(null, 5, false)]
    public void IsExactly_X_Digits_WithVariousInputs(string input, int digits, bool expected)
    {
        // Act
        var result = ParsingHelpers.IsExactly_X_Digits(input, digits);

        // Assert
        Assert.Equal(expected, result);
    }



    [Theory]
    [InlineData("'quoted'", "quoted")]
    [InlineData("\"double quoted\"", "double quoted")]
    [InlineData("'\"nested\"'", "nested")]
    [InlineData("normal text", "normal text")]
    public void RemoveQuotes_WithDifferentQuotePatterns(string input, string expected)
    {
        // Act
        var result = ParsingHelpers.RemoveQuotes(input);

        // Assert
        Assert.Equal(expected, result);
    }


}