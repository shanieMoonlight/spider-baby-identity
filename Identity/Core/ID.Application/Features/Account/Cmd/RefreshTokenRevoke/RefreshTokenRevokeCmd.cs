using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.RefreshTokenRevoke;
public record RefreshTokenRevokeCmd() : AIdUserAwareCommand<AppUser>;



