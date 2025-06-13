using FluentValidation.TestHelper;
using ID.Application.Mediatr.Behaviours.Validation;
using ID.Application.Tests.Mediatr.Validation.LeaderValidators.Data;
using Shouldly;

namespace ID.Application.Tests.Mediatr.Validation.LeaderValidators;
public class LeaderValidatorsTests
{
    private readonly TestIsLeaderValidator validator = new();

    //------------------------------------//

    [Fact]
    public void IsLeaderValidator_Should_Have_Error_When_IsLeader_Is_False()
    {
        var request = new TestIsLeaderRequest { IsAuthenticated = true, IsLeader = false };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(1);
        result.Errors.First().CustomState.ShouldBe(ValidationError.Forbidden);
        result.ShouldHaveValidationErrorFor(d => d.IsLeader);

    }

    //------------------------------------//

    [Fact]
    public void IsLeaderValidator_Should_NOT_Have_Error_When_IsLeader_Is_True()
    {
        var request = new TestIsLeaderRequest { IsAuthenticated = true, IsLeader = true };

        var result = validator.TestValidate(request);


        //act
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveValidationErrorFor(d => d.IsLeader);

    }

    //------------------------------------//


}//Cls

