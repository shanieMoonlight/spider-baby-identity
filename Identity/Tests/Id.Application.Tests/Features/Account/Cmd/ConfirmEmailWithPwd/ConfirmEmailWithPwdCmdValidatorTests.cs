using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.ConfirmEmailWithPwd;

namespace ID.Application.Tests.Features.Account.Cmd.ConfirmEmailWithPwd;
public class ConfirmEmailWithPwdCmdValidatorTests
{
    private readonly ConfirmEmailWithPwdCmdValidator _validator;

    public ConfirmEmailWithPwdCmdValidatorTests()
    {
        _validator = new ConfirmEmailWithPwdCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_DTO_is_null()
    {
        //Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        ConfirmEmailWithPwdCmd cmd = new(null);
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
        ConfirmEmailWithPwdCmd cmd = new(new ConfirmEmailWithPwdDto(null, "", "", "") { });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.UserId);
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.ConfirmationToken);
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.ConfirmPassword);
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.Password);

    }


    //------------------------------------//
    [Fact]
    public void Should_have_error_when_NoTokenSupplied()
    {
        //Arrange
        ConfirmEmailWithPwdCmd cmd = new(new ConfirmEmailWithPwdDto(new Guid(), "", "NEWPWD", "NEWPWD"));


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
        ConfirmEmailWithPwdCmd cmd = new(new ConfirmEmailWithPwdDto(null, "sfgyiyujdfbgst", "NEWPWD", "NEWPWD"));


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.UserId);

    }


    //------------------------------------//
    [Fact]
    public void Should_have_error_when_NoPasswordSupplied()
    {
        //Arrange
        ConfirmEmailWithPwdCmd cmd = new(new ConfirmEmailWithPwdDto(new Guid(), "sfgyiyujdfbgst", "", "NEWPWD"));


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.Password);

    }


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_NoConfirmPasswordSupplied()
    {
        //Arrange
        ConfirmEmailWithPwdCmd cmd = new(new ConfirmEmailWithPwdDto(new Guid(), "sfgyiyujdfbgst", "NEWPWD", ""));


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.ConfirmPassword);

    }


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_PasswordsDoNotMatch()
    {
        //Arrange
        ConfirmEmailWithPwdCmd cmd = new(new ConfirmEmailWithPwdDto(new Guid(), "sfgyiyujdfbgst", "Password", "ConfirmPassword"));


        //Act
        var result0 = _validator.TestValidate(cmd);
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.ConfirmPassword);

    }


    //------------------------------------//

    [Fact]
    public void Should_NOT_have_error_when_PasswordsDoMatch()
    {
        //Arrange
        ConfirmEmailWithPwdCmd cmd = new(new ConfirmEmailWithPwdDto(new Guid(), "sfgyiyujdfbgst", "Password123P", "Password123P"));


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Dto.Password);

    }


    //------------------------------------//

    [Fact]
    public void Should_NOT_have_DTO_error_when_AllDataSupplied()
    {
        //Arrange
        ConfirmEmailWithPwdCmd cmd = new(new ConfirmEmailWithPwdDto(new Guid(), "sfgyiyujdfbgst", "NEWPWD", "NEWPWD"));


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//


}//Cls
