using FluentValidation.TestHelper;
using ID.Application.Features.SubscriptionPlans;
using ID.Application.Features.SubscriptionPlans.Cmd.Create;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.SubscriptionPlans.Cmd.Create;

public class CreateSubscriptionPlanCmdValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new CreateSubscriptionPlanCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new CreateSubscriptionPlanCmd(null);
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
        var validator = new CreateSubscriptionPlanCmdValidator();
        var command = new CreateSubscriptionPlanCmd(new SubscriptionPlanDto());
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
        var validator = new CreateSubscriptionPlanCmdValidator();
        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<CreateSubscriptionPlanCmd>>();
    }

    //------------------------------------//


}//Cls