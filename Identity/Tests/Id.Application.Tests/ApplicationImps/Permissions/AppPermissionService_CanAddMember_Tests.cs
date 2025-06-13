using ID.Tests.Data.Factories;
using Moq;
using Shouldly;
using ID.Application.AppImps.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Tests.ApplicationImps.Permissions;

public class AppPermissionService_CanAddMember_Tests
{
    private readonly CanAddPermissions _appPermissionService;

    public AppPermissionService_CanAddMember_Tests()
    {
        _appPermissionService = new CanAddPermissions();
    }

    //------------------------------------//

    [Fact]
    public async Task CanAddTeamMember_PrincipalPositionLower_But_LEader_ReturnsSuccess()
    {
        // Arrange
        var principalUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), teamPosition: 2);
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeamPosition).Returns(principalUser.TeamPosition);
        request.Setup(r => r.IsLeader).Returns(true);

        // Act
        var result = await _appPermissionService.CanAddTeamMember(principalUser.TeamPosition + 5, request.Object);

        // Assert
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanAddTeamMember_PrincipalPositionHigherOrEqual_ReturnsSuccess()
    {
        // Arrange
        var principalUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), teamPosition: 2);
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeamPosition).Returns(principalUser.TeamPosition);

        // Act
        var result = await _appPermissionService.CanAddTeamMember(1, request.Object);

        // Assert
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanAddTeamMember_PrincipalPositionLower_ReturnsForbidden()
    {
        // Arrange
        var newMemberPosition = 2;
        var principalUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), teamPosition: 1);
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeamPosition).Returns(principalUser.TeamPosition);

        // Act
        var result = await _appPermissionService.CanAddTeamMember(newMemberPosition, request.Object);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Forbidden.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.UNAUTHORIZED_FOR_POSITION(newMemberPosition));
    }

    //------------------------------------//

}//Cls
