using ID.IntegrationEvents.Abstractions;
using LoggingHelpers;
using MediatR;
using Microsoft.Extensions.Logging;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Domain.Utility.Messages;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.Events;
using ID.Domain.Abstractions.Services.Teams;
using ID.GlobalSettings.Errors;

namespace ID.Application.Events.Users;

public record TwoFactorEnableChangedEventHandler(
    IAuthenticatorAppService AuthAppService,
    IEventBus Bus,
    IIdentityTeamManager<AppUser> TeamMgr,
    ITwoFactorVerificationService<AppUser> _2FactorService,
    ILogger<TwoFactorEnableChangedEventHandler> Logger)
    : INotificationHandler<User2FactorEnableChangedDomainEvent>
{
    public async Task Handle(User2FactorEnableChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var user = notification.User;
            var dbUser = await TeamMgr.GetMemberAsync(user.TeamId, user.Id);
            if (dbUser is null)
            {
                Logger.LogError(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(user, user.TeamId.ToString()), IdErrorEvents.Listeners.TwoFactorUpdated);
                return;
            }


            var authResult = await _2FactorService.SetTwoFactorEnabledAsync(dbUser, notification.Enabled);

            if (!authResult.Succeeded)
                Logger.LogBasicResultFailure(authResult, IdErrorEvents.Listeners.TwoFactorUpdated);

        }
        catch (Exception e)
        {
            Logger.LogException(e, IdErrorEvents.Listeners.TwoFactorUpdated);
        }

    }

}
