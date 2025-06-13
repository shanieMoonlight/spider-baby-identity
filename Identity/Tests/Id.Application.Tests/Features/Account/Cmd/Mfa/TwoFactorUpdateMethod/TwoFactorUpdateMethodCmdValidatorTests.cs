using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;
using ID.Application.Features.Account.Cmd.ResendEmailConfirmationPrincipal;
using ID.GlobalSettings.Utility;
using ID.Application.Mediatr.Validation;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorUpdateMethod;

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
    public void Should_Have_Error_When_PROVIDER_Is_Null()
    {
        // Arrange
        var command = new TwoFactorUpdateMethodCmd(null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Provider);
    }

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {

        var validator = new TwoFactorUpdateMethodCmdValidator();
        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<TwoFactorUpdateMethodCmd>>();
    }

    //------------------------------------//


}
