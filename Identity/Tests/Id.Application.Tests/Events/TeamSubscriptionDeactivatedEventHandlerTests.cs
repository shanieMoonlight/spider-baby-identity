using ID.Application.Events.Teams;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.Events;
using ID.Domain.Utility.Messages;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.Subscriptions;
using ID.Tests.Data.Factories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ID.Application.Tests.Events;

public class TeamSubscriptionDeactivatedEventHandlerTests
{
    private readonly Mock<IEventBus> _mockEventBus;
    private readonly Mock<IIdentityTeamManager<AppUser>> _mockTeamManager;
    private readonly Mock<ILogger<TeamSubscriptionDeactivatedDomainEvent>> _mockLogger;
    private readonly TeamSubscriptionDeactivatedEventHandler _handler;

    public TeamSubscriptionDeactivatedEventHandlerTests()
    {
        _mockEventBus = new Mock<IEventBus>();
        _mockEventBus.Setup(m => m.Publish(It.IsAny<SubscriptionsPausedIntegrationEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockTeamManager = new Mock<IIdentityTeamManager<AppUser>>();
        _mockLogger = new Mock<ILogger<TeamSubscriptionDeactivatedDomainEvent>>();
        _handler = new TeamSubscriptionDeactivatedEventHandler(
            _mockEventBus.Object,
            _mockTeamManager.Object,
            _mockLogger.Object
        );
    }

    //------------------------------//

    [Fact]
    public async Task Handle_TeamNotFound_LogsError()
    {
        // Arrange
        var notification = new TeamSubscriptionDeactivatedDomainEvent(
            SubscriptionDataFactory.Create()
        );
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _mockTeamManager.Setup(m => m.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync((Team)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(IDMsgs.Error.NotFound<Team>(notification.Subscription.TeamId))),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

    }

    //------------------------------//

    [Fact]
    public async Task Handle_SubscriptionNotFound_LogsError()
    {
        // Arrange
        var team = TeamDataFactory.Create();
        var notification = new TeamSubscriptionDeactivatedDomainEvent(
            SubscriptionDataFactory.Create()
        );
        _mockTeamManager.Setup(m => m.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(team);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(IDMsgs.Error.NotFound<AppUser>(notification.Subscription.Id))),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    //------------------------------//

    [Fact]
    public async Task Handle_LeaderNotFound_LogsError()
    {
        // Arrange
        var sub = SubscriptionDataFactory.Create();
        var team = TeamDataFactory.Create(subscriptions: [sub]);
        var notification = new TeamSubscriptionDeactivatedDomainEvent(sub);
        _mockTeamManager.Setup(m => m.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(team);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(IDMsgs.Error.NotFound<AppUser>(team.LeaderId))),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    //------------------------------//

    [Fact]
    public async Task Handle_ValidEvent_PublishesIntegrationEvent()
    {
        // Arrange
        var leader = AppUserDataFactory.Create(Guid.NewGuid());
        var subPlan = SubscriptionPlanDataFactory.Create();
        var subscription = SubscriptionDataFactory.Create(plan: subPlan);
        var team = TeamDataFactory.Create(
            leader: leader,
            subscriptions: [subscription]
        );
        var notification = new TeamSubscriptionDeactivatedDomainEvent(subscription);
        _mockTeamManager.Setup(m => m.GetByIdWithEverythingAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(team);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _mockEventBus.Verify(
            x => x.Publish(
                It.IsAny<SubscriptionsPausedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

}//Cls
