using ID.Application.Tests.Features.Utility;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Cmd.Trust;

public class TrustDeviceCmdValidatorTests
{
    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new TrustDeviceCmdValidator();
        var command = new TrustDeviceCmd(default);
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
    public void Validate_ShouldReturnValidationSuccess_WhenDtoIsValid()
    {
        // Arrange
        var validator = new TrustDeviceCmdValidator();
        var dto = new TrustDeviceCreateDto("fp","name");
        var command = new TrustDeviceCmd(dto);
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    //--------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenFingerprintIsWhitespace()
    {
        // Arrange
        var validator = new TrustDeviceCmdValidator();
        var dto = new TrustDeviceCreateDto("     ", "name");
        var command = new TrustDeviceCmd(dto);
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Contains("Fingerprint"));
        //.WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //--------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenFingerPrintIsNull()
    {
        // Arrange
        var validator = new TrustDeviceCmdValidator();
        var dto = new TrustDeviceCreateDto(null, "name");
        var command = new TrustDeviceCmd(dto);
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Contains("Fingerprint"));
        //.WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //--------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        var validator = new TrustDeviceCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<TrustDeviceCmd>>();
    }

    //--------------------------//

}
