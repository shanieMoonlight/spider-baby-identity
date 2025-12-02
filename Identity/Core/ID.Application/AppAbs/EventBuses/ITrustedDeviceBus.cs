using ID.Domain.Entities.Teams;

namespace ID.Application.AppAbs.EventBuses;

/// <summary>
/// For firing off email confirmation events
/// </summary>
public interface ITrustedDeviceBus
{
    /// <summary>
    /// Publishes an integration event indicating that a trusted device was added for a user.
    /// </summary>
    /// <param name="device">The trusted device that was added.</param>
    /// <param name="user">The user who added the device.</param>
    /// <param name="team">The team associated with the user (if any).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while publishing the event.</param>
    /// <returns>A <see cref="Task"/> that completes when the event has been published.</returns>
    Task PublishDeviceAddedEventAsync( TrustedDevice device, AppUser user, Team team,   CancellationToken cancellationToken);


    /// <summary>
    /// Publishes an integration event indicating that a trusted device was added for a user.
    /// </summary>
    /// <param name="device">The trusted device that was added.</param>
    /// <param name="user">The user who added the device.</param>
    /// <param name="team">The team associated with the user (if any).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while publishing the event.</param>
    /// <returns>A <see cref="Task"/> that completes when the event has been published.</returns>
    Task PublishDeviceExpiredEventAsync(TrustedDevice device, AppUser user, Team team, CancellationToken cancellationToken);


    /// <summary>
    /// Publishes an integration event indicating that a trusted device was revoked.
    /// </summary>
    /// <param name="device">The trusted device that was revoked.</param>
    /// <param name="user">The user that revoked the device.</param>
    /// <param name="team">The team associated with the user (if any).</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while publishing the event.</param>
    /// <returns>A <see cref="Task"/> that completes when the event has been published.</returns>
    Task PublishDeviceRevokedEventAsync( TrustedDevice device, AppUser user, Team team,   CancellationToken cancellationToken);
}