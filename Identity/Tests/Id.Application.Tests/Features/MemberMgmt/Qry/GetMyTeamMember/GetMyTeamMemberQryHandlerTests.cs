using ID.Application.Features.MemberMgmt.Qry.GetMyTeamMember;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Qry.GetTeamMember;

public class GetMyTeamMemberQryHandlerTests
{

    //------------------------------------//

    [Fact]
    public async Task Handle_MemberExists_ReturnsMember()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var teamMember =    AppUserDataFactory.Create(id: memberId, teamId: teamId);
        var principalTeam = TeamDataFactory.Create(teamId, members: [teamMember]);

        var request = new GetMyTeamMemberQry(memberId){ PrincipalTeam = principalTeam };
        var handler = new GetMyTeamMemberQryHandler();


        // Act
        var result = await handler.Handle(request, CancellationToken.None);


        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(memberId);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_MemberDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var NON_memberId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var teamMember = AppUserDataFactory.Create(id: memberId, teamId: teamId);
        var principalTeam = TeamDataFactory.Create(teamId, members: [teamMember]);

        var request = new GetMyTeamMemberQry(NON_memberId) { PrincipalTeam = principalTeam };
        var handler = new GetMyTeamMemberQryHandler();


        // Act
        var result = await handler.Handle(request, CancellationToken.None);


        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls