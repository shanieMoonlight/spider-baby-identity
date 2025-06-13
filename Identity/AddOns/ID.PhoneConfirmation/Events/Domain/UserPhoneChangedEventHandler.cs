using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.Events;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.PhoneConfirmation.Events.Integration.Bus;
using LoggingHelpers;
using MediatR;
using Microsoft.Extensions.Logging;
using ID.GlobalSettings.Errors;


namespace ID.PhoneConfirmation.Events.Domain;


/// <summary>
/// Handles the UserPhoneUpdatedDomainEvent by listening for the domain event and publishing it as an integration event.
/// </summary>
internal class UserPhoneChangedEventHandler(
    IPhoneConfirmationBus bus,
    IIdentityTeamManager<AppUser> teamRepo,
    ILogger<UserPhoneChangedEventHandler> logger)
    : INotificationHandler<UserPhoneUpdatedDomainEvent>
{

    /// <summary>
    /// Handles the UserPhoneUpdatedDomainEvent.
    /// </summary>
    /// <param name="notification">The domain event notification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task Handle(UserPhoneUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var user = notification.User;
            var phone = notification.Phone;

            if (phone is null)
            {
                logger.LogError("No Phone number supplied for Phone Changed Event", IdErrorEvents.Listeners.UserPhoneUpdated);
                return; //do nothing
            }

            var team = await teamRepo.GetByIdWithMembersAsync(user.TeamId);
            if (team is null)
            {
                logger.LogError(IDMsgs.Error.NotFound<Team>(user.TeamId), IdErrorEvents.Listeners.UserPhoneUpdated);
                return; //do nothing
            }

            var dbUser = team.Members.FirstOrDefault(m => m.Id == user.Id);
            if (dbUser is null)
            {
                logger.LogError(new EventId(IdErrorEvents.Listeners.UserPhoneUpdated), "{msg}", IDMsgs.Error.NotFound<Team>(notification.User.TeamId));
                return;
            }

            await bus.GenerateTokenAndPublishEventAsync(dbUser, team, cancellationToken);

        }
        catch (Exception ex)
        {
            logger.LogException(ex, IdErrorEvents.Listeners.UserPhoneUpdated);
        }
    }
}
