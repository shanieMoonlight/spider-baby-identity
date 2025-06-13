using FluentValidation.TestHelper;
using ID.Application.Customers.Tests.Features.Utility;
using ID.Application.Features.Teams.Cmd.Subs.AddSubscription;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.MemberMgmt.Qry.GetCustomers;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
public class AddTeamSubscriptionCmdValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new AddTeamSubscriptionCmdValidator();
        var command = new AddTeamSubscriptionCmd(null);
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