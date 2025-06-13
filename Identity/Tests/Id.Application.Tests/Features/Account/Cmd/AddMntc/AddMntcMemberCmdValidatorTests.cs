using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.AddMntcMember;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.AddMntc;

/// <summary>
/// Tests for non auth-validation
/// </summary>
public class AddMntcMemberCmdValidatorTests
{
    private readonly AddMntcMemberCmdValidator _validator;

    public AddMntcMemberCmdValidatorTests()
    {
        _validator = new AddMntcMemberCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Dto_Is_Null()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var model = new AddMntcMemberCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var Dto = new AddMntcMemberDto { Email = string.Empty };
        var model = new AddMntcMemberCmd(Dto);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Dto.Email);
    }

    //------------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_Dto_And_Email_Are_Valid()
    {
        var Dto = new AddMntcMemberDto { Email = "test@example.com" };
        var model = new AddMntcMemberCmd(Dto);
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Dto.Email);
    }

    //------------------------------------//

    [Fact]
    public void AddMntcMemberCmd_Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new AddMntcMemberCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<AddMntcMemberCmd>>();
    }

    //------------------------------------//

}//Cls