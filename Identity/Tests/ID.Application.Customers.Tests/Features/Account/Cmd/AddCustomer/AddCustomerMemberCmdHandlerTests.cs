using ClArch.ValueObjects;
using ID.Application.AppAbs.Permissions;
using ID.Application.Customers.Features.Account.Cmd.AddCustomerMember;
using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.CqrsAbs;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;
using System.Security.Claims;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.AddCustomer;

public class AddCustomerMemberCmdHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly AddCustomerMemberCmdHandler _handler;
    private readonly Mock<IAppPermissionService<AppUser>> _mockAppPermissionService;
    private readonly Mock<ICanAddPermissions> _canAddPermissionService;

    //- - - - - - - - - - - - - - - - - - - - -//

    public AddCustomerMemberCmdHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _mockAppPermissionService = new Mock<IAppPermissionService<AppUser>>();
        _canAddPermissionService = new Mock<ICanAddPermissions>();
        _mockAppPermissionService.Setup(x => x.AddPermissions)
            .Returns(_canAddPermissionService.Object);
        _handler = new AddCustomerMemberCmdHandler(_mockTeamManager.Object, _mockAppPermissionService.Object);
    }

    //------------------------------------//


    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUAppPermisionFails()
    {
        // Arrange
        var newCustomerPosition = 1;
        var team = TeamDataFactory.Create();
        var dto = new AddCustomerMemberDto
        {
            Email = "test@example.com",
            Username = "testuser",
            PhoneNumber = "1234567890",
            FirstName = "Test",
            LastName = "User",
            TeamPosition = newCustomerPosition
        };

        var request = new AddCustomerMemberCmd(dto)
        {
            Dto = new AddCustomerMemberDto
            {
                Email = "test@example.com",
                Username = "testuser",
                PhoneNumber = "1234567890",
                FirstName = "Test",
                LastName = "User",
                TeamPosition = 1
            },
            IsAuthenticated = true,
            Principal = new ClaimsPrincipal(),
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

        _canAddPermissionService.Setup(m => m.CanAddCustomerTeamMember(It.IsAny<int>(), It.IsAny<IIdPrincipalInfoRequest>()))
           .ReturnsAsync(BasicResult.Failure());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
    }


    //------------------------------------//

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserIsRegisteredSuccessfully()
    {
        // Arrange
        var newCustomerPosition = 1;
        var team = TeamDataFactory.Create();
        var dto = new AddCustomerMemberDto
        {
            Email = "test@example.com",
            Username = "testuser",
            PhoneNumber = "1234567890",
            FirstName = "Test",
            LastName = "User",
            TeamPosition = newCustomerPosition
        };

        var request = new AddCustomerMemberCmd(dto)
        {
            Dto = new AddCustomerMemberDto
            {
                Email = "test@example.com",
                Username = "testuser",
                PhoneNumber = "1234567890",
                FirstName = "Test",
                LastName = "User",
                TeamPosition = 1
            },
            IsAuthenticated = true,
            Principal = new ClaimsPrincipal(),
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

        _canAddPermissionService.Setup(m => m.CanAddCustomerTeamMember(It.IsAny<int>(), It.IsAny<IIdPrincipalInfoRequest>()))
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
        var dto = new AddCustomerMemberDto
        {
            Email = "test@example.com",
            Username = "testuser",
            PhoneNumber = "1234567890",
            FirstName = "Test",
            LastName = "User",
            TeamPosition = 1
        };

        var request = new AddCustomerMemberCmd(dto)
        {

            Principal = new ClaimsPrincipal(),
            PrincipalTeam = TeamDataFactory.Create(),
            PrincipalTeamPosition = newCustomerPosition + 1 //Greater than newCustomerPosition
        };

        _canAddPermissionService.Setup(m => m.CanAddCustomerTeamMember(It.IsAny<int>(), It.IsAny<IIdPrincipalInfoRequest>()))
           .ReturnsAsync(BasicResult.Success());

        _mockTeamManager.Setup(m => m.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()))
                   .ReturnsAsync(GenResult<AppUser>.Success(AppUserDataFactory.Create()));


        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockTeamManager.Verify(m => m.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Once);
    }

    //------------------------------------//


}//Cls