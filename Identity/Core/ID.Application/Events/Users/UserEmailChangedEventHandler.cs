using MediatR;
using Microsoft.Extensions.Logging;
using ID.Application.AppAbs.ApplicationServices;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.Events;
using ID.Domain.Abstractions.Services.Teams;
using ID.GlobalSettings.Errors;
using LoggingHelpers;


namespace ID.Application.Events.Users;
internal class UserEmailChangedEventHandler(IEmailConfirmationBus bus, IIdentityTeamManager<AppUser> teamMgr, ILogger<UserEmailChangedEventHandler> logger)
    : INotificationHandler<UserEmailUpdatedDomainEvent>
{

    public async Task Handle(UserEmailUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {

            var team = await teamMgr.GetByIdWithMemberAsync(notification.User.TeamId, notification.User.Id);

            if (team is null)
            {
                logger.LogError(new EventId(IdErrorEvents.Listeners.UserEmailUpdated), "{msg}", IDMsgs.Error.NotFound<Team>(notification.User.TeamId));
                return;
            }

            var member = team.Members.FirstOrDefault(m => m.Id == notification.User.Id);
            if (member is null)
            {
                logger.LogError(new EventId(IdErrorEvents.Listeners.UserEmailUpdated), "{msg}", IDMsgs.Error.NotFound<Team>(notification.User));
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
