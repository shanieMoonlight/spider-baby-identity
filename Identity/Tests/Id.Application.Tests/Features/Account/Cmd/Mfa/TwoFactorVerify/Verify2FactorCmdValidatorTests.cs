using FluentValidation;
using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.Mfa.TwoFactorVerify;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.Mfa.TwoFactorVerify;

public class Verify2FactorCmdValidatorTests
{
    private readonly Verify2FactorCmdValidator _validator;

    public Verify2FactorCmdValidatorTests()
    {
        _validator = new Verify2FactorCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Dto_Is_Null()
    {
        // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new Verify2FactorCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto)
            .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_Dto_Has_Valid_CodeAndToken()
    {
        // Arrange
        var dto = new Verify2FactorDto
        {
            Code = "CODECODECODECODE",
            Token = "TokenTokenTokenToken"
        };
        var command = new Verify2FactorCmd(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Dto_Code_Is_Null()
    {
        // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var dto = new Verify2FactorDto
        {
            Code = null,
            Token = "TokenTokenTokenToken"
        };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new Verify2FactorCmd(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrors();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Dto_Code_Is_Empty()
    {
        // Arrange
        var dto = new Verify2FactorDto
        {
            Code = "   ",
            Token = "TokenTokenTokenToken"
        };
        var command = new Verify2FactorCmd(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrors();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Dto_Token_Is_Null()
    {
        // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var dto = new Verify2FactorDto
        {
            Token = null,
            Code = "CodeCodeCodeCode"
        };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new Verify2FactorCmd(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrors();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Dto_Token_Is_Empty()
    {
        // Arrange
        var dto = new Verify2FactorDto
        {
            Token = "   ",
            Code = "CodeCodeCodeCodeCode"
        };
        var command = new Verify2FactorCmd(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrors();
    }

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        var validator = new Verify2FactorCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AbstractValidator<Verify2FactorCmd>>();
    }


}//Cls
