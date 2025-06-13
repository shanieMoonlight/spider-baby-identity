using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Tests.Mediatr.Validation.CustomerValidators.Data;
using ID.Application.Mediatr.Behaviours.Validation;

namespace ID.Application.Tests.Mediatr.Validation.CustomerValidators;
public class CustomerValidatorMinimumTests
{


    //------------------------------------//

    [Fact]
    public void CustomerOnlyValidator_Should_Have_Error_When_IsCustomer_Is_False()
    {
        var validator = new TestCustomerOnlyValidator();
        var request = new TestRequest { IsAuthenticated = true, IsCustomer = false };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(1);
        result.Errors.First().CustomState.ShouldBe(ValidationError.Forbidden);

    }

    //------------------------------------//

    [Fact]
    public void CustomerOnlyValidator_Should_Not_Have_Error_When_IsCustomer_Is_True()
    {
        var validator = new TestCustomerOnlyValidator();
        var request = new TestRequest { IsAuthenticated = true, IsCustomer = true };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls

