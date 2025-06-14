using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Account.Cmd.Cookies.TwoFactorVerifyCookie;
public record Verify2FactorCookieCmd(Verify2FactorCookieDto Dto) : AIdCommand;



