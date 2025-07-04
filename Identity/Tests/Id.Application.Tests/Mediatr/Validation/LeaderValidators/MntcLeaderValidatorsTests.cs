using FluentValidation.TestHelper;
using ID.Application.Tests.Mediatr.Validation.LeaderValidators.Data;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;

namespace ID.Application.Tests.Mediatr.Validation.LeaderValidators;
public class MntcLeaderValidatorsTests
{
    private readonly TestIsMntcLeaderValidator _validator = new();

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_IsLeader_Is_False()
    {
        var request = new TestIsMntcLeaderRequest { IsLeader = false, IsMntc = true };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.IsLeader)
              .WithErrorMessage(IDMsgs.Error.Teams.ONlY_LEADER_CAN_UPDATE);
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_IsMntc_Is_False()
    {
        var request = new TestIsMntcLeaderRequest { IsLeader = true, IsMntc = false };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.IsMntc)
              .WithErrorMessage(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(TeamType.maintenance));
    }

    //------------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_IsLeader_And_IsMntc_Are_True()
    {
        var request = new TestIsMntcLeaderRequest { IsLeader = true, IsMntc = true };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.IsLeader);
        result.ShouldNotHaveValidationErrorFor(r => r.IsMntc);
    }

    //------------------------------------//


}//Cls

