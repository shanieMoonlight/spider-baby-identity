using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Features.Teams.Cmd.Subs.RecordSubscriptionPayment;
using ID.Application.Features.Teams.Cmd.Subs.RemoveSubscription;
using ID.Application.Tests.Features.Utility;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Tests.Features.Teams.Cmd.@Subs.Payment;

public class RecordSubscriptionPaymentCmdValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new RecordSubscriptionPaymentCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new RecordSubscriptionPaymentCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoTeamID()
    {
        // Arrange
        var validator = new RecordSubscriptionPaymentCmdValidator();
        var command = new RecordSubscriptionPaymentCmd(new RecordSubscriptionPaymentDto(default, Guid.NewGuid()));
        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoSubscriptionPlanId()
    {
        // Arrange
        var validator = new RecordSubscriptionPaymentCmdValidator();
        var command = new RecordSubscriptionPaymentCmd(new RecordSubscriptionPaymentDto(Guid.NewGuid(), default));
        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsValid()
    {
        // Arrange
        var validator = new RecordSubscriptionPaymentCmdValidator();
        var command = new RecordSubscriptionPaymentCmd(new RecordSubscriptionPaymentDto(Guid.NewGuid(), Guid.NewGuid()));
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
        var validator = new RecordSubscriptionPaymentCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<RecordSubscriptionPaymentCmd>>();
    }

    //------------------------------------//


}//Cls