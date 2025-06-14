using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Models;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorVerify;
public record Verify2FactorCmd(Verify2FactorDto Dto) : AIdCommand<JwtPackage>;



