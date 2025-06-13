using FluentValidation.TestHelper;
using ID.Application.Features.SubscriptionPlans.Cmd.FeatureFlags.RemoveFeature;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.SubscriptionPlans.Cmd.FeatureFlags.RemoveFeature;
public class RemoveFeatureFromPlanCmdValidatorTests
{
    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new RemoveFeaturesFromSubscriptionPlanCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new RemoveFeaturesFromSubscriptionPlanCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);
        result.Errors.ShouldContain(e => e.ErrorMessage == IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsNotNull()
    {
        // Arrange
        var validator = new RemoveFeaturesFromSubscriptionPlanCmdValidator();
        var dto = new RemoveFeaturesFromSubscriptionPlanDto(Guid.NewGuid(), [Guid.NewGuid()]);
        var command = new RemoveFeaturesFromSubscriptionPlanCmd(dto);
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
        var validator = new RemoveFeaturesFromSubscriptionPlanCmdValidator();
        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<RemoveFeaturesFromSubscriptionPlanCmd>>();
    }

    //------------------------------------//

}//Cls
