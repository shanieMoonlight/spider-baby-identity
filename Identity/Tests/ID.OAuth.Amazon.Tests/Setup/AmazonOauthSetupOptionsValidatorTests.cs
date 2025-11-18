namespace ID.OAuth.Amazon.Tests.Setup;

public class AmazonOauthSetupOptionsValidatorTests
{
    [Fact]
    public void Validate_ReturnsSuccess_ForValidOptions()
    {
        // Arrange
        var opts = new IdOAuthAmazonOptions
        {
            ClientId = "cid",
            ClientSecret = "secret",
            ApiBaseUrl = "https://api.amazon.com/",
            RequestTimeoutSeconds = 30
        };

        var validator = new AmazonOauthSetupOptionsValidator();

        // Act
        var result = validator.Validate(name: null, options: opts);

        // Assert
        result.ShouldBe(ValidateOptionsResult.Success);
    }

    //--------------------------//

    [Fact]
    public void Validate_ReturnsFailure_WhenMissingRequiredFields()
    {
        // Arrange
        var opts = new IdOAuthAmazonOptions
        {
            ClientId = string.Empty,
            ClientSecret = string.Empty,
            ApiBaseUrl = string.Empty,
            RequestTimeoutSeconds = 0
        };

        var validator = new AmazonOauthSetupOptionsValidator();

        // Act
        var result = validator.Validate(name: null, options: opts);

        // Assert
        result.ShouldNotBe(ValidateOptionsResult.Success);
    }

}//Cls
