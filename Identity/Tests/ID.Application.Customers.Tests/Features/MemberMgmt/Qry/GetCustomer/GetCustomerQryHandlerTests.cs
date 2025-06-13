using ID.Application.Customers.Features.Common.Dtos.User;
using ID.Application.Customers.Features.MemberMgmt.Qry.GetCustomer;
using ID.Application.Features.Common.Dtos.User;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Tests.Data.Factories;
using Moq;
using MyResults;
using Shouldly;

namespace ID.Application.Customers.Tests.Features.MemberMgmt.Qry.GetCustomer;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public class GetCustomerQryHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly GetCustomerQryHandler _handler;

    public GetCustomerQryHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _handler = new GetCustomerQryHandler(_mockTeamManager.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_UserExists_ReturnsUserDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var appUser = AppUserDataFactory.Create(teamId: teamId, id: userId);
        var team = TeamDataFactory.Create(teamId, members: [appUser]);
        var request = new GetCustomerQry(teamId, userId);

        _mockTeamManager.Setup(m => m.GetByIdWithMemberAsync(teamId, userId))
            .ReturnsAsync(team);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TeamId.ShouldBe(teamId);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsNotFoundResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var request = new GetCustomerQry(userId, teamId);

        _mockTeamManager.Setup(m => m.GetByIdWithMemberAsync(teamId, userId))
            .ReturnsAsync((Team)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.ShouldBeOfType<GenResult<AppUser_Customer_Dto>>();
        result.Info.ShouldContain("not a member");
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsNotFoundResult2()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var request = new GetCustomerQry(teamId, userId);
        var appUser = AppUserDataFactory.Create(teamId: teamId, id: userId);
        var otherAppUser = AppUserDataFactory.Create(teamId: teamId, id: userId);
        var team = TeamDataFactory.Create(teamId, members: [otherAppUser]);

        _mockTeamManager.Setup(m => m.GetByIdWithMemberAsync(teamId, userId))
            .ReturnsAsync((Team)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Succeeded.ShouldBeFalse();
        result.Info.ShouldBe(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(userId, teamId.ToString()));
        result.ShouldBeOfType<GenResult<AppUser_Customer_Dto>>();
    }

    //------------------------------------//

}//Cmd

