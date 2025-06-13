using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorResend;

public class Resend2FactorCmdValidatorTests
{
    private readonly Resend2FactorCmdValidator _validator;

    public Resend2FactorCmdValidatorTests()
    {
        _validator = new Resend2FactorCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Dto_Is_Null()
    {
        // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new Resend2FactorCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto)
            .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Dto_Has_No_Valid_Properties()
    {
        // Arrange
        var dto = new Resend2FactorDto { Email = null, Username = null, UserId = null };
        var command = new Resend2FactorCmd(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto)
            .WithErrorMessage("You must supply at least one of [Username, UserId or Email]");
    }

    //------------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_Dto_Has_Valid_Email()
    {
        // Arrange
        var dto = new Resend2FactorDto { Email = "test@example.com", Username = null, UserId = null };
        var command = new Resend2FactorCmd(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Dto);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Dto_Has_Valid_Username()
    {
        // Arrange
        var dto = new Resend2FactorDto { Email = null, Username = "testuser", UserId = null };
        var command = new Resend2FactorCmd(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Dto);
    }

    //------------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_Dto_Has_Valid_UserId()
    {
        // Arrange
        var dto = new Resend2FactorDto { Email = null, Username = null, UserId = Guid.NewGuid() };
        var command = new Resend2FactorCmd(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Dto);
    }


    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        var validator = new Resend2FactorCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<Resend2FactorCmd>>();
    }

    //------------------------------------//
}
