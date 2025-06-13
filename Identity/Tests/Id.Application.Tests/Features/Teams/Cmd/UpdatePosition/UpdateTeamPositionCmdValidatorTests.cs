using FluentValidation.TestHelper;
using ID.Application.Features.Teams.Cmd.UpdatePositionRange;
using ID.Application.Mediatr.Validation;
using ID.Domain.Entities.AppUsers;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Cmd.UpdatePosition;

public class UpdateTeamPositionCmdValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new UpdateTeamPositionRangeCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new UpdateTeamPositionRangeCmd(null);
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
    public void Validate_ShouldReturnValidatioFailure_Min_GREATER_THAN_Max()
    {
        // Arrange
        var validator = new UpdateTeamPositionRangeCmdValidator();
        var dto = new UpdateTeamPositionRangeDto(5, 3);
        var command = new UpdateTeamPositionRangeCmd(dto);
        command.SetAuthenticated_MNTC();
        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(v => v.IsLeader);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsValid()
    {
        // Arrange
        var validator = new UpdateTeamPositionRangeCmdValidator();
        var dto = new UpdateTeamPositionRangeDto(5, 13);
        var command = new UpdateTeamPositionRangeCmd(dto);
        command.SetAuthenticated_MNTC<UpdateTeamPositionRangeCmd, AppUser>(true);

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
        var validator = new UpdateTeamPositionRangeCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsLeaderValidator<UpdateTeamPositionRangeCmd>>();
    }

    //------------------------------------//

}//Cls