using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Features.Teams.Cmd.Dvcs.AddDevice;
using ID.Application.Tests.Features.Utility;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Tests.Features.Teams.Cmd.@Dvcs.Add;

public class AddDeviceToTeamCmdValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new AddDeviceToTeamCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new AddDeviceToTeamCmd(null);
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
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoSubscriptionId()
    {
        // Arrange
        var validator = new AddDeviceToTeamCmdValidator();
        var dto = new AddDeviceToTeamDto(default, "name", "description", $"{Guid.NewGuid()}");
        var command = new AddDeviceToTeamCmd(dto);
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.SubscriptionId);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoName()
    {
        // Arrange
        var validator = new AddDeviceToTeamCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var dto = new AddDeviceToTeamDto(default, null, "description", $"{Guid.NewGuid()}");
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new AddDeviceToTeamCmd(dto);
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoName2()
    {
        // Arrange
        var validator = new AddDeviceToTeamCmdValidator();
        var dto = new AddDeviceToTeamDto(default, string.Empty, "description", $"{Guid.NewGuid()}");
        var command = new AddDeviceToTeamCmd(dto);
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoNoUniqueId()
    {
        // Arrange
        var validator = new AddDeviceToTeamCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var dto = new AddDeviceToTeamDto(default, "name", "description", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new AddDeviceToTeamCmd(dto);
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoUniqueId2()
    {
        // Arrange
        var validator = new AddDeviceToTeamCmdValidator();
        var dto = new AddDeviceToTeamDto(default, "name", "description", string.Empty);
        var command = new AddDeviceToTeamCmd(dto);
        command.SetAuthenticated_CUSTOMER();

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
        var validator = new AddDeviceToTeamCmdValidator();
        var dto = new AddDeviceToTeamDto(Guid.NewGuid(), "name", "description", $"{Guid.NewGuid()}");
        var command = new AddDeviceToTeamCmd(dto);
        command.SetAuthenticated_CUSTOMER();

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
        var validator = new AddDeviceToTeamCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<AddDeviceToTeamCmd>>();
    }

    //------------------------------------//

}//Cls