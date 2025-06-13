using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppComplete;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppComplete;

public class TwoFactorAuthAppCompleteCmdValidatorTests
{
    private readonly TwoFactorAuthAppCompleteRegCmdValidator _validator;

    public TwoFactorAuthAppCompleteCmdValidatorTests()
    {
        _validator = new TwoFactorAuthAppCompleteRegCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_PROVIDER_Is_Null()
    {
        // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new TwoFactorAuthAppCompleteRegCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_TwoFactorCode_Is_Empty()
    {
        // Arrange
        var dto = new TwoFactorAuthAppCompleteRegDto(string.Empty, "secret");
        var command = new TwoFactorAuthAppCompleteRegCmd(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.TwoFactorCode);
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_CustomerSecretKey_Is_Empty()
    {
        // Arrange
        var dto = new TwoFactorAuthAppCompleteRegDto("123456", string.Empty);
        var command = new TwoFactorAuthAppCompleteRegCmd(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto.CustomerSecretKey);
    }

    //------------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_Dto_Is_Valid()
    {
        // Arrange
        var dto = new TwoFactorAuthAppCompleteRegDto("123456", "secret");
        var command = new TwoFactorAuthAppCompleteRegCmd(dto);
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }


    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {

        var validator = new TwoFactorAuthAppCompleteRegCmdValidator();
        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<TwoFactorAuthAppCompleteRegCmd>>();
    }

    //------------------------------------//


}
