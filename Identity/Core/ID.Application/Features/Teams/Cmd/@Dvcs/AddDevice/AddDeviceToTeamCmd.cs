using ID.Application.Features.Teams;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Teams.Cmd.Dvcs.AddDevice;
public record AddDeviceToTeamCmd(AddDeviceToTeamDto Dto) : AIdUserAndTeamAwareCommand<AppUser, SubscriptionDto>;
