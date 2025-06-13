using ID.Application.Events.Teams;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.Events;
using ID.Tests.Data.Factories;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace ID.Application.Tests.Events;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
public class TeamPositionRangeUpdatedDomainEventHandlerTests
{
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly Mock<ILogger<TeamPositionRangeUpdatedDomainEventHandler>> _mockLogger;
    private readonly TeamPositionRangeUpdatedDomainEventHandler _handler;

    public TeamPositionRangeUpdatedDomainEventHandlerTests()
    {
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _mockLogger = new Mock<ILogger<TeamPositionRangeUpdatedDomainEventHandler>>();
        _handler = new TeamPositionRangeUpdatedDomainEventHandler(_mockTeamManager.Object, _mockLogger.Object);
    }

    //------------------------------------//

    [Fact]
    public async Task Handle_TeamNotFound_LogsError()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = TeamDataFactory.Create(id: teamId);

        var notification = new TeamPositionRangeUpdatedDomainEvent(teamId, 5, 6);
        _mockTeamManager.Setup(m => m.GetByIdWithMembersAsync(teamId, It.IsAny<int>())).ReturnsAsync((Team?)null);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("was not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

    }

    //------------------------------------//

    [Fact]
    public async Task Handle_TeamFound_EnsuresMembersHaveValidTeamPositions()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var appUser1 = AppUserDataFactory.Create(teamId: teamId, teamPosition: 1);
        var appUser20 = AppUserDataFactory.Create(teamId: teamId, teamPosition: 20);
        var appUser3 = AppUserDataFactory.Create(teamId: teamId, teamPosition: 3);
        var expectedTeam = TeamDataFactory.Create(
            id: teamId,
            members: [appUser1, appUser3,  appUser20],
            minPosition: 2,
            maxPosition: 10);
        //SHouldn't matter what the range in the event is, as we're testing going to check the Team in the DB
        var notification = new TeamPositionRangeUpdatedDomainEvent(teamId, 2, 10);


        _mockTeamManager.Setup(m => m.GetByIdWithMembersAsync(teamId, It.IsAny<int>()))
            .ReturnsAsync(expectedTeam);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _mockTeamManager.Verify(m => m.GetByIdWithMembersAsync(teamId, It.IsAny<int>()), Times.Once);

        expectedTeam.Members.ShouldContain(appUser1);
        expectedTeam.Members.ShouldContain(appUser20);
        expectedTeam.Members.ShouldContain(appUser3);

        appUser1.TeamPosition.ShouldBe(2); // Should be adjusted to MinPosition
        appUser20.TeamPosition.ShouldBe(10); // Should be adjusted to MaxPosition
        appUser3.TeamPosition.ShouldBe(3); // Should be adjusted to MaxPosition
    }

    //------------------------------------//
}