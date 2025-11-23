using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers.Events;
using ID.GlobalSettings.Errors;
using LoggingHelpers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ID.Application.Events.Users;

public class TwoFactorEnableChangedEventHandler(
    IIdentityTeamManager<AppUser> _teamMgr,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    ILogger<TwoFactorEnableChangedEventHandler> _logger)
    : INotificationHandler<User2FactorEnableChangedDomainEvent>
{
    public async Task Handle(User2FactorEnableChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var user = notification.User;
            var dbUser = await _teamMgr.GetMemberAsync(user.TeamId, user.Id);
            if (dbUser is null)
            {
                _logger.LogError(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(user, user.TeamId.ToString()), IdErrorEvents.Listeners.TwoFactorUpdated);
                return;
            }


            var authResult = await _2FactorService.SetTwoFactorEnabledAsync(dbUser, notification.Enabled);

            if (!authResult.Succeeded)
                _logger.LogBasicResultFailure(authResult, IdErrorEvents.Listeners.TwoFactorUpdated);

        }
        catch (Exception e)
        {
            _logger.LogException(e, IdErrorEvents.Listeners.TwoFactorUpdated);
        }

    }

}
