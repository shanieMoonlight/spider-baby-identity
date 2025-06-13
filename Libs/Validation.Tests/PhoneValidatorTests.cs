using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace ValidationHelpers.Tests;

public class PhoneValidatorTests
{
    private readonly ITestOutputHelper _output;

    public PhoneValidatorTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData("123-456-7890")]
    [InlineData("(123) 456-7890")]
    [InlineData("123.456.7890")]
    [InlineData("123 456 7890")]
    [InlineData("1234567890")]
    public void IsValid_WithValidPhoneFormats_ReturnsTrue(string phone)
    {
        // Act
        var result = PhoneValidator.IsValid(phone);
        
        // Assert
        result.ShouldBeTrue($"Phone '{phone}' should be valid");
    }
    
    [Theory]
    [InlineData("123-456-7890 ext 123")]
    [InlineData("123-456-7890 ext.123")]
    [InlineData("123-456-7890 x123")]
    [InlineData("123-456-7890 ext. 456")]
    [InlineData("123-456-7890 x 789")]
    public void IsValid_WithExtensions_ReturnsTrue(string phone)
    {
        // Act
        var result = PhoneValidator.IsValid(phone);
        _output.WriteLine($"Testing phone with extension: {phone}");
        
        // Assert
        result.ShouldBeTrue($"Phone with extension '{phone}' should be valid");
    }
    
    [Theory]
    [InlineData("abcdefg")]
    [InlineData("123@456")]
    [InlineData("phone: 123-456")]
    public void IsValid_WithInvalidCharacters_ReturnsFalse(string phone)
    {
        // Act
        var result = PhoneValidator.IsValid(phone);
        
        // Assert
        result.ShouldBeFalse($"Phone '{phone}' should be invalid due to invalid characters");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void IsValid_WithNullOrEmpty_ReturnsExpectedResult(string? phone)
    {
        // Act - with allowNulls=true (default)
        var resultWithNulls = PhoneValidator.IsValid(phone,true);
        
        // Assert
        resultWithNulls.ShouldBeTrue($"Null/empty phone should be valid when allowNulls=true");
        
        // Act - with allowNulls=false
        var resultWithoutNulls = PhoneValidator.IsValid(phone);
        
        // Assert
        resultWithoutNulls.ShouldBeFalse($"Null/empty phone should be invalid when allowNulls=false");
    }

    [Theory]
    [InlineData("12345", 6, false)]  // Too short
    [InlineData("12345", 5, true)]   // Exactly minimum length
    [InlineData("123456", 5, true)]  // Longer than minimum
    [InlineData("123-456", 6, true)] // Characters don't count toward minimum
    [InlineData("12-34", 6, false)]  // Too few digits even with separators
    public void IsValid_MinimumLengthValidation_ReturnsExpectedResult(string phone, int minLength, bool expected)
    {
        // Act
        var result = PhoneValidator.IsValid(phone, minLength);
        _output.WriteLine($"Testing phone: '{phone}' with minLength={minLength}");

        // Assert
        result.ShouldBe(expected, $"Phone '{phone}' with minLength={minLength} should return {expected}");
    }

    [Fact]
    public void IsValid_PhoneWithPlusSign_RemovesPlusAndValidates()
    {
        // Arrange
        var phone = "+1-234-567-8901";
        
        // Act
        var result = PhoneValidator.IsValid(phone);
        
        // Assert
        result.ShouldBeTrue($"Phone with plus sign '{phone}' should be valid");
    }
    
    [Theory]
    [InlineData("+1-234-567-8901")]
    [InlineData("+44 (20) 1234 5678")]
    [InlineData("+61.2.3456.7890")]
    [InlineData("+49-89-636-48018")]
    [InlineData("+86 10 6552 9988")]
    public void IsValid_InternationalFormats_ReturnsTrue(string phone)
    {
        // Act
        var result = PhoneValidator.IsValid(phone);
        _output.WriteLine($"Testing international format: {phone}");
        
        // Assert
        result.ShouldBeTrue($"International phone '{phone}' should be valid");
    }
    
    [Theory]
    [InlineData("+1-234-567-8901 ext 123")]
    [InlineData("+44 (20) 1234 5678 x999")]
    [InlineData("+61.2.3456.7890 ext. 7")]
    public void IsValid_InternationalWithExtensions_ReturnsTrue(string phone)
    {
        // Act
        var result = PhoneValidator.IsValid(phone);
        _output.WriteLine($"Testing international with extension: {phone}");
        
        // Assert
        result.ShouldBeTrue($"International phone with extension '{phone}' should be valid");
    }
    
    [Theory]
    [InlineData("++1-234-567-8901")]  // Double plus
    [InlineData("+abc-123-4567")]     // Invalid characters after plus
    public void IsValid_InvalidInternationalFormats_ReturnsFalse(string phone)
    {
        // Act
        var result = PhoneValidator.IsValid(phone);
        _output.WriteLine($"Testing invalid international format: {phone}");
        
        // Assert
        result.ShouldBeFalse($"Invalid international phone '{phone}' should be invalid");
    }
}