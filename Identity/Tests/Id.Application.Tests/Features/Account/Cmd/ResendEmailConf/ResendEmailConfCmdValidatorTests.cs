using FluentValidation.TestHelper;
using ID.Application.Features.Account.Cmd.ResendEmailConfirmation;

namespace ID.Application.Tests.Features.Account.Cmd.ResendEmailConf;
public class ResendEmailConfCmdValidatorTests
{
    private readonly ResendEmailConfirmationCmdValidator _validator;

    public ResendEmailConfCmdValidatorTests()
    {
        _validator = new ResendEmailConfirmationCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_DTO_is_null()
    {
        //Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        ResendEmailConfirmationCmd cmd = new(null);
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
        ResendEmailConfirmationCmd cmd = new(new ResendEmailConfirmationDto());


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//

    [Fact]
    public void Should_NOT_have_error_when_EmailSupplied()
    {
        //Arrange
        ResendEmailConfirmationCmd cmd = new(new ResendEmailConfirmationDto()
        {
            Email = "a@b.c",
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
        ResendEmailConfirmationCmd cmd = new(new ResendEmailConfirmationDto()
        {
            Username = "Username",
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
        ResendEmailConfirmationCmd cmd = new(new ResendEmailConfirmationDto()
        {
            UserId = new Guid(),
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//


}//Cls
