using FluentValidation.TestHelper;
using ID.Application.Features.MemberMgmt.Cmd.UpdateLeaderMntc;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Cmd.UpdateLeader;

public class UpateTeamLeaderCmdValidatorTests
{
    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new UpdateTeamLeaderCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new UpdateTeamLeaderCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoIsMissingNewTheLeaderId()
    {
        // Arrange
        var validator = new UpdateTeamLeaderCmdValidator();
        var command = new UpdateTeamLeaderCmd(new UpdateTeamLeaderDto(Guid.Empty, Guid.Empty));

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
        var validator = new UpdateTeamLeaderCmdValidator();
        var command = new UpdateTeamLeaderCmd(new UpdateTeamLeaderDto(Guid.NewGuid(), Guid.NewGuid()));
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
        var validator = new UpdateTeamLeaderCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<UpdateTeamLeaderCmd>>();
    }

    //------------------------------------//

}//Cls