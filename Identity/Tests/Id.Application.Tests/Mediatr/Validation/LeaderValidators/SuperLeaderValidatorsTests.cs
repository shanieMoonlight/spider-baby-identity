using FluentValidation.TestHelper;
using ID.Application.Tests.Mediatr.Validation.LeaderValidators.Data;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;

namespace ID.Application.Tests.Mediatr.Validation.LeaderValidators;
public class SuperLeaderValidatorsTests
{
    private readonly TestIsSuperLeaderValidator _validator = new();

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_IsLeader_Is_False()
    {
        var request = new TestIsSuperLeaderRequest { IsLeader = false, IsSuper = true };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.IsLeader)
              .WithErrorMessage(IDMsgs.Error.Teams.ONlY_LEADER_CAN_UPDATE);
    }

    //------------------------------------//

    [Fact]
    public void Should_Have_Error_When_IsSuper_Is_False()
    {
        var request = new TestIsSuperLeaderRequest { IsLeader = true, IsSuper = false };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.IsSuper)
              .WithErrorMessage(IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(TeamType.Super));
    }

    //------------------------------------//

    [Fact]
    public void Should_Not_Have_Error_When_IsLeader_And_IsSuper_Are_True()
    {
        var request = new TestIsSuperLeaderRequest { IsLeader = true, IsSuper = true };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(r => r.IsLeader);
        result.ShouldNotHaveValidationErrorFor(r => r.IsSuper);
    }

    //------------------------------------//


}//Cls

