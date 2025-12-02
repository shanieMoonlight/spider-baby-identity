using ID.Application.AppAbs.EventBuses;
using ID.Domain.Entities.Teams;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.TrustedDevices;

namespace ID.Application.AppImps.EventBuses;
/// <summary>
/// Publishes integration events related to trusted devices.
/// </summary>
/// <param name="_bus">The event bus used to publish integration events.</param>
internal class TrustedDeviceBus(IEventBus _bus) : ITrustedDeviceBus
{
    /// <summary>
    /// Publishes an integration event indicating that a trusted device was added for a user.
    /// </summary>
    /// <param name="device">The trusted device that was added.</param>
    /// <param name="user">The user who added the device.</param>
    /// <param name="team">The team associated with the user (if any).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while publishing the event.</param>
    /// <returns>A <see cref="Task"/> that completes when the event has been published.</returns>
    public async Task PublishDeviceAddedEventAsync(TrustedDevice device, AppUser user, Team team, CancellationToken cancellationToken)
    {
        var addedEvent = new TrustedDeviceAddedIntegrationEvent(device, user, team);
        await _bus.PublishAsync(addedEvent, cancellationToken);
    }

    //------------------------//

    /// <summary>
    /// Publishes an integration event indicating that a trusted device has expired.
    /// </summary>
    /// <param name="device">The trusted device that expired.</param>
    /// <param name="user">The user that owned the device.</param>
    /// <param name="team">The team associated with the user (if any).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while publishing the event.</param>
    /// <returns>A <see cref="Task"/> that completes when the event has been published.</returns>
    public async Task PublishDeviceExpiredEventAsync(TrustedDevice device, AppUser user, Team team, CancellationToken cancellationToken)
    {
        var expiredEvent = new TrustedDeviceExpiredIntegrationEvent(device, user, team);
        await _bus.PublishAsync(expiredEvent, cancellationToken);
    }

    //------------------------//

    /// <summary>
    /// Publishes an integration event indicating that a trusted device was revoked.
    /// </summary>
    /// <param name="device">The trusted device that was revoked.</param>
    /// <param name="user">The user that revoked the device.</param>
    /// <param name="team">The team associated with the user (if any).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while publishing the event.</param>
    /// <returns>A <see cref="Task"/> that completes when the event has been published.</returns>
    public async Task PublishDeviceRevokedEventAsync(TrustedDevice device, AppUser user, Team team, CancellationToken cancellationToken)
    {
        var revokedEvent = new TrustedDeviceRevokedIntegrationEvent(device, user, team);
        await _bus.PublishAsync(revokedEvent, cancellationToken);
    }
}//Cls
