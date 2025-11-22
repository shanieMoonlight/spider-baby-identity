using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.TrustedDevices.RevokeByFingerPrint;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.RevokeByFingerPrint;

public class RevokeTrustedDeviceByFingerprintCmdValidatorTests
{
    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDtoIsNull()
    {
        // Arrange
        var validator = new RevokeTrustedDeviceByFingerprintCmdValidator();
        var command = new RevokeTrustedDeviceByFingerprintCmd(default);
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
        var validator = new RevokeTrustedDeviceByFingerprintCmdValidator();
        var dto = new RevokeTrustedDeviceByFingerprintDto("fp-1");
        var command = new RevokeTrustedDeviceByFingerprintCmd(dto);
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
        var validator = new RevokeTrustedDeviceByFingerprintCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<RevokeTrustedDeviceByFingerprintCmd>>();
    }

    //--------------------------//

}
