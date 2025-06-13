using FluentValidation.TestHelper;
using ID.Application.Customers.Dtos.User;
using ID.Application.Customers.Features.Account.Cmd.RegCustomerNoPwd;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.RegCustomerNoPwd;
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

}//Cls
