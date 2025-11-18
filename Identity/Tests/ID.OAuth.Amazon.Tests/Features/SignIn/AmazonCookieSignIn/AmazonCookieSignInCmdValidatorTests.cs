using FluentValidation.TestHelper;
using ID.OAuth.Amazon.Features.SignIn.AmazonCookieSignIn;
using Shouldly;

namespace ID.OAuth.Amazon.Tests.Features.SignIn.AmazonCookieSignIn;

public class AmazonCookieSignInCmdValidatorTests
{
    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new AmazonCookieSignInCmdValidator();
        var command = new AmazonCookieSignInCmd(null!);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);
    }

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsNotNull()
    {
        // Arrange
        var validator = new AmazonCookieSignInCmdValidator();
        var command = new AmazonCookieSignInCmd(new AmazonCookieSignInDto { AuthToken = "token", RememberMe = false });

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }
}
