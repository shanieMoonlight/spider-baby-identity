using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorGoogleComplete;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.Reg.MfaGoogleCompleteReg;

public class MfaGoogleCompleteRegistrationCmdValidatorTests
{
    private readonly MfaGoogleCompleteRegistrationCmdValidator _validator;

    public MfaGoogleCompleteRegistrationCmdValidatorTests()
    {
        _validator = new MfaGoogleCompleteRegistrationCmdValidator();
    }

    //------------------------------------//


    [Fact]
    public void Should_Have_Error_When_TwoFactorCode_Is_Empty()
    {
        // Arrange
        var command = new MfaGoogleCompleteRegistrationCmd(string.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.TwoFactorCode);
    }

    //------------------------------------//


    [Fact]
    public void Should_Not_Have_Error_When_TwoFactorCode_Is_Not_Empty()
    {
        // Arrange
        var command = new MfaGoogleCompleteRegistrationCmd("123456");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.TwoFactorCode);
    }

    //------------------------------------//


    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        MfaGoogleCompleteRegistrationCmdValidator validator = new();

        // Act & Assert
        Assert.IsType<IsAuthenticatedValidator<MfaGoogleCompleteRegistrationCmd>>(validator, exactMatch: false);

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<MfaGoogleCompleteRegistrationCmd>>();
    }

    //------------------------------------//

}

//Cls