using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.Events;
using ID.Domain.Utility.Messages;
using ID.GlobalSettings.Errors;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.Subscriptions;
using LoggingHelpers;
using MediatR;
using Microsoft.Extensions.Logging;



namespace ID.Application.Events.Teams;
internal class TeamSubscriptionDeactivatedEventHandler(
    IEventBus bus,
    IIdentityTeamManager<AppUser> teamMgr,
    ILogger<TeamSubscriptionDeactivatedDomainEvent> logger)
    : INotificationHandler<TeamSubscriptionDeactivatedDomainEvent>
{

    public async Task Handle(TeamSubscriptionDeactivatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {

            var team = await teamMgr.GetByIdWithEverythingAsync(notification.Subscription.TeamId);

            if (team is null)
            {
                logger.LogError(new EventId(IdErrorEvents.Listeners.TeamSubscriptionDeactivated), "{msg}", IDMsgs.Error.NotFound<Team>(notification.Subscription.TeamId));
                return;
            }

            var sub = team.Subscriptions.FirstOrDefault(s => s.Id == notification.Subscription.Id);
            if (sub is null)
            {
                logger.LogError(new EventId(IdErrorEvents.Listeners.TeamSubscriptionDeactivated), "{msg}", IDMsgs.Error.NotFound<AppUser>(notification.Subscription.Id));
                return;
            }

            var leader = team.Members.FirstOrDefault(m => m.Id == team.LeaderId);
            if (leader is null)
            {
                logger.LogError(new EventId(IdErrorEvents.Listeners.TeamSubscriptionDeactivated), "{msg}", IDMsgs.Error.NotFound<AppUser>(team.LeaderId));
                return;
            }

            // ! is ok because GetByIdWithEverythingAsync makes sure everything is attached
            await bus.Publish(
                new SubscriptionsPausedIntegrationEvent(
                    leader.Id,
                    sub.Id,
                    team.Leader!.Email!,
                    team.Leader!.FriendlyName,
                    sub.SubscriptionPlan!.Name),
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            logger.LogException(ex, IdErrorEvents.Listeners.TeamSubscriptionDeactivated);
        }
    }

}//Cls
