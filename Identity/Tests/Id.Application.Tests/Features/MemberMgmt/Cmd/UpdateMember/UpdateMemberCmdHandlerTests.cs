using ID.Application.AppAbs.Permissions;
using ID.Application.Dtos.User;
using ID.Application.Features.MemberMgmt.Cmd.UpdateSelf;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;


namespace ID.Application.Tests.Features.MemberMgmt.Cmd.UpdateMember;


public class UpdateMemberCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly Mock<IAppPermissionService<AppUser>> _mockAppPermissionService;
    private readonly Mock<ICanUpdatePermissions<AppUser>> _mockCanUpdatePermissionService;
    private readonly UpdateSelfCmdHandler _handler;

    public UpdateMemberCmdHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _mockAppPermissionService = new Mock<IAppPermissionService<AppUser>>();
        _mockCanUpdatePermissionService = new Mock<ICanUpdatePermissions<AppUser>>();
        _mockAppPermissionService.Setup(x => x.UpdatePermissions)
            .Returns(_mockCanUpdatePermissionService.Object);
        _handler = new UpdateSelfCmdHandler(_mockTeamManager.Object, _mockAppPermissionService.Object);
    }

    //------------------------------------//


    [Fact]
    public async Task Handle_ShouldCallCanUpdateAsync()
    {
        // Arrange
        var appUser = AppUserDataFactory.Create(Guid.NewGuid());
        var expectedTeam = TeamDataFactory.Create(Guid.NewGuid());
        var updateMemberDto = new UpdateSelfDto
        {
            Id = appUser.Id,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            Username = appUser.UserName,
            Email = appUser.Email!,
            PhoneNumber = appUser.PhoneNumber,
            TwoFactorProvider = appUser.TwoFactorProvider,
            TwoFactorEnabled = appUser.TwoFactorEnabled,
            TeamId = expectedTeam.Id
        };
        var request = new UpdateSelfCmd(updateMemberDto) { PrincipalTeam = expectedTeam };

        _mockCanUpdatePermissionService.Setup(x => x.CanUpdateAsync(appUser.Id, request))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));

        _mockTeamManager.Setup(x => x.UpdateMemberAsync(expectedTeam, appUser))
           .ReturnsAsync(GenResult<AppUser>.Success(appUser));


        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Verify that UpdateMemberAsync was called
        _mockCanUpdatePermissionService.Verify(x => x.CanUpdateAsync(appUser.Id, request), Times.AtLeastOnce);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldUpdateMember_WhenUserHasPermission()
    {
        // Arrange
        var appUser = AppUserDataFactory.Create(Guid.NewGuid());
        var expectedTeam = TeamDataFactory.Create(Guid.NewGuid());
        var updateMemberDto = new UpdateSelfDto
        {
            Id = appUser.Id,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            Username = appUser.UserName,
            Email = appUser.Email!,
            PhoneNumber = appUser.PhoneNumber,
            TwoFactorProvider = appUser.TwoFactorProvider,
            TwoFactorEnabled = appUser.TwoFactorEnabled,
            TeamId = expectedTeam.Id
        };
        var request = new UpdateSelfCmd(updateMemberDto) { PrincipalTeam = expectedTeam };

        _mockCanUpdatePermissionService.Setup(x => x.CanUpdateAsync(appUser.Id, request))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));

        _mockTeamManager.Setup(x => x.UpdateMemberAsync(expectedTeam, appUser))
            .ReturnsAsync(GenResult<AppUser>.Success(appUser));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(appUser.Id);
        result.Value.ShouldBeEquivalentTo(appUser.ToDto());

        // Verify that UpdateMemberAsync was called
        _mockTeamManager.Verify(x => x.UpdateMemberAsync(expectedTeam, appUser), Times.Once);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserHasNoPermission()
    {
        // Arrange
        var appUser = AppUserDataFactory.Create(Guid.NewGuid());
        var expectedTeam = TeamDataFactory.Create(Guid.NewGuid());
        var updateMemberDto = new UpdateSelfDto
        {
            Id = appUser.Id,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            Username = appUser.UserName,
            Email = appUser.Email!,
            PhoneNumber = appUser.PhoneNumber,
            //TeamPosition = appUser.TeamPosition,
            TwoFactorProvider = appUser.TwoFactorProvider,
            TwoFactorEnabled = appUser.TwoFactorEnabled,
            TeamId = expectedTeam.Id
        };
        var request = new UpdateSelfCmd(updateMemberDto) { PrincipalTeam = expectedTeam };

        _mockCanUpdatePermissionService.Setup(x => x.CanUpdateAsync(appUser.Id, request))
            .ReturnsAsync(GenResult<AppUser>.Failure("Permission denied"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
    }

    //------------------------------------//

}//Cls
