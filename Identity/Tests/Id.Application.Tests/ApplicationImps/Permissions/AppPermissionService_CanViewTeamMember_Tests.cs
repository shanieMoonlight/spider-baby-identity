using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.AppImps.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.Permissions;

public class AppPermissionService_CanViewTeamMember_Tests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly CanViewTeamMemberPermissions<AppUser> _appPermissionService;

    public AppPermissionService_CanViewTeamMember_Tests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _appPermissionService = new CanViewTeamMemberPermissions<AppUser>(_mockTeamManager.Object);
    }

    //------------------------------------//

    public static TheoryData<TeamType, TeamType, Func<GenResult<AppUser>, bool?>, Func<Team, AppUser, string>> GetPrincipalUserNotInHigherTeamData()
    {
        return new TheoryData<TeamType, TeamType, Func<GenResult<AppUser>, bool?>, Func<Team, AppUser, string>>
        {
            {
                TeamType.Customer,
                TeamType.Maintenance,
                (GenResult<AppUser> result) => result.Forbidden,
                (Team mbrTeam, AppUser mbr) => IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(mbrTeam.TeamType)
            },
            {
                TeamType.Customer,
                TeamType.Super,
                (GenResult<AppUser> result) => result.Forbidden,
                (Team mbrTeam, AppUser mbr) => IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(mbrTeam.TeamType)
            },
            {
                TeamType.Maintenance,
                TeamType.Super,
                (GenResult<AppUser> result) => result.Forbidden,
                (Team mbrTeam, AppUser mbr) => IDMsgs.Error.Teams.UNAUTHORIZED_FOR_TEAM_TYPE(mbrTeam.TeamType)
            },
            {
                TeamType.Maintenance,
                TeamType.Customer,
                (GenResult<AppUser> result) => null,
                (Team mbrTeam, AppUser mbr) => string.Empty
            },   
            {
                TeamType.Super,
                TeamType.Maintenance,
                (GenResult<AppUser> result) => null,
                (Team mbrTeam, AppUser mbr) => string.Empty
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetPrincipalUserNotInHigherTeamData))]
    public async Task CanViewTeamMemberAsync_PrincipalUser_NOT_InHigherTeam_ShouldReturnForbiddenResult(
        TeamType principalTeamType,
        TeamType memberTeamType,
        Func<GenResult<AppUser>, bool?> getFlag,
        Func<Team, AppUser, string> getMsg)
    {
        // Arrange
        var principalUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), teamPosition: 2);
        var member = AppUserDataFactory.Create(teamId: Guid.NewGuid(), teamPosition: 1);

        var memberTeam = TeamDataFactory.Create(id: member.TeamId, teamType: memberTeamType, members: [member]);

        var request = new Mock<IIdUserAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalUser).Returns(principalUser);
        request.Setup(r => r.TeamType).Returns(principalTeamType);

        _mockTeamManager.Setup(m => m.GetByIdWithMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(memberTeam);


        // Act
        var result = await _appPermissionService.CanViewTeamMemberAsync(memberTeam.Id, member.Id, request.Object);


        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        var flag = getFlag(result);
        if (flag.HasValue)
        {
            flag.Value.ShouldBeTrue();
            result.Succeeded.ShouldBeFalse();
        }
        else
        {
            result.Succeeded.ShouldBeTrue();
        }
        var msg = getMsg(memberTeam, member);
        result.Info.ShouldBe(msg);
    }

    //------------------------------------//

}
