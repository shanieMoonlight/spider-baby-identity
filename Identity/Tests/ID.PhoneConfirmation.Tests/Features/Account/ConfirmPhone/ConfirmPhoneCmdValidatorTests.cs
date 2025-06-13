using FluentValidation.TestHelper;
using ID.Application.Mediatr.Validation;
using ID.PhoneConfirmation.Features.Account.ConfirmPhone;
using Shouldly;

namespace ID.PhoneConfirmation.Tests.Features.Account.ConfirmPhone;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
public class ConfirmPhoneCmdValidatorTests
{
    private readonly ConfirmPhoneCmdValidator _validator;

    public ConfirmPhoneCmdValidatorTests()
    {
        _validator = new ConfirmPhoneCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_DTO_is_null()
    {
        //Arrange
        ConfirmPhoneCmd cmd = new(null);


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_AllPropsAreMissing()
    {
        //Arrange
        ConfirmPhoneCmd cmd = new(new ConfirmPhoneDto(null) { });


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.ConfirmationToken);

    }


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_NoTokenSupplied()
    {
        //Arrange
        ConfirmPhoneCmd cmd = new(new ConfirmPhoneDto(""));


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.ConfirmationToken);

    }


    //------------------------------------//

    [Fact]
    public void Implements_IsAuthenticatedValidator()
    {

        // Act & Assert
        _validator.ShouldBeAssignableTo<IsAuthenticatedValidator<ConfirmPhoneCmd>>();
    }

    //------------------------------------//


}//Cls
