namespace StringHelpers.Tests;

public class StringExtensionsTests
{

    [Fact]
    public void AppendPath_CombinesUrlWithPath()
    {
        // Arrange
        string baseUrl = "https://example.com";
        string path = "api/users";
        
        // Act
        var result = baseUrl.AppendPath(path);
        
        // Assert
        Assert.Equal("https://example.com/api/users", result);
    }


    
    [Theory]
    [InlineData("camelCase", "CamelCase")]
    [InlineData("already", "Already")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void CamelToPascal_ConvertsFirstLetterToUppercase(string input, string expected)
    {
        // Act
        var result = input.CamelToPascal();
        
        // Assert
        Assert.Equal(expected, result);
    }


    
    [Theory]
    [InlineData("Hello", "HELLO", true)]
    [InlineData("Hello", "hello", true)]
    [InlineData("Hello", "Hallo", false)]
    [InlineData(null, null, true)]
    [InlineData("Hello", null, false)]
    [InlineData(null, "Hello", false)]
    public void InvariantEquals_IgnoresCaseWhenComparing(string first, string second, bool expected)
    {
        // Act
        var result = first.InvariantEquals(second);
        
        // Assert
        Assert.Equal(expected, result);
    }


    
    [Theory]
    [InlineData("Hello123#$%", "Hello123")]
    [InlineData("123ABC", "123ABC")]
    [InlineData("!@#$%^", "")]
    [InlineData(null, "")]
    public void OnlyAlphanumeric_RemovesAllNonAlphanumericChars(string input, string expected)
    {
        // Act
        var result = input.OnlyAlphanumeric();
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void IsNullOrWhiteSpace_WithEmptyEnumerable_ReturnsTrue()
    {
        // Arrange
        List<string> emptyList = [];
        
        // Act
        var result = emptyList.IsNullOrWhiteSpace();
        
        // Assert
        Assert.True(result);
    }


    
    [Fact]
    public void IsNullOrWhiteSpace_WithValidItems_ReturnsFalse()
    {
        // Arrange
        List<string> validList = ["item1", "item2"];
        
        // Act
        var result = validList.IsNullOrWhiteSpace();
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData("This has spaces", false, "Thishasspaces")]
    [InlineData("This has spaces", true, "thishasspaces")]
    [InlineData(null, false, "")]
    public void RemoveWhitespaces_RemovesAllWhitespaceChars(string input, bool toLower, string expected)
    {
        // Act
        var result = input.RemoveWhitespaces(toLower);
        
        // Assert
        Assert.Equal(expected, result);
    }


    
    [Theory]
    [InlineData("123", 123)]
    [InlineData("not a number", -1)]
    [InlineData("", -1)]
    [InlineData(null, -1)]
    [InlineData("123", 456, 123)]
    [InlineData("not a number", 456, 456)]
    public void ToInt_ParsesIntegerOrReturnsDefault(string input, int defaultValue, int? expected = null)
    {
        // Act
        var result = input.ToInt(defaultValue);
        
        // Assert
        Assert.Equal(expected ?? defaultValue, result);
    }


    
    [Theory]
    [InlineData("A very long string that needs truncation", 10, "A very ...")]
    [InlineData("Short", 10, "Short")]
    [InlineData("", 10, "")]
    [InlineData(null, 10, null)]
    public void Truncate_AddsDotDotDotForLongStrings(string input, int maxChars, string expected)
    {
        // Act
        var result = input.Truncate(maxChars);
        
        // Assert
        Assert.Equal(expected, result);
    }
}