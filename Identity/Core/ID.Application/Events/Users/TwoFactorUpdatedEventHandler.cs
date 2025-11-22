using CollectionHelpers;
using ID.Application.AppAbs.MFA.AuthenticatorApps;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers.Events;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.GlobalSettings.Errors;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.TwoFactor;
using LoggingHelpers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ID.Application.Events.Users;

public class TwoFactorUpdatedEventHandler(
    IAuthenticatorAppService _authAppService,
    IEventBus _bus,
    IIdentityTeamManager<AppUser> _teamMgr,
    ILogger<TwoFactorUpdatedEventHandler> _logger)
    : INotificationHandler<User2FactorUpdatedDomainEvent>
{
    public async Task Handle(User2FactorUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var user = notification.User;
            var dbTeam = await _teamMgr.GetByIdWithMemberAsync(user.TeamId, user.Id);
            if (dbTeam ==  null || !dbTeam.Members.AnyValues()){
                _logger.LogError(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(user, user.TeamId.ToString()), IdErrorEvents.Listeners.TwoFactorAuthSetup);
                return;
            }

            BasicResult authResult = BasicResult.Success();
            switch (notification.Provider)
            {
                case TwoFactorProvider.AuthenticatorApp:
                    authResult = await SetupAuthenticatorAppAsync(dbTeam, notification.User, cancellationToken);
                    break;
                default:
                    break;//Otherwise do nothing
            }


            if (!authResult.Succeeded)
                _logger.LogBasicResultFailure(authResult, IdErrorEvents.Listeners.TwoFactorAuthSetup);

        }
        catch (Exception e)
        {
            _logger.LogException(e, IdErrorEvents.Listeners.TwoFactorAuthSetup);
        }

    }

    //----------------------//

    private async Task<BasicResult> SetupAuthenticatorAppAsync(Team team, AppUser user, CancellationToken cancellationToken)
    {
        var setupInfo = await _authAppService.Setup(user);

        var setKeyResult = await SetTwoFactorKeyAsync(team, user, setupInfo.CustomerSecretKey);
        if (!setKeyResult.Succeeded)
            return setKeyResult;

        await _bus.Publish(
          new TwoFactorGoogleSetupRequestIntegrationEvent(
              user,
              setupInfo.QrCodeImageData,
              setupInfo.TwoFactorSetupKey),
          cancellationToken);

        return BasicResult.Success("Handled");
    }

    //----------------------//

    private async Task<BasicResult> SetTwoFactorKeyAsync(Team team, AppUser user, string sid)
    {
        var dbUser = await _teamMgr.GetMemberAsync(user.TeamId, user.Id);
        if (dbUser is null)
            return BasicResult.NotFoundResult(IDMsgs.Error.Teams.NOT_TEAM_MEMBER(user, user.TeamId));
        dbUser.SetTwoFactorKey(sid);
        await _teamMgr.UpdateMemberAsync(team, dbUser);

        return BasicResult.Success();
    }

    //----------------------//



}
