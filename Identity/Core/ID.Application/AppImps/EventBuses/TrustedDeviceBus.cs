using ID.Application.AppAbs.EventBuses;
using ID.Domain.Entities.Teams;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.TrustedDevices;

namespace ID.Application.AppImps.EventBuses;
internal class TrustedDeviceBus(IEventBus _bus) : ITrustedDeviceBus
{
    public async Task PublishDeviceAddedEventAsync(TrustedDevice device, AppUser user, Team team, CancellationToken cancellationToken)
    {
        var addedEvent = new TrustedDeviceAddedIntegrationEvent(device, user, team);
        await _bus.PublishAsync(addedEvent, cancellationToken);
    }

    //------------------------//


    public async Task PublishDeviceRevokedEventAsync(TrustedDevice device, AppUser user, Team team, CancellationToken cancellationToken)
    {
        var revokedEvent = new TrustedDeviceRevokedIntegrationEvent(device, user, team);
        await _bus.PublishAsync(revokedEvent, cancellationToken);
    }

}//Cls
