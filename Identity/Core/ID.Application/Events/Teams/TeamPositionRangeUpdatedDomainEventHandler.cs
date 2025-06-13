using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.Teams.Events;
using ID.Domain.Utility.Messages;
using MediatR;
using Microsoft.Extensions.Logging;
using ID.GlobalSettings.Errors;
using LoggingHelpers;


namespace ID.Application.Events.Teams;
internal class TeamPositionRangeUpdatedDomainEventHandler(
    IIdentityTeamManager<AppUser> teamMgr,
    ILogger<TeamPositionRangeUpdatedDomainEventHandler> logger)
    : INotificationHandler<TeamPositionRangeUpdatedDomainEvent>
{

    public async Task Handle(TeamPositionRangeUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var team = await teamMgr.GetByIdWithMembersAsync(notification.TeamId);

            if (team is null)
            {
                logger.Log(
                    LogLevel.Error,
                    new EventId(IdErrorEvents.Listeners.TeamPositionRangeUpdated),
                    "{msg}",
                    IDMsgs.Error.NotFound<Team>(notification.TeamId));
                return;
            }

            team.EnsureMembersHaveValidTeamPositions();
            await teamMgr.UpdateAsync(team);

        }
        catch (Exception ex)
        {
            logger.LogException(ex, IdErrorEvents.Listeners.TeamPositionRangeUpdated);
        }

    }

}//Cls
