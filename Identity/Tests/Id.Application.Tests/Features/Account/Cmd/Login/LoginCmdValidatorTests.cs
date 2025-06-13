using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.Cookies.SignIn;
using ID.Application.Features.Account.Cmd.Login;

namespace ID.Application.Tests.Features.Account.Cmd.Login;
public class LoginCmdValidatorTests
{
    private readonly LoginCmdValidator _validator;

    public LoginCmdValidatorTests()
    {
        _validator = new LoginCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_DTO_is_null()
    {
        //Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        LoginCmd cmd = new(null);
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
        LoginCmd cmd = new(new LoginDto());


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
        LoginCmd cmd = new(new LoginDto()
        {
            Email = "a@b.c",
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.Password);

    }


    //------------------------------------//

    [Fact]
    public void Should_NOT_have_error_when_EmailSupplied()
    {
        //Arrange
        LoginCmd cmd = new(new LoginDto()
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
    public void Should_NOT_have_error_when_UsernameSupplied()
    {
        //Arrange
        LoginCmd cmd = new(new LoginDto()
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
    public void Should_NOT_have_error_when_UserIdSupplied()
    {
        //Arrange
        LoginCmd cmd = new(new LoginDto()
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


}//Cls
