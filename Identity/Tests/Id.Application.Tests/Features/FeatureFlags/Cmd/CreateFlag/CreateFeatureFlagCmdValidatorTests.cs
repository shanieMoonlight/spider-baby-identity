using FluentValidation.TestHelper;
using ID.Application.Features.FeatureFlags;
using ID.Application.Features.FeatureFlags.Cmd.Create;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using Shouldly;

namespace ID.Application.Tests.Features.FeatureFlags.Cmd.CreateFlag;

public class CreateFeatureFlagCmdValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new CreateFeatureFlagCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new CreateFeatureFlagCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsNotNull()
    {
        // Arrange
        var validator = new CreateFeatureFlagCmdValidator();
        var command = new CreateFeatureFlagCmd(new FeatureFlagDto());
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
        var validator = new CreateFeatureFlagCmdValidator();


        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<CreateFeatureFlagCmd>>();
    }

    //------------------------------------//

}//Cls