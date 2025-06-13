using ClArch.ValueObjects;
using ID.Application.AppAbs.Permissions;
using ID.Application.Features.Account.Cmd.AddSprMember;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using ID.Domain.Entities.AppUsers.ValueObjects;

namespace ID.Application.Tests.Features.Account.Cmd.AddSpr;

public class AddSprMemberCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly Mock<IAppPermissionService<AppUser>> _mockAppPermissionService;
    private readonly Mock<ICanAddPermissions> _canAddPermissionService;
    private readonly AddSprMemberCmdHandler _handler;

    //- - - - - - - - - - - - - - - - - - - - -//

    public AddSprMemberCmdHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _mockAppPermissionService = new Mock<IAppPermissionService<AppUser>>();
        _canAddPermissionService = new Mock<ICanAddPermissions>();
        _mockAppPermissionService.Setup(x => x.AddPermissions)
            .Returns(_canAddPermissionService.Object);
        _handler = new AddSprMemberCmdHandler(_mockTeamManager.Object, _mockAppPermissionService.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUAppPermisionFails()
    {
        // Arrange
        var newCustomerPosition = 5;
        var Dto = new AddSprMemberDto { TeamPosition = newCustomerPosition };
        var team = TeamDataFactory.Create();
        var request = new AddSprMemberCmd(Dto)
        {
            PrincipalTeam = team,
            PrincipalTeamPosition = newCustomerPosition - 1 //Less than newCustomerPosition
        };

        var expectedMsg = IDMsgs.Error.Authorization.UNAUTHORIZED_FOR_POSITION(newCustomerPosition);
        _canAddPermissionService.Setup(m => m.CanAddTeamMember(It.IsAny<int>(), It.IsAny<IIdPrincipalInfoRequest>()))
           .ReturnsAsync(BasicResult.Failure(expectedMsg));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(expectedMsg);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserIsRegisteredSuccessfully()
    {
        // Arrange
        var newCustomerPosition = 1;
        var team = TeamDataFactory.Create();
        var dto = new AddSprMemberDto
        {
            Email = "test@example.com",
            Username = "testuser",
            PhoneNumber = "1234567890",
            FirstName = "Test",
            LastName = "User",
            TeamPosition = newCustomerPosition
        };

        var request = new AddSprMemberCmd(dto)
        {
            Dto = new AddSprMemberDto
            {
                Email = "test@example.com",
                Username = "testuser",
                PhoneNumber = "1234567890",
                FirstName = "Test",
                LastName = "User",
                TeamPosition = 1
            },
            IsAuthenticated = true,
            Principal = new System.Security.Claims.ClaimsPrincipal(),
            PrincipalTeam = team,
            PrincipalTeamPosition = newCustomerPosition + 1 //Greater than newCustomerPosition
        };

        var newUser = AppUser.Create(
            request.PrincipalTeam,
            EmailAddress.Create(request.Dto.Email),
            UsernameNullable.Create(request.Dto.Username),
            PhoneNullable.Create(request.Dto.PhoneNumber),
            FirstNameNullable.Create(request.Dto.FirstName),
            LastNameNullable.Create(request.Dto.LastName),
            TeamPositionNullable.Create(request.Dto.TeamPosition)
        );

        _mockTeamManager.Setup(m => m.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()))
            .ReturnsAsync(GenResult<AppUser>.Success(newUser));

        _canAddPermissionService.Setup(m => m.CanAddTeamMember(It.IsAny<int>(), It.IsAny<IIdPrincipalInfoRequest>()))
           .ReturnsAsync(BasicResult.Success());

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
        var dto = new AddSprMemberDto
        {
            Email = "test@example.com",
            Username = "testuser",
            PhoneNumber = "1234567890",
            FirstName = "Test",
            LastName = "User",
            TeamPosition = 1
        };

        var request = new AddSprMemberCmd(dto)
        {

            Principal = new System.Security.Claims.ClaimsPrincipal(),
            PrincipalTeam = TeamDataFactory.Create(),
            PrincipalTeamPosition = newCustomerPosition + 1 //Greater than newCustomerPosition
        };

        _mockTeamManager.Setup(m => m.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()))
                   .ReturnsAsync(GenResult<AppUser>.Success(AppUserDataFactory.Create()));

        _canAddPermissionService.Setup(m => m.CanAddTeamMember(It.IsAny<int>(), It.IsAny<IIdPrincipalInfoRequest>()))
           .ReturnsAsync(BasicResult.Success());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockTeamManager.Verify(m => m.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Once);
    }

    //------------------------------------//


}//Cls