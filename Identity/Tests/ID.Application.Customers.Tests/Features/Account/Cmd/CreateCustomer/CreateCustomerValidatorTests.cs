using FluentValidation.TestHelper;
using ID.Application.Customers.Dtos.User;
using ID.Application.Customers.Features.Account.Cmd.RegCustomerNoPwd;
using ID.Application.Mediatr.Validation;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.CreateCustomer;
public class CreateCustomerCmdValidatorTests
{
    private readonly RegisterCustomerCmdValidator _validator;

    public CreateCustomerCmdValidatorTests()
    {
        _validator = new RegisterCustomerCmdValidator();
    }

    //------------------------------------//

    [Fact]
    public void Should_have_error_when_DTO_is_null()
    {
        //Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        RegisterCustomerNoPwdCmd cmd = new(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto);

    }


    //------------------------------------//

    [Fact]
    public void Should_have_error_when_Email_IsMissing()
    {
        //Arrange
        RegisterCustomerNoPwdCmd cmd = new(new RegisterCustomerDto());


        //Act
        var result = _validator.TestValidate(cmd);


        //Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.Email);

    }


    //------------------------------------//

    //[Fact]
    //public void Should_have_error_when_SubscriptionId_IsMissing()
    //{
    //    //Arrange
    //    CreateCustomerCmd cmd = new(new CreateCustomerDto()
    //    {
    //        Email = "a@b.c"
    //    });


    //    //Act
    //    var result = _validator.TestValidate(cmd);


    //    //Assert
    //    result.ShouldHaveValidationErrorFor(cmd => cmd.Dto.SubscriptionId);

    //}


    //------------------------------------//

    [Fact]
    public void Implements_AMntcMinimumValidator()
    {
        // Arrange
        var validator = new RegisterCustomerCmdValidator();

        // Act & Assert
        validator.ShouldBeAssignableTo<AMntcMinimumValidator<RegisterCustomerNoPwdCmd>>();
    }

    //------------------------------------//

}//Cls
