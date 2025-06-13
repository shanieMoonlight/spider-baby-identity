using FluentValidation.TestHelper;
using ID.Application.Tests.Mediatr.Validation.LeaderValidators.Data;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;

namespace ID.Application.Tests.Mediatr.Validation.LeaderValidators;
public class Super_Min_LeaderValidatorsTests
{
    private readonly TestIsSuper_MIN_LeaderValidator _validator = new();

    //------------------------------------//

    [Fact]
    public void IsSuperMinimumLeaderValidator_Should_Have_Error_When_IsSuperMinimum_Is_False()
    {
        var request = new TestIsSuperLeaderRequest { IsLeader = true, IsSuper = false };  // This will cause SuperMin to be false
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.IsSuperMinimum)
              .WithErrorMessage(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(TeamType.Super));
    }

    //------------------------------------//

    [Fact]
    public void IsSuperMinimumLeaderValidator_Should_Not_Have_Error_When_IsSuperMinimum_Is_True()
    {
        var request = new TestIsSuperLeaderRequest { IsLeader = true, IsSuper = true };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.IsSuperMinimum);
    }

    //------------------------------------//

}//Cls

