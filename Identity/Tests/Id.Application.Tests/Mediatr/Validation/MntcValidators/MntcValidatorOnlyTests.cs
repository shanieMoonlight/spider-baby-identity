using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Tests.Mediatr.Validation.MntcValidators.Data;
using ID.Application.Mediatr.Behaviours.Validation;

namespace ID.Application.Tests.Mediatr.Validation.MntcValidators;
public class MntcValidatorOnlyTests
{

    //------------------------------------//

    [Fact]
    public void MntcOnlyValidator_Should_Have_Error_When_IsMntc_Is_False()
    {
        var validator = new TestMntcOnlyValidator();
        var request = new TestRequest { IsAuthenticated = true, IsMntc = false };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(1);
        result.Errors.First().CustomState.ShouldBe(ValidationError.Forbidden);

    }

    //------------------------------------//

    [Fact]
    public void MntcOnlyValidator_Should_Not_Have_Error_When_IsMntc_Is_True()
    {
        var validator = new TestMntcOnlyValidator();
        var request = new TestRequest { IsAuthenticated = true, IsMntc = true };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls

