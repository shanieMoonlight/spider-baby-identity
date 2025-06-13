using ID.Application.AppAbs.TokenVerificationServices;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;
using ID.IntegrationEvents.Abstractions;
using ID.IntegrationEvents.Events.Account.PhoneConfirmation;

namespace ID.PhoneConfirmation.Events.Integration.Bus;
internal class PhoneConfirmationBus(
        IPhoneConfirmationService<AppUser> _phoneConfService,
        IEventBus bus) : IPhoneConfirmationBus
{
    public async Task GenerateTokenAndPublishEventAsync(AppUser user, Team team, CancellationToken cancellationToken)
    {
        if (user.PhoneNumberConfirmed)
            return;

        string safePwdResetTkn = await _phoneConfService.GeneratePhoneChangedConfirmationTokenAsync(team, user, user.PhoneNumber ?? "");

        await bus.Publish(
            new PhoneConfirmationIntegrationEvent(user, safePwdResetTkn, team.TeamType),
            cancellationToken
        );
    }

}//Cls
