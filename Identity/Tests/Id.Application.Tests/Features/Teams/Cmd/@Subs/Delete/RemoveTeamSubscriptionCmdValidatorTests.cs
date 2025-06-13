using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Features.Teams.Cmd.Subs.RemoveSubscription;
using ID.Application.Tests.Features.Utility;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Tests.Features.Teams.Cmd.@Subs.Delete;

public class RemoveTeamSubscriptionCmdValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new RemoveTeamSubscriptionCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new RemoveTeamSubscriptionCmd(null);
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
        var validator = new RemoveTeamSubscriptionCmdValidator();
        var command = new RemoveTeamSubscriptionCmd(new RemoveTeamSubscriptionDto(default, Guid.NewGuid()));
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
        var validator = new RemoveTeamSubscriptionCmdValidator();
        var command = new RemoveTeamSubscriptionCmd(new RemoveTeamSubscriptionDto(Guid.NewGuid(), default));
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
        var validator = new RemoveTeamSubscriptionCmdValidator();
        var command = new RemoveTeamSubscriptionCmd(new RemoveTeamSubscriptionDto(Guid.NewGuid(), Guid.NewGuid()));
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
        var validator = new RemoveTeamSubscriptionCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<RemoveTeamSubscriptionCmd>>();
    }

    //------------------------------------//

}//Cls