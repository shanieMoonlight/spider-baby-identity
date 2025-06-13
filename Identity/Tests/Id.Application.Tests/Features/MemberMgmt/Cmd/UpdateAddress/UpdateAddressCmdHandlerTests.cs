using ID.Application.Features.Common.Dtos.User;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Application.Features.MemberMgmt.Cmd.UpdateAddress;
using ID.Application.AppAbs.Permissions;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Abstractions.Services.Teams;

namespace ID.Application.Tests.Features.MemberMgmt.Cmd.UpdateAddress;

public class UpdateAddressCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly Mock<IAppPermissionService<AppUser>> _mockAppPermissionService;
    private readonly UpdateAddressCmdHandler _handler;

    public UpdateAddressCmdHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _mockAppPermissionService = new Mock<IAppPermissionService<AppUser>>();
        _handler = new UpdateAddressCmdHandler(_mockTeamManager.Object, _mockAppPermissionService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenUpdateIsSuccessful()
    {
        // Arrange
        var appUser = AppUserDataFactory.Create(Guid.NewGuid());
        var identityAddressDto = new IdentityAddressDto(IdentityAddressDataFactory.Create(appUser.Id));
        var updateAddressCmd = new UpdateAddressCmd(identityAddressDto)
        {
            PrincipalUserId = appUser.Id,
            PrincipalTeam = TeamDataFactory.Create(appUser.TeamId)
        };

        _mockAppPermissionService.Setup(x => x.UpdatePermissions.CanUpdateAsync(appUser.Id, updateAddressCmd))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));

        _mockTeamManager.Setup(x => x.UpdateMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));

        // Act
        var result = await _handler.Handle(updateAddressCmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(appUser.Id);
        result.Value.Address.ShouldBeEquivalentTo(appUser.Address.ToDto());
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenPermissionCheckFails()
    {
        // Arrange
        var appUser = AppUserDataFactory.Create(Guid.NewGuid());
        var identityAddressDto = new IdentityAddressDto(IdentityAddressDataFactory.Create(appUser.Id));
        var updateAddressCmd = new UpdateAddressCmd(identityAddressDto)
        {
            PrincipalUserId = appUser.Id,
            PrincipalTeam = TeamDataFactory.Create(appUser.TeamId)
        };

        _mockAppPermissionService.Setup(x => x.UpdatePermissions.CanUpdateAsync(appUser.Id, updateAddressCmd))
            .ReturnsAsync(GenResult<AppUser>.Failure("Permission denied"));

        // Act
        var result = await _handler.Handle(updateAddressCmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe("Permission denied");
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenUpdateFails()
    {
        // Arrange
        var appUser = AppUserDataFactory.Create(Guid.NewGuid());
        var identityAddressDto = new IdentityAddressDto(IdentityAddressDataFactory.Create(appUser.Id));
        var updateAddressCmd = new UpdateAddressCmd(identityAddressDto)
        {
            PrincipalUserId = appUser.Id,
            PrincipalTeam = TeamDataFactory.Create(appUser.TeamId)
        };

        _mockAppPermissionService.Setup(x => x.UpdatePermissions.CanUpdateAsync(appUser.Id, updateAddressCmd))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));

        _mockTeamManager.Setup(x => x.UpdateMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()))
            .ReturnsAsync(GenResult<AppUser>.Failure("Update failed"));

        // Act
        var result = await _handler.Handle(updateAddressCmd, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe("Update failed");
    }

    //------------------------------------//

}
