using ID.Application.AppAbs.ApplicationServices;
using ID.Application.Events.Users.TrustedDevices.Utils;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.TrustedDevices.Events;
using ID.Domain.Repos;
using ID.GlobalSettings.Errors;
using LoggingHelpers;
using MediatR;
using Microsoft.Extensions.Logging;


namespace ID.Application.Events.Users.TrustedDevices;
internal class TrustedDeviceAddedEventHandler(IIdentityTrustedDeviceRepo _repo, ILogger<TrustedDeviceAddedEventHandler> _logger)
    : INotificationHandler<TrustedDeviceAddedDomainEvent>
{

    public async Task Handle(TrustedDeviceAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {

            var deviceId = notification.TrustedDeviceId;
            var userId = notification.UserId;

            var deviceResult = await TrustedDeviceFinder.FindWithUserAsync(deviceId, userId, _repo);
            if (!deviceResult.Succeeded)
            {
                _logger.LogError(new EventId(IdErrorEvents.Listeners.TrustedDeviceAdded), "{msg}", deviceResult.Info);
                return;
            }


            //Do something, e.g., send a confirmation email 
        }
        catch (Exception ex)
        {
            _logger.LogException(ex, IdErrorEvents.Listeners.TrustedDeviceAdded);
        }
    }

}//Cls
