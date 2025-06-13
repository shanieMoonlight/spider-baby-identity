using FluentValidation.TestHelper;
using ID.Application.Features.SubscriptionPlans.Cmd.Delete;
using ID.Application.Mediatr.Validation;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.SubscriptionPlans.Cmd.Delete;

public class DeleteSubscriptionPlanCmdValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new DeleteSubscriptionPlanCmdValidator();
        var command = new DeleteSubscriptionPlanCmd(default);
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(cmd => cmd.Id);
        result.Errors.First().ErrorMessage.ShouldBe(IDMsgs.Error.IsRequired(nameof(SubscriptionPlan.Id)));
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsNotNull()
    {
        // Arrange
        var validator = new DeleteSubscriptionPlanCmdValidator();
        var command = new DeleteSubscriptionPlanCmd(Guid.NewGuid());
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
        var validator = new DeleteSubscriptionPlanCmdValidator();
        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<DeleteSubscriptionPlanCmd>>();
    }

    //------------------------------------//

}//Cls