using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Customers.Features.Account.Cmd.CloseAccount;
public record CloseAccountCmd(Guid TeamId) : AIdUserAndTeamAwareCommand<AppUser>;



