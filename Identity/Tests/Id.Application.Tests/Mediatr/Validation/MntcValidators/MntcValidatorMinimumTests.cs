using Shouldly;
using FluentValidation.TestHelper;
using ID.Application.Tests.Mediatr.Validation.MntcValidators.Data;
using ID.Application.Mediatr.Behaviours.Validation;

namespace ID.Application.Tests.Mediatr.Validation.MntcValidators;
public class MntcValidatorMinimumTests
{



    //------------------------------------//

    [Fact]
    public void MntcMinimumValidator_Should_Have_Error_When_IsMntcMinimum_Is_False()
    {
        var validator = new TestMntcMinimumValidator();
        var request = new TestRequest { IsAuthenticated = true, IsMntc = false };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(1);
        result.Errors.First().CustomState.ShouldBe(ValidationError.Forbidden);
    }

    //------------------------------------//

    [Fact]
    public void MntcMinimumValidator_Should_Not_Have_Error_When_IsMntcMinimum_Is_True()
    {
        var validator = new TestMntcMinimumValidator();
        var request = new TestRequest { IsAuthenticated = true, IsMntc = true };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void MntcMinimumValidator_With_Position_Should_Have_Error_When_TeamPosition_Is_Less_Than_Position()
    {
        var validator = new TestMntcMinimumValidator(5);
        var request = new TestRequest { IsAuthenticated = true, IsMntc = true, PrincipalTeamPosition = 3 };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(1);
        result.Errors.First().CustomState.ShouldBe(ValidationError.Forbidden);
    }

    //------------------------------------//

    [Fact]
    public void MntcMinimumValidator_With_Position_Should_Not_Have_Error_When_TeamPosition_Is_Greater_Than_Or_Equal_To_Position()
    {
        var validator = new TestMntcMinimumValidator(5);
        var request = new TestRequest { IsAuthenticated = true, IsMntc = true, PrincipalTeamPosition = 5 };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls
