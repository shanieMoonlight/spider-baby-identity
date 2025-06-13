using FluentValidation.TestHelper;
using ID.Application.Customers.Tests.Features.Utility;
using ID.Application.Features.Teams.Cmd.Delete;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Teams.Cmd.DeleteCustomer;

public class DeleteCustomerTeamCmdValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenIdIsNull()
    {
        // Arrange
        var validator = new DeleteCustomerTeamCmdValidator();
        var command = new DeleteCustomerTeamCmd(default); ;
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenIdIsValid()
    {
        // Arrange
        var validator = new DeleteCustomerTeamCmdValidator();
        var command = new DeleteCustomerTeamCmd(Guid.NewGuid());
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
        var validator = new DeleteCustomerTeamCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<DeleteCustomerTeamCmd>>();
    }

    //------------------------------------//

}//Cls