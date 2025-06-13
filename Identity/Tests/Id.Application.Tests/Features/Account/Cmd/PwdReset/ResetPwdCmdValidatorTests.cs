using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.Cookies.SignOut;
using ID.Application.Features.Account.Cmd.PwdReset;
using ID.Application.Features.Account.Cmd.Refresh;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.PwdReset;
public class ResetPwdCmdValidatorTests
{
    private readonly ResetPwdCmdValidator _validator;

    public ResetPwdCmdValidatorTests()
    {
        _validator = new ResetPwdCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_DTO_is_null()
    {
        //Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        ResetPwdCmd cmd = new(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_ResetToken_Missing()
    {
        //Arrange
        ResetPwdCmd cmd = new(new ResetPwdDto()
        {
            UserId = Guid.NewGuid(),
            Email = "a@b.cd",
            Username = "Username",
            NewPassword = "Password",
            ConfirmPassword = "Password",
            //ResetToken = "ResetToken",
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.ResetToken);

    }


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_ConfirmPassword_Missing()
    {
        //Arrange
        ResetPwdCmd cmd = new(new ResetPwdDto()
        {
            UserId = Guid.NewGuid(),
            Email = "a@b.cd",
            Username = "Username",
            //NewPassword = "Password",
            ConfirmPassword = "Password",
            ResetToken = "ResetToken",
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.NewPassword);

    }


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_NewPassword_Missing()
    {
        //Arrange
        ResetPwdCmd cmd = new(new ResetPwdDto()
        {
            UserId = Guid.NewGuid(),
            Email = "a@b.cd",
            Username = "Username",
            NewPassword = "Password",
            //ConfirmPassword = "Password",
            ResetToken = "ResetToken",
        });


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
        ResetPwdCmd cmd = new(new ResetPwdDto()
        {
            UserId = Guid.NewGuid(),
            Email = "a@b.cd",
            Username = "Username",
            NewPassword = "Password",
            ConfirmPassword = "Password_Different",
            ResetToken = "ResetToken",
        });


        //Act
        var result0 = _validator.TestValidate(cmd);
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.ConfirmPassword);

    }


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_AllIdentifiersAreMissing()
    {
        //Arrange
        ResetPwdCmd cmd = new(new ResetPwdDto() { });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//

    [Fact]
    public void Should_Succeed_when_EmailSupplied()
    {
        //Arrange
        ResetPwdCmd cmd = new(new ResetPwdDto()
        {
            Email = "a@b.cd",
            NewPassword = "Password",
            ConfirmPassword = "Password",
            ResetToken = "ResetToken",
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldSatisfyAllConditions();

    }


    //------------------------------------//

    [Fact]
    public void Should_Succeed_when_UsernameSupplied()
    {
        //Arrange
        ResetPwdCmd cmd = new(new ResetPwdDto()
        {
            Username = "Username",
            NewPassword = "Password",
            ConfirmPassword = "Password",
            ResetToken = "ResetToken",
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldSatisfyAllConditions();

    }


    //------------------------------------//

    [Fact]
    public void Should_Succeed_when_UserIdSupplied()
    {
        //Arrange
        ResetPwdCmd cmd = new(new ResetPwdDto()
        {
            UserId = Guid.NewGuid(),
            NewPassword = "Password",
            ConfirmPassword = "Password",
            ResetToken = "ResetToken",
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldSatisfyAllConditions();

    }


    //------------------------------------//


}//Cls
