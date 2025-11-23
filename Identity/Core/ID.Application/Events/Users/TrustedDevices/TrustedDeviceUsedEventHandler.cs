using ID.Domain.Entities.TrustedDevices.Events;
using ID.Domain.Repos;
using ID.Domain.Repos.Specs.TrustedDevices;
using ID.GlobalSettings.Errors;
using LoggingHelpers;
using MediatR;
using Microsoft.Extensions.Logging;


namespace ID.Application.Events.Users.TrustedDevices;
internal class TrustedDeviceUsedEventHandler(IIdentityTrustedDeviceRepo _repo, ILogger<TrustedDeviceAddedEventHandler> _logger)
    : INotificationHandler<TrustedDeviceUsedDomainEvent>
{

    public async Task Handle(TrustedDeviceUsedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var deviceId = notification.TrustedDeviceId;

            var spec = TrustedDeviceByIdWithUserSpec.Create(deviceId);
            var device = await _repo.FirstOrDefaultAsync(spec, cancellationToken);

            if (device is null)
            {
                _logger.LogError(new EventId(IdErrorEvents.Listeners.TrustedDeviceAdded), "{msg}", IDMsgs.Error.NotFound<TrustedDevice>(deviceId));
                return;
            }

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
            _logger.LogException(ex, IdErrorEvents.Listeners.TrustedDeviceUsed);
        }
    }

}//Cls
