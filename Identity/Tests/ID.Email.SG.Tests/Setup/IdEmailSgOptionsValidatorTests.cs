using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;
using ID.Email.SG.Setup;

namespace ID.Email.SG.Tests.Setup;

public class IdEmailSgOptionsValidatorTests
{
    [Fact]
    public void Validate_ReturnsSuccess_ForValidOptions()
    {
        // Arrange
        var opts = new IdEmailSgOptions
        {
            ApiKey = "sg.testkey",
        };

        var validator = new IdEmailSgOptionsValidator();

        // Act
        var result = validator.Validate(name: null, options: opts);

        // Assert
        result.ShouldBe(ValidateOptionsResult.Success);
    }

    //--------------------//

    [Fact]
    public void Validate_ReturnsFailure_WhenApiKeyMissing()
    {
        // Arrange
        var opts = new IdEmailSgOptions
        {
            ApiKey = string.Empty
        };

        var validator = new IdEmailSgOptionsValidator();

        // Act
        var result = validator.Validate(name: null, options: opts);

        // Assert
        result.ShouldNotBe(ValidateOptionsResult.Success);
    }
    //--------------------//

    [Fact]
    public void Validate_ReturnsFailure_WhenApiKeyIsWhitespace()
    {
        // Arrange
        var opts = new IdEmailSgOptions
        {
            ApiKey = "   "
        };

        var validator = new IdEmailSgOptionsValidator();

        // Act
        var result = validator.Validate(name: null, options: opts);

        // Assert
        result.ShouldNotBe(ValidateOptionsResult.Success);
    }
}//Cls
