using FluentValidation.TestHelper;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorUpdateMethod;

public class TwoFactorUpdateMethodCmdValidatorTests
{
    private readonly TwoFactorUpdateMethodCmdValidator _validator;

    public TwoFactorUpdateMethodCmdValidatorTests()
    {
        _validator = new TwoFactorUpdateMethodCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_DTO_Is_Null()
    {
        // Arrange
        var command = new UpdateTwoFactorProviderCmd(null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenIdIsNull()
    {
        // Arrange
        var dto = new UpdateTwoFactorProviderDto( (TwoFactorProvider)0);
        var command = new UpdateTwoFactorProviderCmd(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Dto.Provider);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenProviderIsProvided()
    {
        // Arrange
        var dto = new UpdateTwoFactorProviderDto(TwoFactorProvider.Email);
        var command = new UpdateTwoFactorProviderCmd(dto);
        command.SetAuthenticated_MNTC();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//


    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {

        var validator = new TwoFactorUpdateMethodCmdValidator();
        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<UpdateTwoFactorProviderCmd>>();
    }

    //------------------------------------//


}
