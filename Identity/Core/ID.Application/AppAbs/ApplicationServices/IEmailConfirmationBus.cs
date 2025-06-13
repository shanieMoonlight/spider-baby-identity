using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.Application.AppAbs.ApplicationServices;

/// <summary>
/// For firing off email confirmation events
/// </summary>
public interface IEmailConfirmationBus
{
    /// <summary>
    /// Generates an email confirmation token and publishes the appropriate event.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task GenerateTokenAndPublishEventAsync(AppUser user, Team team, CancellationToken cancellationToken);
}