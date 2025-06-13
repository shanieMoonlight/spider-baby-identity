using FluentValidation.TestHelper;
using ID.Application.Features.Teams;
using ID.Application.Features.Teams.Cmd.Update;
using ID.Application.Mediatr.Validation;
using ID.Domain.Entities.AppUsers;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.Teams.Cmd.Update;

public class UpateTeamCmdValidatorTests
{

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new UpdateTeamCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new UpdateTeamCmd(null);
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
    public void Validate_ShouldReturnValidationFailure_WhenIdIsNull()
    {
        // Arrange
        var validator = new UpdateTeamCmdValidator();
        var command = new UpdateTeamCmd(new TeamDto() { Name = "ThisIsTheName" }); //No ID

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Dto.Id);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoIsNotValid()
    {
        // Arrange
        var validator = new UpdateTeamCmdValidator();
        var command = new UpdateTeamCmd(new TeamDto());
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
        var validator = new UpdateTeamCmdValidator();
        var command = new UpdateTeamCmd(new TeamDto() { Id = Guid.NewGuid(), Name = "ThisIsTheName" });
        command.SetAuthenticated_MNTC<UpdateTeamCmd, AppUser>(true);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//


    [Fact]
    public void Should_Have_Error_When_IsLeader_Is_False()
    {
        var model = new UpdateTeamCmd(new TeamDto()) { IsLeader = false };
        var validator = new UpdateTeamCmdValidator();
        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.IsLeader)
              .WithErrorMessage(IDMsgs.Error.Teams.ONlY_LEADER_CAN_UPDATE);
    }

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        var validator = new UpdateTeamCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsLeaderValidator<UpdateTeamCmd>>();
    }

    //------------------------------------//

}//Cls