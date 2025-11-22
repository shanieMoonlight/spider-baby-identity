using ID.Application.AppAbs.ApplicationServices;
using ID.Application.Events.Users.TrustedDevices.Utils;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.Events;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.TrustedDevices.Events;
using ID.Domain.Repos;
using ID.Domain.Repos.Specs.TrustedDevices;
using ID.Domain.Utility.Messages;
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
            var device = await _repo.FirstOrDefaultAsync(spec);

            if (device is null)
            {
                _logger.LogError(new EventId(IdErrorEvents.Listeners.TrustedDeviceAdded), "{msg}", IDMsgs.Error.NotFound<TrustedDevice>(deviceId));
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
