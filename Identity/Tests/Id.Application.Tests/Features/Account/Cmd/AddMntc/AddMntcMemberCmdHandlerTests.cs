using ClArch.ValueObjects;
using ID.Application.AppAbs.Permissions;
using ID.Application.Features.Account.Cmd.AddMntcMember;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Domain.Entities.AppUsers.ValueObjects;

namespace ID.Application.Tests.Features.Account.Cmd.AddMntc;

public class AddMntcMemberCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly AddMntcMemberCmdHandler _handler;
    private readonly Mock<IAppPermissionService<AppUser>> _mockAppPermissionService;
    private readonly Mock<ICanAddPermissions> _canAddPermissionService;

    //- - - - - - - - - - - - - - - - - - - - -//

    public AddMntcMemberCmdHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _mockAppPermissionService = new Mock<IAppPermissionService<AppUser>>();
        _canAddPermissionService = new Mock<ICanAddPermissions>();
        _mockAppPermissionService.Setup(x => x.AddPermissions)
            .Returns(_canAddPermissionService.Object);
        _handler = new AddMntcMemberCmdHandler(_mockTeamManager.Object, _mockAppPermissionService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserCannotAdministerRole_MNTC()
    {
        // Arrange
        var newCustomerPosition = 5;
        var Dto = new AddMntcMemberDto { TeamPosition = newCustomerPosition };
        var team = TeamDataFactory.Create();
        var request = new AddMntcMemberCmd(Dto)
        {
            PrincipalTeamPosition = newCustomerPosition - 1 //Less than newCustomerPosition
        };


        _canAddPermissionService.Setup(m => m.CanAddTeamMember(It.IsAny<int>(), It.IsAny<IIdPrincipalInfoRequest>()))
           .ReturnsAsync(BasicResult.Failure());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserCanAdministerRole_SPR()
    {
        // Arrange
        var newCustomerPosition = 5;
        var dto = new AddMntcMemberDto
        {
            Email = "test@example.com",
            Username = "testuser",
            PhoneNumber = "1234567890",
            FirstName = "Test",
            LastName = "User",
            TeamPosition = newCustomerPosition
        };
        var team = TeamDataFactory.Create();
        var request = new AddMntcMemberCmd(dto)
        {
            IsSuper = true,
            PrincipalTeamPosition = newCustomerPosition - 1 //Less than newCustomerPosition
        };
        _mockTeamManager.Setup(m => m.GetMntcTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(team);

        _mockTeamManager.Setup(m => m.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()))
            .ReturnsAsync(GenResult<AppUser>.Success(AppUserDataFactory.Create()));


        _canAddPermissionService.Setup(m => m.CanAddTeamMember(It.IsAny<int>(), It.IsAny<IIdPrincipalInfoRequest>()))
           .ReturnsAsync(BasicResult.Failure()); //Will fail but should never be called in the first place

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        _mockTeamManager.Verify(m => m.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Once());
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserIsRegisteredSuccessfully()
    {
        // Arrange
        var newCustomerPosition = 1;
        var team = TeamDataFactory.Create();
        var dto = new AddMntcMemberDto
        {
            Email = "test@example.com",
            Username = "testuser",
            PhoneNumber = "1234567890",
            FirstName = "Test",
            LastName = "User",
            TeamPosition = newCustomerPosition
        };

        var request = new AddMntcMemberCmd(dto)
        {
            IsAuthenticated = true,
            Principal = new System.Security.Claims.ClaimsPrincipal(),
            //PrincipalTeam = team,
            PrincipalTeamPosition = newCustomerPosition + 1 //Greater than newCustomerPosition
        };

        _canAddPermissionService.Setup(m => m.CanAddTeamMember(It.IsAny<int>(), It.IsAny<IIdPrincipalInfoRequest>()))
           .ReturnsAsync(BasicResult.Success());

        var newUser = AppUser.Create(
           team,
            EmailAddress.Create(request.Dto.Email),
            UsernameNullable.Create(request.Dto.Username),
            PhoneNullable.Create(request.Dto.PhoneNumber),
            FirstNameNullable.Create(request.Dto.FirstName),
            LastNameNullable.Create(request.Dto.LastName),
            TeamPositionNullable.Create(request.Dto.TeamPosition)
        );

        _mockTeamManager.Setup(m => m.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()))
            .ReturnsAsync(GenResult<AppUser>.Success(newUser));

        _mockTeamManager.Setup(m => m.GetMntcTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(team);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeAssignableTo<AppUserDto>();
        result.Value.Email.ShouldBe(request.Dto.Email);
        result.Value.UserName.ShouldBe(request.Dto.Username);
        result.Value.FirstName.ShouldBe(request.Dto.FirstName);
        result.Value.LastName.ShouldBe(request.Dto.LastName);
        result.Value.TeamPosition.ShouldBe(request.Dto.TeamPosition);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_Should_Call_RegisterMemberAsync()
    {
        // Arrange
        var newCustomerPosition = 1;
        var dto = new AddMntcMemberDto
        {
            Email = "test@example.com",
            Username = "testuser",
            PhoneNumber = "1234567890",
            FirstName = "Test",
            LastName = "User",
            TeamPosition = 1
        };
        var mntcTeam = TeamDataFactory.Create();
        var request = new AddMntcMemberCmd(dto)
        {

            Principal = new System.Security.Claims.ClaimsPrincipal(),
            //PrincipalTeam = TeamDataFactory.Create(),
            PrincipalTeamPosition = newCustomerPosition + 1 //Greater than newCustomerPosition
        };

        _mockTeamManager.Setup(m => m.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()))
                   .ReturnsAsync(GenResult<AppUser>.Success(AppUserDataFactory.Create()));

        _mockTeamManager.Setup(m => m.GetMntcTeamWithMembersAsync(It.IsAny<int>()))
            .ReturnsAsync(mntcTeam);

        _canAddPermissionService.Setup(m => m.CanAddTeamMember(It.IsAny<int>(), It.IsAny<IIdPrincipalInfoRequest>()))
           .ReturnsAsync(BasicResult.Success());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockTeamManager.Verify(m => m.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Once);
    }

    //------------------------------------//


}//Cls