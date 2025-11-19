using FluentValidation.TestHelper;
using ID.OAuth.Amazon.Features.SignIn;
using ID.OAuth.Amazon.Features.SignIn.AmazonSignIn;

namespace ID.OAuth.Amazon.Tests.Features.SignIn.AmazonSignIn;

public class AmazonSignInCmdValidatorTests
{
    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new AmazonSignInCmdValidator();
        var command = new AmazonSignInCmd(null!);

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
        var validator = new AmazonSignInCmdValidator();
        var command = new AmazonSignInCmd(new AmazonSignInDto { AuthToken = "token" });

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }
}

