using ID.Application.Features.Account.Cmd.Cookies.SignIn;
using ID.Application.Mediatr.Cqrslmps.Commands;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Models;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorCookieVerify;
public record Verify2FactorCookieCmd(Verify2FactorCookieCmdDto Dto) : AIdUserAndTeamAwareCommand<AppUser>;



