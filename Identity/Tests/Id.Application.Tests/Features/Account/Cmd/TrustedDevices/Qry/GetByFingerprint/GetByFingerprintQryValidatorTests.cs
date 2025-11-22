using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetByFingerprint;
using ID.Application.Features.Account.Cmd.TrustedDevices.Qry.GetByName;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Qry.GetByFingerprint;

public class GetByFingerprintQryValidatorTests
{
    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenFingerprintIsNullOrEmpty()
    {
        // Arrange
        var validator = new GetByFingerprintQryValidator();
        var command = new GetTrustedDeviceByFingerprintQry("" );
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.DeviceFingerprint)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //--------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenFingerprintIsValid()
    {
        // Arrange
        var validator = new GetByFingerprintQryValidator();
        var command = new GetTrustedDeviceByFingerprintQry("fp-1");
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
        var validator = new GetByFingerprintQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<GetTrustedDeviceByFingerprintQry>>();
    }

    //--------------------------//

}
