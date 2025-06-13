using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Features.Teams.Cmd.Dvcs;
using ID.Application.Features.Teams.Cmd.Dvcs.UpdateDevice;
using ID.Application.Tests.Features.Utility;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Tests.Features.Teams.Cmd.@Dvcs.Update;

public class UpdateDeviceCmdValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new UpdateDeviceCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new UpdateDeviceCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoDeviceId()
    {
        // Arrange
        var validator = new UpdateDeviceCmdValidator();
        var dto = new DeviceDto()
        {
            Id = null,
            UniqueId = "lkjlskdjflksdjf",
            SubscriptionId = Guid.NewGuid()
        };
        var command = new UpdateDeviceCmd(dto);
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Id);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidatioFailure_WhenDtoHasNoSubscriptionId()
    {
        // Arrange
        var validator = new UpdateDeviceCmdValidator();
        var dto = new DeviceDto()
        {
            Id = Guid.NewGuid(),
            UniqueId = "UniqueId",
            Name = "SomeName",
            SubscriptionId = default
        };
        var command = new UpdateDeviceCmd(dto);
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
        var validator = new UpdateDeviceCmdValidator();
        var dto = new DeviceDto()
        {
            Id = Guid.NewGuid(),
            UniqueId = "UniqueId",
            Name = "SomeName",
            SubscriptionId = Guid.NewGuid()
        };
        var command = new UpdateDeviceCmd(dto);
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsValid_Without_DeviceId()
    {
        // Arrange
        var validator = new UpdateDeviceCmdValidator();
        var dto = new DeviceDto()
        {
            Id = null,
            Name = "SomeName",
            UniqueId = "UniqueId",
            SubscriptionId = Guid.NewGuid()
        };

        var command = new UpdateDeviceCmd(dto);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.Id);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDto_HasNoName()
    {
        // Arrange
        var validator = new UpdateDeviceCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var dto = new DeviceDto()
        {
            Id = Guid.NewGuid(),
            Name = null,
            UniqueId = "notnull",
            SubscriptionId = Guid.NewGuid()
        };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        var command = new UpdateDeviceCmd(dto);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        var validator = new UpdateDeviceCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<UpdateDeviceCmd>>();
    }

    //------------------------------------//

}//Cls