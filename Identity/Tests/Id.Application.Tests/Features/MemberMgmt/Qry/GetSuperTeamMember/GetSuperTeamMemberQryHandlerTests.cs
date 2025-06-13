using ID.Application.Features.MemberMgmt.Qry.GetSuperTeamMember;
using ID.Tests.Data.Factories;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Qry.GetSuperTeamMember;

public class GetSuperTeamMemberQryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenMemberExists()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var member = AppUserDataFactory.Create(teamId:teamId);
        var team = TeamDataFactory.Create(id:teamId, members: [member]);

        var request = new GetSuperTeamMemberQry(member.Id)
        {
            PrincipalTeam = team
        };

        var handler = new GetSuperTeamMemberQryHandler();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(member.Id);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFoundResult_WhenMemberDoesNotExist()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var request = new GetSuperTeamMemberQry(Guid.NewGuid())
        {
            PrincipalTeam = team
        };

        var handler = new GetSuperTeamMemberQryHandler();

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

}