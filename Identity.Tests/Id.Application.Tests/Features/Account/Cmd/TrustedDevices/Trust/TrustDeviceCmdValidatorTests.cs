using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.TrustedDevices.Trust;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Trust;

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
        var dto = new TrustDeviceCreateDto("fp","name", null, null);
        var command = new TrustDeviceCmd(dto);
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
        var validator = new TrustDeviceCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<TrustDeviceCmd>>();
    }

    //--------------------------//

}
