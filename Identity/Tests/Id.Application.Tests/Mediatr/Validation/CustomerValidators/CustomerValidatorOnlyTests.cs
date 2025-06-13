using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Tests.Mediatr.Validation.CustomerValidators.Data;
using ID.Application.Mediatr.Behaviours.Validation;

namespace ID.Application.Tests.Mediatr.Validation.CustomerValidators;
public class CustomerValidatorOnlyTests
{


    //------------------------------------//

    [Fact]
    public void CustomerMinimumValidator_Should_Have_Error_When_IsCustomerMinimum_Is_False()
    {
        var validator = new TestCustomerMinimumValidator();
        var request = new TestRequest { IsAuthenticated = true, IsCustomerMinimum = false };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(1);
        result.Errors.First().CustomState.ShouldBe(ValidationError.Forbidden);
    }

    //------------------------------------//

    [Fact]
    public void CustomerMinimumValidator_Should_Not_Have_Error_When_IsCustomerMinimum_Is_True()
    {
        var validator = new TestCustomerMinimumValidator();
        var request = new TestRequest { IsAuthenticated = true, IsCustomerMinimum = true };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void CustomerMinimumValidator_With_Position_Should_Have_Error_When_TeamPosition_Is_Less_Than_Position()
    {
        var validator = new TestCustomerMinimumValidator(5);
        var request = new TestRequest { IsAuthenticated = true, IsCustomerMinimum = true, PrincipalTeamPosition = 3 };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(1);
        result.Errors.First().CustomState.ShouldBe(ValidationError.Forbidden);
    }

    //------------------------------------//

    [Fact]
    public void CustomerMinimumValidator_With_Position_Should_Not_Have_Error_When_TeamPosition_Is_Greater_Than_Or_Equal_To_Position()
    {
        var validator = new TestCustomerMinimumValidator(5);
        var request = new TestRequest { IsAuthenticated = true, IsCustomerMinimum = true, PrincipalTeamPosition = 5 };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls
