using ID.Application.AppAbs.Permissions;
using ID.Application.Features.MemberMgmt.Cmd.DeleteSuperMember;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Cmd.DeleteSuperMember;

public class DeleteSprMemberCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamMgrMock;
    private readonly Mock<IAppPermissionService<AppUser>> _appPermissionsMock;
    private readonly Mock<ICanDeletePermissions<AppUser>> _mockCanDeletePermissionService;
    private readonly DeleteSprMemberCmdHandler _handler;

    public DeleteSprMemberCmdHandlerTests()
    {
        _teamMgrMock = new Mock<IIdentityTeamManager<AppUser>>();
        _appPermissionsMock = new Mock<IAppPermissionService<AppUser>>();
        _mockCanDeletePermissionService = new Mock<ICanDeletePermissions<AppUser>>();
        _appPermissionsMock.Setup(x => x.DeletePermissions)
            .Returns(_mockCanDeletePermissionService.Object);
        _handler = new DeleteSprMemberCmdHandler(_teamMgrMock.Object, _appPermissionsMock.Object);
    }


    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_When_CanDeleteFails()
    {
        // Arrange
        var request = new DeleteSprMemberCmd(Guid.NewGuid());
        var canDeleteUserResult = GenResult<AppUser>.Success(AppUserDataFactory.Create(Guid.NewGuid()));
        _mockCanDeletePermissionService.Setup(x => x.CanDeleteAsync(It.IsAny<Guid>(), It.IsAny<DeleteSprMemberCmd>()))
            .ReturnsAsync(GenResult<AppUser>.ForbiddenResult());
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _teamMgrMock.Setup(x => x.GetSuperTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync((Team)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Forbidden.ShouldBeTrue();
        result.Succeeded.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnNotFoundResult_WhenSuperTeamIsNull()
    {
        // Arrange
        var request = new DeleteSprMemberCmd(Guid.NewGuid());
        var canDeleteUserResult = GenResult<AppUser>.Success(AppUserDataFactory.Create(Guid.NewGuid()));
        _mockCanDeletePermissionService.Setup(x => x.CanDeleteAsync(It.IsAny<Guid>(), It.IsAny<DeleteSprMemberCmd>()))
            .ReturnsAsync(canDeleteUserResult);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _teamMgrMock.Setup(x => x.GetSuperTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync((Team)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.NotFound.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenMemberIsDeleted()
    {
        // Arrange
        var request = new DeleteSprMemberCmd(Guid.NewGuid());
        var appUser = AppUserDataFactory.Create(Guid.NewGuid());
        var canDeleteUserResult = GenResult<AppUser>.Success(appUser);
        var team = TeamDataFactory.Create();
        _mockCanDeletePermissionService.Setup(x => x.CanDeleteAsync(It.IsAny<Guid>(), It.IsAny<DeleteSprMemberCmd>()))
            .ReturnsAsync(canDeleteUserResult);
        _teamMgrMock.Setup(x => x.GetSuperTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(team);
        _teamMgrMock.Setup(x => x.DeleteMemberAsync(It.IsAny<Team>(), It.IsAny<Guid>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls