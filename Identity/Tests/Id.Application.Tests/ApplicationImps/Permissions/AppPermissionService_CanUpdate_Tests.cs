using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.AppImps.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.ApplicationImps.Permissions;

public class AppPermissionService_CanUpdate_Tests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamManagerMock;
    private readonly CanUpdatePermissions<AppUser> _appPermissionService;

    public AppPermissionService_CanUpdate_Tests()
    {
        _teamManagerMock = new Mock<IIdentityTeamManager<AppUser>>();
        _appPermissionService = new CanUpdatePermissions<AppUser>(_teamManagerMock.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task CanUpdateAsync_UserDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        var updateUserId = Guid.NewGuid();
        var request = new Mock<IIdPrincipalInfoRequest>();
        request.SetupGet(r => r.PrincipalTeamId).Returns(Guid.NewGuid());

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _teamManagerMock.Setup(tm => tm.GetMemberAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>()))
            .ReturnsAsync((AppUser)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act
        var result = await _appPermissionService.CanUpdateAsync(updateUserId, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanUpdateAsync_CannotUpdateOtherUser_ReturnsBadRequestResult()
    {
        // Arrange
        var updateUserId = Guid.NewGuid();
        var request = new Mock<IIdPrincipalInfoRequest>();
        request.SetupGet(r => r.PrincipalUserId).Returns(Guid.NewGuid());
        request.SetupGet(r => r.PrincipalTeamId).Returns(Guid.NewGuid());

        var updateUser = AppUserDataFactory.Create(request.Object.PrincipalTeamId!.Value, updateUserId);

        _teamManagerMock.Setup(tm => tm.GetMemberAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>()))
            .ReturnsAsync(updateUser);

        // Act
        var result = await _appPermissionService.CanUpdateAsync(updateUserId, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.BadRequest.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanUpdateAsync_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var updateUserId = Guid.NewGuid();
        var request = new Mock<IIdPrincipalInfoRequest>();
        request.SetupGet(r => r.PrincipalUserId).Returns(updateUserId);
        request.SetupGet(r => r.PrincipalTeamId).Returns(Guid.NewGuid());

        var updateUser = AppUserDataFactory.Create(request.Object.PrincipalTeamId!.Value, updateUserId);

        _teamManagerMock.Setup(tm => tm.GetMemberAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>()))
            .ReturnsAsync(updateUser);

        // Act
        var result = await _appPermissionService.CanUpdateAsync(updateUserId, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls