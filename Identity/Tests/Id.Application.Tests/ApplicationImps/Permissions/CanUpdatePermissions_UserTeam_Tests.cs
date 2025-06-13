using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.AppImps.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.ApplicationImps.Permissions;

public class CanUpdatePermissions_UserTeam_Tests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly CanUpdatePermissions<AppUser> _canUpdatePermissions;

    public CanUpdatePermissions_UserTeam_Tests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _canUpdatePermissions = new CanUpdatePermissions<AppUser>(_mockTeamManager.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task CanUpdateAsync_UserNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var updateUserId = Guid.NewGuid();
        var request = new Mock<IIdUserAwareRequest<AppUser>>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        request.Setup(r => r.PrincipalUser).Returns((AppUser)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act
        var result = await _canUpdatePermissions.CanUpdateAsync(updateUserId, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanUpdateAsync_UserCannotUpdateOthers_ReturnsBadRequestResult()
    {
        // Arrange
        var updateUserId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId);
        var request = new Mock<IIdUserAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalUser).Returns(principalUser);
        request.Setup(r => r.PrincipalUserId).Returns(Guid.NewGuid());

        // Act
        var result = await _canUpdatePermissions.CanUpdateAsync(updateUserId, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.BadRequest.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanUpdateAsync_UserCanUpdateSelf_ReturnsSuccessResult()
    {
        // Arrange
        var updateUserId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var principalUser = AppUserDataFactory.Create(teamId: teamId, id: updateUserId);
        var request = new Mock<IIdUserAwareRequest<AppUser>>();
        request.Setup(r => r.PrincipalUser).Returns(principalUser);
        request.Setup(r => r.PrincipalUserId).Returns(updateUserId);

        // Act
        var result = await _canUpdatePermissions.CanUpdateAsync(updateUserId, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(principalUser);
    }

    //------------------------------------//

}
