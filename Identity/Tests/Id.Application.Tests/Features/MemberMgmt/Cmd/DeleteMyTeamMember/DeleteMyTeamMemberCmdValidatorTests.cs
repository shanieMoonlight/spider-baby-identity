using FluentValidation.TestHelper;
using ID.Application.Features.MemberMgmt.Cmd.DeleteMember;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Cmd.DeleteMyTeamMember;

public class DeleteMyTeamMemberCmdValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenIdIsNull()
    {
        // Arrange
        var validator = new DeleteMyTeamMemberCmdValidator();
        var command = new DeleteMyTeamMemberCmd(default); ;
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
        var validator = new DeleteMyTeamMemberCmdValidator();
        var command = new DeleteMyTeamMemberCmd(Guid.NewGuid());
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        var validator = new DeleteMyTeamMemberCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<DeleteMyTeamMemberCmd>>();
    }

    //------------------------------------//

}//Cls