using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Models;
using ID.Domain.Entities.AppUsers;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorVerify;
public record Verify2FactorCmd(Verify2FactorDto Dto) : AIdUserAndTeamAwareCommand<AppUser, JwtPackage>;



