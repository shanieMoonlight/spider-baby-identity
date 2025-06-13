using FluentValidation.TestHelper;
using ID.Application.Features.MemberMgmt.Cmd.DeleteMntcMember;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Cmd.DeleteMntcMember;

public class DeleteMntcMemberCmdValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenIdIsNull()
    {
        // Arrange
        var validator = new DeleteMntcMemberCmdValidator();
        var command = new DeleteMntcMemberCmd(default); ;
        command.SetAuthenticated_MNTC();

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
        var validator = new DeleteMntcMemberCmdValidator();
        var command = new DeleteMntcMemberCmd(Guid.NewGuid());
        command.SetAuthenticated_MNTC();

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
        var validator = new DeleteMntcMemberCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcOnlyValidator<DeleteMntcMemberCmd>>();
    }

    //------------------------------------//

}//Cls