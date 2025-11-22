using ID.Application.Features.Account.Cmd.TrustedDevices.Cmd.Revoke;
using ID.Application.Tests.Features.Utility;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Revoke;

public class RevokeTrustedDeviceCmdValidatorTests
{
    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new RevokeTrustedDeviceCmdValidator();
        var command = new RevokeTrustedDeviceCmd(default);
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //--------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenIdIsNull()
    {
        // Arrange
        var validator = new RevokeTrustedDeviceCmdValidator();
        var dto = new RevokeTrustedDeviceDto(default);
        var command = new RevokeTrustedDeviceCmd(dto);
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Dto.DeviceId);
              //.WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //--------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsValid()
    {
        // Arrange
        var validator = new RevokeTrustedDeviceCmdValidator();
        var dto = new RevokeTrustedDeviceDto(Guid.NewGuid());
        var command = new RevokeTrustedDeviceCmd(dto);
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    //--------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        var validator = new RevokeTrustedDeviceCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<RevokeTrustedDeviceCmd>>();
    }

    //--------------------------//

}
