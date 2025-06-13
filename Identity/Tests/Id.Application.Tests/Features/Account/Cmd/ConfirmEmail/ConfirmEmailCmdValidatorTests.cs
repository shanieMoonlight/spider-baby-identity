using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.ConfirmEmail;

namespace ID.Application.Tests.Features.Account.Cmd.ConfirmEmail;
public class ConfirmEmailCmdValidatorTests
{
    private readonly ConfirmEmailCmdValidator _validator;

    public ConfirmEmailCmdValidatorTests()
    {
        _validator = new ConfirmEmailCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_DTO_is_null()
    {
        //Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        ConfirmEmailCmd cmd = new(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_AllIdentifiersAreMissing()
    {
        //Arrange
        ConfirmEmailCmd cmd = new(new ConfirmEmailDto(null, "") { });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.UserId);
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.ConfirmationToken);

    }


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_NoTokenSupplied()
    {
        //Arrange
        ConfirmEmailCmd cmd = new(new ConfirmEmailDto(new Guid(), ""));


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.ConfirmationToken);

    }


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_NoIdSupplied()
    {
        //Arrange
        ConfirmEmailCmd cmd = new(new ConfirmEmailDto(null, "sfgyiyujdfbgst"));


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.UserId);

    }


    //------------------------------------//

    [Fact]
    public void Should_NOT_have_DTO_error_when_IDAndTOkenSupplied()
    {
        //Arrange
        ConfirmEmailCmd cmd = new(new ConfirmEmailDto(new Guid(), "sfgyiyujdfbgst"));


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//


}//Cls
