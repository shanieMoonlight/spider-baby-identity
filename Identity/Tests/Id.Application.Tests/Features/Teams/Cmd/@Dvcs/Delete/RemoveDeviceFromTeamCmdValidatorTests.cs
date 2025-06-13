using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Features.Teams.Cmd.Dvcs.AddDevice;
using ID.Application.Features.Teams.Cmd.Dvcs.RemoveDevice;
using ID.Application.Tests.Features.Utility;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Tests.Features.Teams.Cmd.@Dvcs.Delete;

public class RemoveDeviceFromTeamCmdValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new RemoveDeviceFromTeamSubscriptionCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new RemoveDeviceFromTeamSubscriptionCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoDeviceOrSub()
    {
        // Arrange
        var validator = new RemoveDeviceFromTeamSubscriptionCmdValidator();
        var dto = new RemoveDeviceFromTeamSubscriptionDto(default, default);
        var command = new RemoveDeviceFromTeamSubscriptionCmd(dto);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.DeviceId);
        result.ShouldHaveValidationErrorFor(x => x.Dto.SubscriptionId);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoSubscriptionId()
    {
        // Arrange
        var validator = new RemoveDeviceFromTeamSubscriptionCmdValidator();
        var dto = new RemoveDeviceFromTeamSubscriptionDto(default, Guid.NewGuid());
        var command = new RemoveDeviceFromTeamSubscriptionCmd(dto);
        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.SubscriptionId);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsValid()
    {
        // Arrange
        var validator = new RemoveDeviceFromTeamSubscriptionCmdValidator();
        var dto = new RemoveDeviceFromTeamSubscriptionDto(Guid.NewGuid(), Guid.NewGuid());
        var command = new RemoveDeviceFromTeamSubscriptionCmd(dto);
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIs_Without_DeviceId()
    {
        // Arrange
        var validator = new RemoveDeviceFromTeamSubscriptionCmdValidator();
        var dto = new RemoveDeviceFromTeamSubscriptionDto(Guid.NewGuid(), default);
        var command = new RemoveDeviceFromTeamSubscriptionCmd(dto);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new RemoveDeviceFromTeamSubscriptionCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<RemoveDeviceFromTeamSubscriptionCmd>>();
    }

    //------------------------------------//
}//Cls