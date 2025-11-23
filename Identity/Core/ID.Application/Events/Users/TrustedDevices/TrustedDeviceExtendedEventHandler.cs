using ID.Application.Events.Users.TrustedDevices.Utils;
using ID.Domain.Entities.TrustedDevices.Events;
using ID.Domain.Repos;
using ID.GlobalSettings.Errors;
using LoggingHelpers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ID.Application.Events.Users.TrustedDevices;

internal class TrustedDeviceExtendedEventHandler(IIdentityTrustedDeviceRepo _repo, ILogger<TrustedDeviceExtendedEventHandler> _logger)
    : INotificationHandler<TrustedDeviceExtendedDomainEvent>
{
    public async Task Handle(TrustedDeviceExtendedDomainEvent notification, CancellationToken cancellationToken)
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


            //Do something, e.g., send a confirmation email 
        }
        catch (Exception ex)
        {
            _logger.LogException(ex, IdErrorEvents.Listeners.TrustedDeviceExtended);
        }
    }

}//Cls
