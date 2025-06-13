using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.PwdChange;
public record ChPwdCmd(ChPwdDto Dto) : AIdUserAwareCommand<AppUser>;



