using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.EmailConfirmation;
using StringHelpers;
using ID.Application.AppAbs.ApplicationServices;
using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.AppImps;

/// <summary>
/// Handles email confirmation operations and publishes related events.
/// </summary>
internal class EmailConfirmationBus(IEmailConfirmationService<AppUser> _emailConfService, IEventBus bus) : IEmailConfirmationBus
{
    //------------------------------------//

    /// <summary>
    /// Generates an email confirmation token and publishes the appropriate event.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task GenerateTokenAndPublishEventAsync(AppUser user, Team team, CancellationToken cancellationToken)
    {
        if (user.EmailConfirmed)
            return;

        string safePwdResetTkn = await _emailConfService.GenerateSafeEmailConfirmationTokenAsync(team, user);

        if (user.PasswordHash.IsNullOrWhiteSpace())
            await SendRequiringPassword(user, team, safePwdResetTkn, cancellationToken);
        else
            await Send(user, team, safePwdResetTkn, cancellationToken);
    }

    //------------------------------------//

    /// <summary>
    /// Publishes an event indicating that email confirmation is required and the user needs to set a password.
    /// </summary>
    /// <param name="user">The user for whom the event is published.</param>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="safePwdResetTkn">The email confirmation token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task SendRequiringPassword(AppUser user, Team team, string safePwdResetTkn, CancellationToken cancellationToken) =>
        await bus.Publish(
               new EmailConfirmationRequiringPasswordIntegrationEvent(
                   user,
                   safePwdResetTkn,
                   team.TeamType == TeamType.Customer),
               cancellationToken);

    //------------------------------------//

    /// <summary>
    /// Publishes an event indicating that email confirmation is required.
    /// </summary>
    /// <param name="user">The user for whom the event is published.</param>
    /// <param name="team">The team to which the user belongs.</param>
    /// <param name="safePwdResetTkn">The email confirmation token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task Send(AppUser user, Team team, string safePwdResetTkn, CancellationToken cancellationToken) =>
        await bus.Publish(
                new EmailConfirmationIntegrationEvent(
                    user,
                    safePwdResetTkn,
                    team.TeamType == TeamType.Customer),
                cancellationToken);

    //------------------------------------//

}//Cls
