namespace ID.OAuth.Facebook.Tests.Setup;

public class FbOauthSetupOptionsValidatorTests
{
    [Fact]
    public void Validate_ReturnsSuccess_ForValidOptions()
    {
        // Arrange
        var opts = new IdOAuthFacebookOptions
        {
            AppId = "app123",
            AppSecret = "secret",
            RequestTimeoutSeconds = 10,
            GraphApiVersion = "v24.0",
            GraphApiBaseUrl = "https://graph.facebook.com"
        };

        var validator = new FbOauthSetupOptionsValidator();

        // Act
        var result = validator.Validate(name: null, options: opts);

        // Assert
        result.ShouldBe(ValidateOptionsResult.Success);
    }

    //-----------------------//   

    [Fact]
    public void Validate_ReturnsFailure_WhenRequiredFieldsMissing()
    {
        // Arrange - missing AppId and AppSecret and bad timeout
        var opts = new IdOAuthFacebookOptions
        {
            AppId = string.Empty,
            AppSecret = string.Empty,
            RequestTimeoutSeconds = 0,
            GraphApiVersion = string.Empty,
            GraphApiBaseUrl = string.Empty
        };

        var validator = new FbOauthSetupOptionsValidator();

        // Act
        var result = validator.Validate(name: null, options: opts);

        // Assert
        result.ShouldNotBe(ValidateOptionsResult.Success);
    }
}//Cls
