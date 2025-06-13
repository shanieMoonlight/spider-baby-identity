using FluentValidation.TestHelper;
using ID.Application.Customers.Features.MemberMgmt.Cmd.DeleteCustomerMember;
using ID.Application.Customers.Mediatr.Validation;
using ID.Application.Customers.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.MemberMgmt.Cmd.DeleteCustomerMember;

public class DeleteCusMemberCmdValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenIdIsNull()
    {
        // Arrange
        var validator = new DeleteCustomerMemberCmdValidator();
        var command = new DeleteCustomerMemberCmd(default); ;
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.UserId)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenIdIsValid()
    {
        // Arrange
        var validator = new DeleteCustomerMemberCmdValidator();
        var command = new DeleteCustomerMemberCmd(Guid.NewGuid());
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Implements_ASuperMinimumValidator()
    {
        // Arrange
        var validator = new DeleteCustomerMemberCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<ACustomerOnlyValidator<DeleteCustomerMemberCmd>>();
    }

    //------------------------------------//

}//Cls