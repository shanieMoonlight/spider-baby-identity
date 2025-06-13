using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.AppImps.Permissions;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.ApplicationImps.Permissions;

public class AppPermissionService_CanDelete_Tests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly CanDeletePermissions<AppUser> _service;

    public AppPermissionService_CanDelete_Tests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _service = new CanDeletePermissions<AppUser>(_mockTeamManager.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task CanDeleteAsync_UserNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var deleteUserId = Guid.NewGuid();
        var request = new Mock<IIdPrincipalInfoRequest>();
        request.Setup(r => r.PrincipalTeamId).Returns(Guid.NewGuid());

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _mockTeamManager.Setup(m => m.GetMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()))
            .ReturnsAsync((AppUser)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act
        var result = await _service.CanDeleteAsync(deleteUserId, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanDeleteAsync_CannotDeleteSelf_ReturnsBadRequestResult()
    {
        // Arrange
        var deleteUserId = Guid.NewGuid();
        var request = new Mock<IIdPrincipalInfoRequest>();
        var user = AppUserDataFactory.Create(Guid.NewGuid(), deleteUserId);

        request.Setup(r => r.PrincipalUserId).Returns(deleteUserId);
        request.Setup(r => r.PrincipalTeamId).Returns(user.TeamId);

        _mockTeamManager.Setup(m => m.GetMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.CanDeleteAsync(deleteUserId, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.BadRequest.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanDeleteAsync_UserFromDifferentTeam_ReturnsForbiddenResult()
    {
        // Arrange
        var deleteUserId = Guid.NewGuid();
        var request = new Mock<IIdPrincipalInfoRequest>();
        var user = AppUserDataFactory.Create(Guid.NewGuid(), deleteUserId);

        request.Setup(r => r.PrincipalUserId).Returns(Guid.NewGuid());
        request.Setup(r => r.PrincipalTeamId).Returns(Guid.NewGuid());

        _mockTeamManager.Setup(m => m.GetMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.CanDeleteAsync(deleteUserId, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Forbidden.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanDeleteAsync_CannotDeleteHigherPosition_ReturnsForbiddenResult()
    {
        // Arrange
        var deleteUserId = Guid.NewGuid();
        var request = new Mock<IIdPrincipalInfoRequest>();
        var user = AppUserDataFactory.Create(Guid.NewGuid(), deleteUserId, teamPosition: 2);

        request.Setup(r => r.PrincipalUserId).Returns(Guid.NewGuid());
        request.Setup(r => r.PrincipalTeamId).Returns(user.TeamId);
        request.Setup(r => r.PrincipalTeamPosition).Returns(1);

        _mockTeamManager.Setup(m => m.GetMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.CanDeleteAsync(deleteUserId, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Forbidden.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task CanDeleteAsync_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var deleteUserId = Guid.NewGuid();
        var request = new Mock<IIdPrincipalInfoRequest>();
        var user = AppUserDataFactory.Create(Guid.NewGuid(), deleteUserId, teamPosition: 1);

        request.Setup(r => r.PrincipalUserId).Returns(Guid.NewGuid());
        request.Setup(r => r.PrincipalTeamId).Returns(user.TeamId);
        request.Setup(r => r.PrincipalTeamPosition).Returns(2);

        _mockTeamManager.Setup(m => m.GetMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.CanDeleteAsync(deleteUserId, request.Object);

        // Assert
        result.ShouldBeOfType<GenResult<AppUser>>();
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls