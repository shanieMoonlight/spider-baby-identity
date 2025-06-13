using FluentValidation.TestHelper;
using ID.Application.Mediatr.Validation;
using ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmation;
using Shouldly;

namespace ID.PhoneConfirmation.Tests.Features.Account.ResendPhoneConfirmation;
public class ResendPhoneConfirmationValidatorTests
{
    private readonly ResendPhoneConfirmationCmdValidator _validator;

    public ResendPhoneConfirmationValidatorTests()
    {
        _validator = new ResendPhoneConfirmationCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_DTO_is_null()
    {
        //Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        ResendPhoneConfirmationCmd cmd = new(null);
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
        ResendPhoneConfirmationCmd cmd = new(new ResendPhoneConfirmationDto());


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
        ResendPhoneConfirmationCmd cmd = new(new ResendPhoneConfirmationDto()
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
        ResendPhoneConfirmationCmd cmd = new(new ResendPhoneConfirmationDto()
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
        ResendPhoneConfirmationCmd cmd = new(new ResendPhoneConfirmationDto()
        {
            UserId = new Guid(),
        });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//


    [Fact]
    public void Implements_IsAuthenticatedValidatorValidator()
    {
        // Arrange
        ResendPhoneConfirmationCmdValidator validator = new();


        // Act & Assert
        validator.ShouldBeAssignableTo<IsAuthenticatedValidator<ResendPhoneConfirmationCmd>>();
    }

    //------------------------------------//


}//Cls
