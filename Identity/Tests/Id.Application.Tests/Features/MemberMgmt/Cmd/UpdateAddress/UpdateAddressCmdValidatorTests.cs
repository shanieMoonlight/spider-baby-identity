using FluentValidation.TestHelper;
using ID.Application.Features.MemberMgmt.Cmd.UpdateAddress;
using ID.Application.Features.MemberMgmt.Cmd.UpdateSelf;
using ID.Application.Mediatr.Validation;
using ID.Application.Tests.Features.Utility;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories.Dtos;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Cmd.UpdateAddress;

public class UpdateAddressCmdValidatorTests
{
    private readonly UpdateAddressCmdValidator _validator;

    public UpdateAddressCmdValidatorTests()
    {
        _validator = new UpdateAddressCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Dto_Is_Null()
    {
        // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var command = new UpdateAddressCmd(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto)
              .WithErrorMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Line1_Is_Empty()
    {
        // Arrange
        var dto = IdentityAddressDataFactoryDto.Create(Guid.NewGuid(), line1: string.Empty);
        var command = new UpdateAddressCmd (dto);
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.Line1);
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_Line2_Is_Empty()
    {
        // Arrange
        var dto = IdentityAddressDataFactoryDto.Create(Guid.NewGuid(), line2: string.Empty);
        var command = new UpdateAddressCmd(dto);
        command.SetAuthenticated_CUSTOMER();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.Line2);
    }

    //------------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_Dto_Is_Valid()
    {
        // Arrange
        var dto = IdentityAddressDataFactoryDto.Create(Guid.NewGuid(), line1: "line1", line2: "line2");
        var command = new UpdateAddressCmd(dto);
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
        // Arrange
        var validator = new UpdateSelfCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<UpdateSelfCmd>>();
    }

    //------------------------------------//

}//Cls
