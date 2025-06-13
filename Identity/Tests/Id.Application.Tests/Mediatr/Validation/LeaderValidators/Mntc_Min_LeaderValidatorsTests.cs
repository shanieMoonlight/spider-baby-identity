using FluentValidation.TestHelper;
using ID.Application.Tests.Mediatr.Validation.LeaderValidators.Data;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;

namespace ID.Application.Tests.Mediatr.Validation.LeaderValidators;
public class Mntc_Min_LeaderValidatorsTests
{
    private readonly TestIsMntc_MIN_LeaderValidator _validator = new();

    //------------------------------------//

    [Fact]
    public void IsMntcMinimumLeaderValidator_Should_Have_Error_When_IsMntcMinimum_Is_False()
    {
        var request = new TestIsMntcLeaderRequest { IsLeader = true, IsMntc = false };  // This will cause MntcMin to be false
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.IsMntcMinimum)
              .WithErrorMessage(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(TeamType.Maintenance));
    }

    //------------------------------------//

    [Fact]
    public void IsMntcMinimumLeaderValidator_Should_Not_Have_Error_When_IsMntcMinimum_Is_True()
    {
        var request = new TestIsMntcLeaderRequest { IsLeader = true, IsMntc = true };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.IsMntcMinimum);
    }

    //------------------------------------//

}//Cls

