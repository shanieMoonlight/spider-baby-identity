using FluentValidation.TestHelper;
using ID.Application.Features.System.Cmd.Init;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Mntc.Init;

/// <summary>
/// Tests for non auth-validation
/// </summary>
public class InitializeCmdValidatorTests
{
    private readonly InitializeCmdValidator _validator;

    //- - - - - - - - - - - - - - - - - - //

    public InitializeCmdValidatorTests()
    {
        _validator = new InitializeCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void ShouldHaveValidationErrorWhenDtoIsNull()
    {
        // Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var result = _validator.TestValidate(new InitializeCmd(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto);
    }

    //------------------------------------//

    [Fact]
    public void ShouldHaveValidationErrorWhenDtoPasswordIsNull()
    {
        // Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var dto = new InitializeDto(null, null); // Password is null
        var result = _validator.TestValidate(new InitializeCmd(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Assert
        result.ShouldHaveValidationErrors();
    }

    //------------------------------------//

    [Fact]
    public void ShouldHaveValidationErrorWhenDtoPasswordIsWhitespace()
    {
        // Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var dto = new InitializeDto("  ", null); // Password is null
        var result = _validator.TestValidate(new InitializeCmd(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Assert
        result.ShouldHaveValidationErrors();
    }

    //------------------------------------//

    [Fact]
    public void DowsNotRequireAuthentication()
    {
        // Arrange
        var validator = new InitializeCmdValidator();

        // Act & Assert
        validator.ShouldNotBeAssignableTo<IsAuthenticatedValidator<InitializeCmd>>();
    }

    //------------------------------------//

}//Cls
