using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Tests.Mediatr.Validation.SuperValidators.Data;
using ID.Application.Mediatr.Behaviours.Validation;

namespace ID.Application.Tests.Mediatr.Validation.SuperValidators;
public class SuperValidatorOnlyTests
{


    //------------------------------------//

    [Fact]
    public void SuperOnlyValidator_Should_Have_Error_When_IsSuper_Is_False()
    {
        var validator = new TestSuperOnlyValidator();
        var request = new TestRequest { IsAuthenticated = true, IsSuper = false };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(1);
        result.Errors.First().CustomState.ShouldBe(ValidationError.Forbidden);

    }

    //------------------------------------//

    [Fact]
    public void SuperOnlyValidator_Should_Not_Have_Error_When_IsSuper_Is_True()
    {
        var validator = new TestSuperOnlyValidator();
        var request = new TestRequest { IsAuthenticated = true, IsSuper = true };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls

