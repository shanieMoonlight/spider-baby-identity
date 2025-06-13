using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.Teams;

namespace ID.PhoneConfirmation.Events.Integration.Bus;
public interface IPhoneConfirmationBus
{
    Task GenerateTokenAndPublishEventAsync(AppUser user, Team team, CancellationToken cancellationToken);

}