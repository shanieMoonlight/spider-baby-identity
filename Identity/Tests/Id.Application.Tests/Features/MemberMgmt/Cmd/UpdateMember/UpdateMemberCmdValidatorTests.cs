using FluentValidation.TestHelper;
using ID.Application.Features.MemberMgmt.Cmd.UpdateSelf;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Cmd.UpdateMember;

public class UpdateMemberCmdValidatorTests
{

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenDTOIsNull()
    {
        // Arrange
        var validator = new UpdateSelfCmdValidator();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new UpdateSelfCmd(default);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//
    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenIdIsNull()
    {
        // Arrange
        var dto = new UpdateSelfDto()
        {
            Id = default,
        };
        var validator = new UpdateSelfCmdValidator();
        var command = new UpdateSelfCmd(dto);
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Dto.Id);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenIdIsValid()
    {
        // Arrange
        var dto = new UpdateSelfDto()
        {
            Id = Guid.NewGuid(),
        };
        var validator = new UpdateSelfCmdValidator();
        var command = new UpdateSelfCmd(dto);
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        var validator = new UpdateSelfCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<UpdateSelfCmd>>();
    }

    //------------------------------------//

}//Cls