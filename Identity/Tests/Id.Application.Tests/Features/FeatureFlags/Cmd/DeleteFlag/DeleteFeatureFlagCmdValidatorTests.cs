using FluentValidation.TestHelper;
using ID.Application.Features.FeatureFlags.Cmd.Delete;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using Shouldly;

namespace ID.Application.Tests.Features.FeatureFlags.Cmd.DeleteFlag;

public class DeleteFeatureFlagCmdValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new DeleteFeatureFlagCmdValidator();
        var command = new DeleteFeatureFlagCmd(default);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(cmd => cmd.Id);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsNotNull()
    {
        // Arrange
        var validator = new DeleteFeatureFlagCmdValidator();
        var command = new DeleteFeatureFlagCmd(Guid.NewGuid());
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new DeleteFeatureFlagCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<DeleteFeatureFlagCmd>>();
    }

    //------------------------------------//

}//Cls