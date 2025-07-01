using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Customers.Features.Account.Cmd.CloseMyAccount;
public record CloseMyAccountCmd(Guid TeamId) : AIdUserAndTeamAwareCommand<AppUser>;



