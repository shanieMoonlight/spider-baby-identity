using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthyComplete;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthyCompleteReg;

public class TwoFactorAuthyCompleteRegCmdValidatorTests
{
    private readonly TwoFactorAuthyCompleteCmdValidator _validator;

    public TwoFactorAuthyCompleteRegCmdValidatorTests()
    {
        _validator = new TwoFactorAuthyCompleteCmdValidator();
    }

    //------------------------------------//


    [Fact]
    public void Should_Have_Error_When_TwoFactorCode_Is_Empty()
    {
        // Arrange
        var command = new TwoFactorAuthyCompleteRegCmd(string.Empty);

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
        var command = new TwoFactorAuthyCompleteRegCmd("123456");

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
        TwoFactorAuthyCompleteCmdValidator validator = new();

        // Act & Assert
        Assert.IsType<IsAuthenticatedValidator<TwoFactorAuthyCompleteRegCmd>>(validator, exactMatch: false);

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<TwoFactorAuthyCompleteRegCmd>>();
    }

    //------------------------------------//

}


