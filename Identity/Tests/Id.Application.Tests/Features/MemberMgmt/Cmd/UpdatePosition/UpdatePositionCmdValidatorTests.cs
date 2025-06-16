using FluentValidation.TestHelper;
using ID.Application.Features.MemberMgmt.Cmd.UpdatePosition;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Cmd.DeleteCustomerMember;

public class UpdatePositionCmdValidatorTests
{
    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenISCustomer()
    {
        // Arrange
        var validator = new UpdatePositionCmdValidator();
        var dto = new UpdatePositionDto(default, 5);
        var command = new UpdatePositionCmd(dto);
        command.SetAuthenticated_CUSTOMER();
        command.IsLeader = true;

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Dto.UserId);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationFailure_WhenIdIsNull()
    {
        // Arrange
        var validator = new UpdatePositionCmdValidator();
        var dto = new UpdatePositionDto(default, 5);
        var command = new UpdatePositionCmd(dto);
        command.SetAuthenticated_MNTC();
        command.IsLeader = true;

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Dto.UserId);
    }

    //------------------------------------//

    [Fact]
    public void Validate_ShouldReturnValidationSuccess_WhenIdIsValid()
    {
        // Arrange
        var validator = new UpdatePositionCmdValidator();
        var dto = new UpdatePositionDto(Guid.NewGuid(), 5);
        var command = new UpdatePositionCmd(dto);
        command.SetAuthenticated_MNTC();
        command.IsLeader = true;

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
        var validator = new UpdatePositionCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsMntcMinimumLeaderValidator<UpdatePositionCmd>>();
    }

    //------------------------------------//

}//Cls