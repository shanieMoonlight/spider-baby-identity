using ID.Application.Features.Common.Dtos.User;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Models;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorUpdateMethod;
public record TwoFactorUpdateMethodCmd(TwoFactorProvider? Provider) : AIdUserAndTeamAwareCommand<AppUser, AppUserDto>;



