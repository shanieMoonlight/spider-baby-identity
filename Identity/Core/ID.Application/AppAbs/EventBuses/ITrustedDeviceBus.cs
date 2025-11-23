using ID.Domain.Entities.Teams;

namespace ID.Application.AppAbs.EventBuses;

/// <summary>
/// For firing off email confirmation events
/// </summary>
public interface ITrustedDeviceBus
{
    /// <summary>
    /// Generates an email confirmation token and publishes the appropriate event.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishDeviceAddedEventAsync( TrustedDevice device, AppUser user, Team team,   CancellationToken cancellationToken);


    /// <summary>
    /// Generates an email confirmation token and publishes the appropriate event.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishDeviceRevokedEventAsync( TrustedDevice device, AppUser user, Team team,   CancellationToken cancellationToken);
}