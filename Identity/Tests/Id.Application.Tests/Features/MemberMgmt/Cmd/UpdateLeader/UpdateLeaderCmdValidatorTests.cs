using FluentValidation.TestHelper;
using ID.Application.Features.MemberMgmt.Cmd.UpdateMyTeamLeader;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Cmd.UpdateLeader;

public class UpdateLeaderCmdValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenIdIsNull()
    {
        // Arrange
        var validator = new UpdateMyTeamLeaderCmdValidator();
        var command = new UpdateMyTeamLeaderCmd(default); ;
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.NewLeaderId)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenIdIsValid()
    {
        // Arrange
        var validator = new UpdateMyTeamLeaderCmdValidator();
        var command = new UpdateMyTeamLeaderCmd(Guid.NewGuid());
        command.SetAuthenticated_CUSTOMER();

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
        var validator = new UpdateMyTeamLeaderCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<UpdateMyTeamLeaderCmd>>();
    }

    //------------------------------------//

}//Cls