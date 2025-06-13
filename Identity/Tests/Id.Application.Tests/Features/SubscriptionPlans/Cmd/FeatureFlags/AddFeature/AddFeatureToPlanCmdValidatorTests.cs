using FluentValidation.TestHelper;
using ID.Application.Features.SubscriptionPlans.Cmd.FeatureFlags.AddFeature;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.SubscriptionPlans.Cmd.FeatureFlags.AddFeature;
public class AddFeaturesToPlanCmdValidatorTests
{
    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new AddFeatureToSubscriptionPlanCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new AddFeatureToSubscriptionPlanCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED); 

        result.IsValid.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsNotNull()
    {
        // Arrange
        var validator = new AddFeatureToSubscriptionPlanCmdValidator();
        var command = new AddFeatureToSubscriptionPlanCmd(new AddFeaturesToPlanDto(Guid.NewGuid(), [Guid.NewGuid()]));
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
        var validator = new AddFeatureToSubscriptionPlanCmdValidator();
        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<AddFeatureToSubscriptionPlanCmd>>();
    }

    //------------------------------------//

}//Cls
