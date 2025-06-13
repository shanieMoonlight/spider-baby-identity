using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.AddSprMember;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.AddSpr;

/// <summary>
/// Tests for non auth-validation
/// </summary>
public class AddSprMemberCmdValidatorTests
{
    private readonly AddSprMemberCmdValidator _validator;

    public AddSprMemberCmdValidatorTests()
    {
        _validator = new AddSprMemberCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Dto_Is_Null()
    {
        // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var model = new AddSprMemberCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var Dto = new AddSprMemberDto { Email = string.Empty };
        var model = new AddSprMemberCmd(Dto);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Dto.Email);
    }

    //------------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_Dto_And_Email_Are_Valid()
    {
        var Dto = new AddSprMemberDto { Email = "test@example.com" };
        var model = new AddSprMemberCmd(Dto);
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Email);
    }

    //------------------------------------//

    [Fact]
    public void AddSprMemberCmd_Implements_ASprMinimumValidator()
    {
        // Arrange
        var validator = new AddSprMemberCmdValidator();

        // Act & Assert
        Assert.IsType<ASuperMinimumValidator<AddSprMemberCmd>>(validator, exactMatch: false);

        // Act & Assert
        validator.ShouldBeAssignableTo<ASuperMinimumValidator<AddSprMemberCmd>>();
    }

    //------------------------------------//

}//Cls