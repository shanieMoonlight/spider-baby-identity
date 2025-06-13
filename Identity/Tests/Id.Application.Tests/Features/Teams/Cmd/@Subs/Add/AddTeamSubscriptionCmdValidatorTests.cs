using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Features.Teams.Cmd.Dvcs.UpdateDevice;
using ID.Application.Features.Teams.Cmd.Subs.AddSubscription;
using ID.Application.Tests.Features.Utility;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Tests.Features.Teams.Cmd.@Subs.Add;

public class AddTeamSubscriptionCmdValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new AddTeamSubscriptionCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new AddTeamSubscriptionCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        command.SetAuthenticated_MNTC();

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
        var validator = new AddTeamSubscriptionCmdValidator();
        var command = new AddTeamSubscriptionCmd(new AddTeamSubscriptionDto(default, Guid.NewGuid(), null));
        command.SetAuthenticated_MNTC();
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
        var validator = new AddTeamSubscriptionCmdValidator();
        var command = new AddTeamSubscriptionCmd(new AddTeamSubscriptionDto(Guid.NewGuid(), default, null));
        command.SetAuthenticated_MNTC();
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
        var validator = new AddTeamSubscriptionCmdValidator();
        var command = new AddTeamSubscriptionCmd(new AddTeamSubscriptionDto(Guid.NewGuid(), Guid.NewGuid(), null));
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
        var validator = new AddTeamSubscriptionCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<AddTeamSubscriptionCmd>>();
    }

    //------------------------------------//

}//Cls