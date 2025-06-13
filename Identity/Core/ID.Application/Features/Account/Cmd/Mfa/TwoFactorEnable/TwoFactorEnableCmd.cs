using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorEnable;
public record TwoFactorEnableCmd() : AIdUserAwareCommand<AppUser, AppUserDto>;



