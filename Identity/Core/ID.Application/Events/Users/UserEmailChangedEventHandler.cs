using ID.Application.AppAbs.EventBuses;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers.Events;
using ID.Domain.Entities.Teams;
using ID.GlobalSettings.Errors;
using LoggingHelpers;
using MediatR;
using Microsoft.Extensions.Logging;


namespace ID.Application.Events.Users;
internal class UserEmailChangedEventHandler(IEmailConfirmationBus bus, IIdentityTeamManager<AppUser> teamMgr, ILogger<UserEmailChangedEventHandler> logger)
    : INotificationHandler<UserEmailUpdatedDomainEvent>
{

    public async Task Handle(UserEmailUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {

            var team = await teamMgr.GetByIdWithMemberAsync(notification.TeamId, notification.UserId);

            if (team is null)
            {
                logger.LogError(new EventId(IdErrorEvents.Listeners.UserEmailUpdated), "{msg}", IDMsgs.Error.NotFound<Team>(notification.TeamId));
                return;
            }

            var member = team.Members.FirstOrDefault(m => m.Id == notification.UserId);
            if (member is null)
            {
                logger.LogError(new EventId(IdErrorEvents.Listeners.UserEmailUpdated), "{msg}", IDMsgs.Error.NotFound<Team>(notification.UserId));
                return;
            }

            await bus.GenerateTokenAndPublishEventAsync(member, team, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogException(ex, IdErrorEvents.Listeners.UserEmailUpdated);
        }
    }

}//Cls
