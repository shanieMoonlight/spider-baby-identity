using FluentValidation.TestHelper;
using ID.Application.Features.MemberMgmt.Cmd.DeleteSuperMember;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Cmd.DeleteSuperMember;

public class DeleteSprMemberCmdValidatorTests
{


    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenIdIsNull()
    {
        // Arrange
        var validator = new DeleteSprMemberCmdValidator();
        var command = new DeleteSprMemberCmd(default); ;
        command.SetAuthenticated_SUPER();

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
        var validator = new DeleteSprMemberCmdValidator();
        var command = new DeleteSprMemberCmd(Guid.NewGuid());
        command.SetAuthenticated_SUPER();

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
        var validator = new DeleteSprMemberCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<ASuperMinimumValidator<DeleteSprMemberCmd>>();
    }

    //------------------------------------//

}//Cls