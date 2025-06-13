using ID.Application.AppAbs.Permissions;
using ID.Application.Customers.Features.MemberMgmt.Cmd.DeleteCustomerMember;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.MemberMgmt.Cmd.DeleteCustomerMember;

#pragma warning disable CS8600 // Cannot convert null literal to non-nullable reference type.
public class DeleteCusMemberCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly Mock<ICanDeletePermissions<AppUser>> _mockCanDeletePermissionService;
    private readonly Mock<IAppPermissionService<AppUser>> _mockAppPermissionService;
    private readonly DeleteCustomerMemberCmdHandler _handler;

    public DeleteCusMemberCmdHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _mockAppPermissionService = new Mock<IAppPermissionService<AppUser>>();
        _mockCanDeletePermissionService = new Mock<ICanDeletePermissions<AppUser>>();
        _mockAppPermissionService.Setup(x => x.DeletePermissions)
            .Returns(_mockCanDeletePermissionService.Object);
        _handler = new DeleteCustomerMemberCmdHandler(_mockTeamManager.Object, _mockAppPermissionService.Object);
    }


    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_When_CanDeleteFails()
    {
        // Arrange
        var request = new DeleteCustomerMemberCmd(Guid.NewGuid());
        var canDeleteUserResult = GenResult<AppUser>.Success(AppUserDataFactory.Create(Guid.NewGuid()));
        _mockCanDeletePermissionService.Setup(x => x.CanDeleteAsync(It.IsAny<Guid>(), It.IsAny<DeleteCustomerMemberCmd>()))
            .ReturnsAsync(GenResult<AppUser>.ForbiddenResult());
        _mockTeamManager.Setup(x => x.GetMntcTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync((Team)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Forbidden.ShouldBeTrue();
        result.Succeeded.ShouldBeFalse();
    }

    //------------------------------------//


    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenMemberIsDeleted()
    {
        // Arrange
        var request = new DeleteCustomerMemberCmd(Guid.NewGuid());
        var appUser = AppUserDataFactory.Create(Guid.NewGuid());
        var canDeleteUserResult = GenResult<AppUser>.Success(appUser);
        var team = TeamDataFactory.Create();
        _mockCanDeletePermissionService.Setup(x => x.CanDeleteAsync(It.IsAny<Guid>(), It.IsAny<DeleteCustomerMemberCmd>()))
            .ReturnsAsync(canDeleteUserResult);
        _mockTeamManager.Setup(x => x.GetMntcTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(team);
        _mockTeamManager.Setup(x => x.DeleteMemberAsync(It.IsAny<Team>(), It.IsAny<Guid>()))
            .ReturnsAsync(BasicResult.Success());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BasicResult>();
        result.Succeeded.ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenMemberIsDeleted()
    {
        // Arrange
        var request = new DeleteCustomerMemberCmd(Guid.NewGuid());
        var appUser = AppUserDataFactory.Create(Guid.NewGuid());
        var canDeleteUserResult = GenResult<AppUser>.Success(appUser);
        var team = TeamDataFactory.Create();
        _mockCanDeletePermissionService.Setup(x => x.CanDeleteAsync(It.IsAny<Guid>(), It.IsAny<DeleteCustomerMemberCmd>()))
            .ReturnsAsync(canDeleteUserResult);
        _mockTeamManager.Setup(x => x.GetMntcTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(team);
        _mockTeamManager.Setup(x => x.DeleteMemberAsync(It.IsAny<Team>(), It.IsAny<Guid>()))
            .ReturnsAsync(BasicResult.BadRequestResult("Something is wrong"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.BadRequest.ShouldBeTrue();
    }

    //------------------------------------//

}//Cls