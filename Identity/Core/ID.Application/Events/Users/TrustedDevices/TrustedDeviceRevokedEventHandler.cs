using ID.Application.AppAbs.EventBuses;
using ID.Application.Events.Users.TrustedDevices.Utils;
using ID.Domain.Entities.TrustedDevices.Events;
using ID.Domain.Repos;
using ID.GlobalSettings.Errors;
using LoggingHelpers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ID.Application.Events.Users.TrustedDevices;

internal class TrustedDeviceRevokedEventHandler(
    IIdentityTrustedDeviceRepo _repo,
    ITrustedDeviceBus _bus,
    ILogger<TrustedDeviceRevokedEventHandler> _logger)
    : INotificationHandler<TrustedDeviceRevokedDomainEvent>
{
    public async Task Handle(TrustedDeviceRevokedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {

            var deviceId = notification.TrustedDeviceId;
            var userId = notification.UserId;

            var deviceResult = await TrustedDeviceFinder.FindWithUserAndTeamAsync(deviceId, userId, _repo);
            if (!deviceResult.Succeeded)
            {
                _logger.LogError(new EventId(IdErrorEvents.Listeners.TrustedDeviceAdded), "{msg}", deviceResult.Info);
                return;
            }
            var device = deviceResult.Value!; //success is non-null
            var user = device.User; //success is non-null
            if (user is null)
            {
                _logger.LogError(new EventId(IdErrorEvents.Listeners.TrustedDeviceAdded), "{msg}", IDMsgs.Error.TrustedDevices.USER_NOT_FOUND(device));
                return;
            }

            var team = user.Team; //success is non-null
            if (team is null)
            {
                _logger.LogError(new EventId(IdErrorEvents.Listeners.TrustedDeviceAdded), "{msg}", IDMsgs.Error.TrustedDevices.TEAM_NOT_FOUND(device, user));
                return;
            }

            await _bus.PublishDeviceRevokedEventAsync(device, user, team, cancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogException(ex, IdErrorEvents.Listeners.TrustedDeviceRevoked);
        }
    }

}//Cls