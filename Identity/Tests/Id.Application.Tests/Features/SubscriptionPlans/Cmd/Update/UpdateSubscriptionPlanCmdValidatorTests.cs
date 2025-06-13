using FluentValidation.TestHelper;
using ID.Application.Features.SubscriptionPlans;
using ID.Application.Features.SubscriptionPlans.Cmd.Update;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.SubscriptionPlans.Cmd.Update;

public class UpdateSubscriptionPlanCmdValidatorTests
{
    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new UpdateSubscriptionPlanCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new UpdateSubscriptionPlanCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsNotNull()
    {
        // Arrange
        var validator = new UpdateSubscriptionPlanCmdValidator();
        var command = new UpdateSubscriptionPlanCmd(new SubscriptionPlanDto());
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
        var validator = new UpdateSubscriptionPlanCmdValidator();
        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<UpdateSubscriptionPlanCmd>>();
    }

    //------------------------------------//

}//Cls