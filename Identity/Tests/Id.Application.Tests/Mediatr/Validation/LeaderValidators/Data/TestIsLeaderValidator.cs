using ID.Application.Mediatr.Validation;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Tests.Mediatr.Validation.LeaderValidators.Data;

//==============================================================//


internal record TestIsLeaderRequest : AIdUserAndTeamAwareCommand<AppUser> { }
internal class TestIsLeaderValidator : IsLeaderValidator<TestIsLeaderRequest> { }


//==============================================================//


internal record TestIsMntcLeaderRequest : AIdUserAndTeamAwareCommand<AppUser> { }
internal class TestIsMntcLeaderValidator : IsMntcLeaderValidator<TestIsMntcLeaderRequest> { }

internal class TestIsMntc_MIN_LeaderValidator : IsMntcMinimumLeaderValidator<TestIsMntcLeaderRequest> { }



//==============================================================//


internal record TestIsSuperLeaderRequest : AIdUserAndTeamAwareCommand<AppUser> { }
internal class TestIsSuperLeaderValidator : IsSuperLeaderValidator<TestIsSuperLeaderRequest> { }

internal class TestIsSuper_MIN_LeaderValidator : IsSuperMinimumLeaderValidator<TestIsSuperLeaderRequest> { }


//==============================================================//