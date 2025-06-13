using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppComplete;
public record TwoFactorAuthAppCompleteRegCmd(TwoFactorAuthAppCompleteRegDto Dto) : AIdUserAwareCommand<AppUser, AppUserDto>;



