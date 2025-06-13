using ClArch.ValueObjects.Exceptions;
using ID.Application.Customers.Features.Account.Cmd.AddCustomerMemberMntc;
using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Features.Common.Dtos.User;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.Account.Cmd.AddCustomer_Mntc;

public class AddCustomerMemberCmdHandler_MntcTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _teamMgrMock;
    private readonly AddCustomerMemberCmdHandler_Mntc _handler;


    //- - - - - - - - - - - - - - - - - - //   

    public AddCustomerMemberCmdHandler_MntcTests()
    {
        _teamMgrMock = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new AddCustomerMemberCmdHandler_Mntc(_teamMgrMock.Object);
    }

    //--------------------------------------------//

    [Fact]
    public async Task ShouldCreateNewUserAndRegisterMemberSuccessfully()
    {
        // Arrange
        var request = new AddCustomerMemberCmd_Mntc(new AddCustomerMember_MntcDto
        {
            Email = "test@example.com",
            Username = "testuser",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1234567890",
            TeamPosition = 1,
            TeamId = Guid.NewGuid()
        });

        var team = TeamDataFactory.Create(
            id: Guid.NewGuid(),
            name: "Test Team",
            description: "Test description",
            subscriptions: null,
            TeamType.Customer
        );

        var newUser = AppUserDataFactory.Create(
            team.Id,
            firstName: "John",
            lastName: "Doe",
            userName: "testuser",
            email: "test@example.com",
            phoneNumber: "1234567890"
        );

        _teamMgrMock.Setup(m => m.GetByIdWithMembersAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(team);
        _teamMgrMock.Setup(m => m.RegisterMemberAsync(team, It.IsAny<AppUser>())).ReturnsAsync(GenResult<AppUser>.Success(newUser));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBeOfType<AppUser_Customer_Dto>();
        result.Value?.Email.ShouldBe("test@example.com");
        result.Value?.UserName.ShouldBe("testuser");
        result.Value?.PhoneNumber.ShouldBe("1234567890");

        _teamMgrMock.Verify(m => m.GetByIdWithMembersAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Once);
        _teamMgrMock.Verify(m => m.RegisterMemberAsync(It.IsAny<Team>(), It.IsAny<AppUser>()), Times.Once);
    }

    //--------------------------------------------//

    [Fact]
    public async Task ShouldReturnFailureWhenTeamNotFound()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var request = new AddCustomerMemberCmd_Mntc(new AddCustomerMember_MntcDto
        {
            Email = "test@example.com",
            Username = "testuser",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1234567890",
            TeamPosition = 1,
            TeamId = teamId
        });

        _teamMgrMock.Setup(m => m.GetByIdWithMembersAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync((Team?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<Team>(teamId));
    }

    //--------------------------------------------//

    [Fact]
    public async Task ShouldReturnFailureWhenTeam_NotCustomer()
    {
        // Arrange
        var team = TeamDataFactory.Create(
            id: Guid.NewGuid(),
            name: "Test Team",
            description: "Test description",
            null,
            TeamType.Super
        );

        var request = new AddCustomerMemberCmd_Mntc(new AddCustomerMember_MntcDto
        {
            Email = "test@example.com",
            Username = "testuser",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1234567890",
            TeamPosition = 1,
            TeamId = team.Id
        });


        var newUser = AppUserDataFactory.Create(
            team.Id,
            firstName: "John",
            lastName: "Doe",
            userName: "testuser",
            email: "test@example.com",
            phoneNumber: "1234567890"
        );

        _teamMgrMock.Setup(m => m.GetByIdWithMembersAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(team);
        _teamMgrMock.Setup(m => m.RegisterMemberAsync(team, It.IsAny<AppUser>())).ReturnsAsync(GenResult<AppUser>.Success(newUser));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.Teams.NotACustomerTeam(team.Id));
    }

    //--------------------------------------------//

    [Fact]
    public async Task ShouldThrow_WhenMemberCreationFailed()
    {
        // Arrange
        var request = new AddCustomerMemberCmd_Mntc(new AddCustomerMember_MntcDto
        {
            Email = "test@example.com",
            Username = "testuser",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1234567890",
            TeamPosition = 1,
            TeamId = Guid.NewGuid()
        });

        var team = TeamDataFactory.Create(
            id: Guid.NewGuid(),
            name: "Test Team",
            description: "Test description",
            subscriptions: null,
            TeamType.Customer
        );

        var newUser = AppUserDataFactory.Create(
            team.Id,
            firstName: "John",
            lastName: "Doe",
            userName: "testuser",
            email: "test@example.com",
            phoneNumber: "1234567890"
        );

        _teamMgrMock.Setup(m => m.GetByIdWithMembersAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(team);
        _teamMgrMock.Setup(m => m.RegisterMemberAsync(team, It.IsAny<AppUser>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(request, CancellationToken.None));

    }

    //--------------------------------------------//

    [Fact]
    public async Task ShouldHandleEmptyDTO()
    {
        // Arrange
        var request = new AddCustomerMemberCmd_Mntc(new AddCustomerMember_MntcDto());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.NotFound<Team>((Guid)default));
    }

    //--------------------------------------------//

    [Fact]
    public async Task ShouldHandleInvalidEmail()
    {
        // Arrange
        var request = new AddCustomerMemberCmd_Mntc(new AddCustomerMember_MntcDto
        {
            Email = "invalid.email",
            Username = "testuser",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1234567890",
            TeamPosition = 1,
            TeamId = Guid.NewGuid(),
        });

        _teamMgrMock.Setup(m => m.GetByIdWithMembersAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(TeamDataFactory.Create(
            id: Guid.NewGuid(),
            name: "Test Team",
            description: "Test description",
            subscriptions: null,
            TeamType.Customer
        ));

        // Act
        await Assert.ThrowsAsync<InvalidPropertyException>(() => _handler.Handle(request, CancellationToken.None));
    }

    //--------------------------------------------//



}//Cls