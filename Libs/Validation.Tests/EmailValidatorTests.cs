using Microsoft.VisualStudio.TestPlatform.Utilities;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace ValidationHelpers.Tests;

public class EmailValidatorTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@example.co.uk")]
    [InlineData("a.b-c_d@domain.com")]
    [InlineData("1234567890@example.com")]
    [InlineData("email@example-domain.com")]
    [InlineData("email@example.name")]
    [InlineData("email@example.museum")]
    [InlineData("email@example.co.jp")]
    public void IsValid_WhenGivenValidEmailAddress_ReturnsTrue(string email)
    {
        // Act
        var result = EmailValidator.IsValid(email);
        
        // Assert
        result.ShouldBeTrue();
    }
    
    [Theory]
    [InlineData("plainaddress")]
    [InlineData("#@%^%#$@#$@#.com")]
    [InlineData("@example.com")]
    [InlineData("Joe Smith <email@example.com>")]
    [InlineData("email.example.com")]
    [InlineData("email@example@example.com")]
    [InlineData("email@-example.com")]
    [InlineData(".email@example.com")]
    public void IsValid_WhenGivenInvalidEmailAddress_ReturnsFalse(string email)
    {
        // Act
        var result = EmailValidator.IsValid(email);
        output.WriteLine($"Testing email: {email} - Result: {result}");

        // Assert
        result.ShouldBeFalse();
    }
    


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void IsValid_WhenGivenNullOrEmptyWithAllowNullsTrue_ReturnsTrue(string? email)
    {
        // Act
        var result = EmailValidator.IsValid(email, allowNulls: true);
        
        // Assert
        result.ShouldBeTrue();
    }
    


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void IsValid_WhenGivenNullOrEmptyWithAllowNullsFalse_ReturnsFalse(string? email)
    {
        // Act
        var result = EmailValidator.IsValid(email, allowNulls: false);
        
        // Assert
        result.ShouldBeFalse();
    }
}