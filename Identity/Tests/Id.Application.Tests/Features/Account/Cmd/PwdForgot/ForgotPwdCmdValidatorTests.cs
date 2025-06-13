using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.PwdForgot;
using Shouldly;

namespace ID.Application.Tests.Features.Account.Cmd.PwdForgot;
public class ForgotPwdCmdValidatorTests
{
    private readonly ForgotPwdCmdValidator _validator;

    public ForgotPwdCmdValidatorTests()
    {
        _validator = new ForgotPwdCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_DTO_is_null()
    {
        //Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        ForgotPwdCmd cmd = new(null);
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
        ForgotPwdCmd cmd = new(new ForgotPwdDto() { });


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
        ForgotPwdCmd cmd = new(new ForgotPwdDto()
        {
            Email = "a@b.cd"
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
        ForgotPwdCmd cmd = new(new ForgotPwdDto()
        {
            Username = "Username"
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
        ForgotPwdCmd cmd = new(new ForgotPwdDto()
        {
            UserId = Guid.NewGuid()
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldSatisfyAllConditions();

    }


    //------------------------------------//


}//Cls
