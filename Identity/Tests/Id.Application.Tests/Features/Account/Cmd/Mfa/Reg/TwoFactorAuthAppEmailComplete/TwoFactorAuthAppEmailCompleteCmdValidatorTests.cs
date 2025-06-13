using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppEmailComplete;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppEmailComplete;

public class TwoFactorAuthAppEmailCompleteCmdValidatorTests
{
    private readonly TwoFactorAuthAppEmailCompleteCmdValidator _validator;

    public TwoFactorAuthAppEmailCompleteCmdValidatorTests()
    {
        _validator = new TwoFactorAuthAppEmailCompleteCmdValidator();
    }

    //------------------------------------//


    [Fact]
    public void Should_Have_Error_When_TwoFactorCode_Is_Empty()
    {
        // Arrange
        var command = new TwoFactorAuthAppEmailCompleteCmd(string.Empty);

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
        var command = new TwoFactorAuthAppEmailCompleteCmd("123456");

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
        TwoFactorAuthAppEmailCompleteCmdValidator validator = new();

        // Act & Assert
        Assert.IsType<IsAuthenticatedValidator<TwoFactorAuthAppEmailCompleteCmd>>(validator, exactMatch: false);

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<TwoFactorAuthAppEmailCompleteCmd>>();
    }

    //------------------------------------//

}//Cls