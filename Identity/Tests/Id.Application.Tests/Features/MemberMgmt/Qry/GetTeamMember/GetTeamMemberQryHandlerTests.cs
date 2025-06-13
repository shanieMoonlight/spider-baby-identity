using ID.Application.AppAbs.Permissions;
using ID.Application.Dtos.User;
using ID.Application.Features.MemberMgmt.Qry.GetTeamMemberQry;
using ID.Domain.Entities.AppUsers;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Tests.Features.MemberMgmt.Qry.GetTeamMember;

public class GetTeamMemberQryHandlerTests
{
    private readonly Mock<ICanViewTeamMemberPermissions<AppUser>> _mockCanViewPermissionService;
    private readonly Mock<IAppPermissionService<AppUser>> _mockAppPermissionService;
    private readonly GetTeamMemberQryHandler _handler;

    public GetTeamMemberQryHandlerTests()
    {
        _mockAppPermissionService = new Mock<IAppPermissionService<AppUser>>();
        _mockCanViewPermissionService = new Mock<ICanViewTeamMemberPermissions<AppUser>>();
        _mockAppPermissionService.Setup(x => x.ViewTeamMemberPermissions)
            .Returns(_mockCanViewPermissionService.Object);
        _handler = new GetTeamMemberQryHandler(_mockAppPermissionService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnAppUserDto_WhenUserHasPermission()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var request = new GetTeamMemberQry(teamId, memberId);
        var appUser = AppUserDataFactory.Create(teamId: teamId);
        var appUserDto = appUser.ToDto();
        var genResult = GenResult<AppUser>.Success(appUser);

        _mockCanViewPermissionService
            .Setup(x => x.CanViewTeamMemberAsync(teamId, memberId, request))
            .ReturnsAsync(genResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBeEquivalentTo(appUserDto);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotHavePermission()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var request = new GetTeamMemberQry(teamId, memberId);
        var genResult = GenResult<AppUser>.UnauthorizedResult("Unauthorized");

        _mockCanViewPermissionService
            .Setup(x => x.CanViewTeamMemberAsync(teamId, memberId, request))
            .ReturnsAsync(genResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Unauthorized.ShouldBeTrue();
    }

    //------------------------------------//

}
