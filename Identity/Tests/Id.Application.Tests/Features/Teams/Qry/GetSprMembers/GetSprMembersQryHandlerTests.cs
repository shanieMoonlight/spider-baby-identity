using ID.Tests.Data.Factories;
using Moq;
using Shouldly;
using ID.Application.Features.Teams.Qry.GetSprMembers;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.Features.Teams.Qry.GetSprMembers;

public class GetSprMembersQryHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly GetSprMembersQryHandler _handler;

    public GetSprMembersQryHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new GetSprMembersQryHandler(_mockTeamManager.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShoulCallGetSuperTeamWithMembersAsync_WithPrincipalTeamPosition_IfIsMntc()
    {
        // Arrange
        var teamPosition = 5;
        var team = TeamDataFactory.Create();
        var request = new GetSprMembersQry { IsSuper = false, PrincipalTeamPosition = teamPosition };
        _mockTeamManager.Setup(mgr => mgr.GetSuperTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(team);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        _mockTeamManager.Verify(mgr => mgr.GetSuperTeamWithMembersAsync(teamPosition));
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldCallGetCustomerTeamsByNameAsync_WithRequestName()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var mbr1 = AppUserDataFactory.Create(teamId: teamId);
        var mbr2 = AppUserDataFactory.Create(teamId: teamId);
        var mbr3 = AppUserDataFactory.Create(teamId: teamId);
        var mbr4 = AppUserDataFactory.Create(teamId: teamId);
        var mbr5 = AppUserDataFactory.Create(teamId: teamId);
        HashSet<AppUser> members = [mbr1, mbr2, mbr3, mbr4, mbr5];
        var mntcTeam = TeamDataFactory.Create(id: teamId, teamType: TeamType.super, members: members);

        _mockTeamManager.Setup(mgr => mgr.GetSuperTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(mntcTeam);

        var request = new GetSprMembersQry();

        // Act
        var result = await _handler.Handle(request, new CancellationToken());

        // Assert
        result.Value?.Count().ShouldBe(members.Count);
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls
