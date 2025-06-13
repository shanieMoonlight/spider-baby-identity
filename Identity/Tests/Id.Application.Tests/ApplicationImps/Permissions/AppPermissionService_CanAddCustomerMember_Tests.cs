using ID.Tests.Data.Factories;
using Moq;
using Shouldly;
using ID.Application.AppImps.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Tests.ApplicationImps.Permissions;

public class AppPermissionService_CanAddCustomerMember_Tests
{
    private readonly CanAddPermissions _appPermissionService;

    public AppPermissionService_CanAddCustomerMember_Tests()
    {
        _appPermissionService = new CanAddPermissions();
    }

    //------------------------------------//

    [Fact]
    public async Task CanAddCustomerTeamMember_NotCustomer_ReturnsForbidden()
    {
        // Arrange
        var newMemberPosition = 2;
        var principalUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), teamPosition: newMemberPosition + 2); //SHould pass position condition
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeamPosition).Returns(principalUser.TeamPosition);
        request.Setup(r => r.IsCustomer).Returns(false);

        // Act
        var result = await _appPermissionService.CanAddCustomerTeamMember(newMemberPosition, request.Object);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Forbidden.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanAddCustomerTeamMember_PrincipalPositionHigherOrEqual_ReturnsSuccess()
    {
        // Arrange
        var principalUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), teamPosition: 2);
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeamPosition).Returns(principalUser.TeamPosition);
        request.Setup(r => r.IsCustomer).Returns(true);

        // Act
        var result = await _appPermissionService.CanAddCustomerTeamMember(1, request.Object);

        // Assert
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanAddCustomerTeamMember_PrincipalPositionLower_ReturnsForbidden()
    {
        // Arrange
        var newMemberPosition = 2;
        var principalUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), teamPosition: 1);
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeamPosition).Returns(principalUser.TeamPosition);
        request.Setup(r => r.IsCustomer).Returns(true);

        // Act
        var result = await _appPermissionService.CanAddCustomerTeamMember(newMemberPosition, request.Object);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Forbidden.ShouldBeTrue();
        result.Info.ShouldBe(IDMsgs.Error.Authorization.UNAUTHORIZED_FOR_POSITION(newMemberPosition));
    }

    //------------------------------------//

    [Fact]
    public async Task CanAddTeamMember_PrincipalPositionLower_But_LEader_ReturnsSuccess()
    {
        // Arrange
        var principalUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), teamPosition: 2);
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeamPosition).Returns(principalUser.TeamPosition);
        request.Setup(r => r.IsCustomer).Returns(true);
        request.Setup(r => r.IsLeader).Returns(true);

        // Act
        var result = await _appPermissionService.CanAddCustomerTeamMember(principalUser.TeamPosition + 5, request.Object);

        // Assert
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanAddTeamMember_NonCustomer_But_Leader_ReturnsForbidden()
    {
        // Arrange
        var principalUser = AppUserDataFactory.Create(teamId: Guid.NewGuid(), teamPosition: 2);
        var request = new Mock<IIdUserAndTeamAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalTeamPosition).Returns(principalUser.TeamPosition);
        request.Setup(r => r.IsCustomer).Returns(false);
        request.Setup(r => r.IsLeader).Returns(true);

        // Act
        var result = await _appPermissionService.CanAddCustomerTeamMember(principalUser.TeamPosition + 5, request.Object);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Forbidden.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls
