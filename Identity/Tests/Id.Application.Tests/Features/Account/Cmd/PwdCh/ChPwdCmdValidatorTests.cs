using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.PwdChange;
using ID.Application.Features.Account.Cmd.RefreshTokenRevoke;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.PwdCh;
public class ChPwdCmdValidatorTests
{
    private readonly ChPwdCmdValidator _validator;

    public ChPwdCmdValidatorTests()
    {
        _validator = new ChPwdCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_DTO_is_null()
    {
        //Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        ChPwdCmd cmd = new(null);
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
        ChPwdCmd cmd = new(new ChPwdDto() { });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//
    [Fact]
    public void Should_have_error_when_NoPasswordSupplied()
    {
        //Arrange
        ChPwdCmd cmd = new(new ChPwdDto()
        {
            Email = "a@b.c",
            NewPassword = "NewPassword",
            ConfirmPassword = "ConfirmPassword"
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.Password);

    }


    //------------------------------------//
    [Fact]
    public void Should_have_error_when_NoNewPasswordSupplied()
    {
        //Arrange
        ChPwdCmd cmd = new(new ChPwdDto()
        {
            Email = "a@b.c",
            Password = "Password",
            ConfirmPassword = "ConfirmPassword"
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.NewPassword);

    }


    //------------------------------------//
    [Fact]
    public void Should_have_error_when_NoConfirmPasswordSupplied()
    {
        //Arrange
        ChPwdCmd cmd = new(new ChPwdDto()
        {
            Email = "a@b.c",
            Password = "Password",
            NewPassword = "NewPassword",
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
        ChPwdCmd cmd = new(new ChPwdDto()
        {
            Email = "a@b.c",
            Password = "Password",
            NewPassword = "NewPassword",
            ConfirmPassword = "ConfirmPassword"
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.NewPassword);

    }


    //------------------------------------//
    [Fact]
    public void Should_NOT_have_error_when_PasswordsDoMatch()
    {
        //Arrange
        ChPwdCmd cmd = new(new ChPwdDto()
        {
            Email = "a@b.c",
            Password = "Password",
            NewPassword = "NewPassword",
            ConfirmPassword = "NewPassword"
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Dto.NewPassword);

    }


    //------------------------------------//

    [Fact]
    public void Should_NOT_have_DTO_error_when_EmailSupplied()
    {
        //Arrange
        ChPwdCmd cmd = new(new ChPwdDto()
        {
            Email = "a@b.c",
            Password = "password!"
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//

    [Fact]
    public void Should_NOT_have_DTO_error_when_UsernameSupplied()
    {
        //Arrange
        ChPwdCmd cmd = new(new ChPwdDto()
        {
            Username = "Username",
            Password = "password!"
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//

    [Fact]
    public void Should_NOT_have_DTO_error_when_UserIdSupplied()
    {
        //Arrange
        ChPwdCmd cmd = new(new ChPwdDto()
        {
            UserId = new Guid(),
            Password = "password!"
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Dto);

    }

    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {
        // Arrange
        ChPwdCmdValidator validator = new();

        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<ChPwdCmd>>();
    }

    //------------------------------------//

}//Cls
