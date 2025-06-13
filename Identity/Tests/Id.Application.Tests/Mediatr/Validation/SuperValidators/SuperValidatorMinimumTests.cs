using FluentValidation.TestHelper;
using ID.Application.Mediatr.Behaviours.Validation;
using ID.Application.Tests.Mediatr.Validation.SuperValidators.Data;
using Shouldly;

namespace ID.Application.Tests.Mediatr.Validation.SuperValidators;
public class SuperValidatorMinimumTests
{


    //------------------------------------//

    [Fact]
    public void SuperMinimumValidator_Should_Have_Error_When_IsSuperMinimum_Is_False()
    {
        var validator = new TestSuperMinimumValidator();
        var request = new TestRequest { IsAuthenticated = true, IsSuper = false };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(1);
        result.Errors.First().CustomState.ShouldBe(ValidationError.Forbidden);
    }

    //------------------------------------//

    [Fact]
    public void SuperMinimumValidator_Should_Not_Have_Error_When_IsSuperMinimum_Is_True()
    {
        var validator = new TestSuperMinimumValidator();
        var request = new TestRequest { IsAuthenticated = true, IsSuper = true };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void SuperMinimumValidator_With_Position_Should_Have_Error_When_TeamPosition_Is_Less_Than_Position()
    {
        var validator = new TestSuperMinimumValidator(5);
        var request = new TestRequest { IsAuthenticated = true, IsSuper = true, PrincipalTeamPosition = 3 };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(1);
        result.Errors.First().CustomState.ShouldBe(ValidationError.Forbidden);
    }

    //------------------------------------//

    [Fact]
    public void SuperMinimumValidator_With_Position_Should_Not_Have_Error_When_TeamPosition_Is_Greater_Than_Or_Equal_To_Position()
    {
        var validator = new TestSuperMinimumValidator(5);
        var request = new TestRequest { IsAuthenticated = true, IsSuper = true, PrincipalTeamPosition = 5 };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls
