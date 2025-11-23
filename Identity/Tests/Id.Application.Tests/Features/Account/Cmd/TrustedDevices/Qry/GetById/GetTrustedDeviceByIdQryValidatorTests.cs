using ID.Application.Features.Account.TrustedDevices.Qry.GetById;
using ID.Application.Tests.Features.Utility;

namespace ID.Application.Tests.Features.Account.Cmd.TrustedDevices.Qry.GetById;

public class GetTrustedDeviceByIdQryValidatorTests
{
    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenIdIsDefault()
    {
        // Arrange
        var validator = new GetTrustedDeviceByIdQryValidator();
        var command = new GetTrustedDeviceByIdQry(default);
        command.SetAuthenticated_MNTC();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //--------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenIdIsValid()
    {
        // Arrange
        var validator = new GetTrustedDeviceByIdQryValidator();
        var command = new GetTrustedDeviceByIdQry(Guid.NewGuid());
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
        var validator = new GetTrustedDeviceByIdQryValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<GetTrustedDeviceByIdQry>>();
    }

    //--------------------------//

}
