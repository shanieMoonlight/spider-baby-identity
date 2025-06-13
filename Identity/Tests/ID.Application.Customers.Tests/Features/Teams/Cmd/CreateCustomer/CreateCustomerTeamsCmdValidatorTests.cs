using FluentValidation.TestHelper;
using ID.Application.Customers.Tests.Features.Utility;
using ID.Application.Features.Teams;
using ID.Application.Features.Teams.Cmd.Create;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Teams.Cmd.CreateCustomer;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

public class CreateCustomerTeamsCmdValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new CreateCustomerTeamCmdValidator();
        var command = new CreateCustomerTeamCmd(null);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoIsNotValid()
    {
        // Arrange
        var validator = new CreateCustomerTeamCmdValidator();
        var command = new CreateCustomerTeamCmd(new TeamDto());
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
        var validator = new CreateCustomerTeamCmdValidator();
        var command = new CreateCustomerTeamCmd(new TeamDto() { Name = "ThisIsTheName" });
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
        var validator = new CreateCustomerTeamCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<CreateCustomerTeamCmd>>();
    }

    //------------------------------------//

}//Cls