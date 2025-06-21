using ID.Application.Dtos.Account.Cookies;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.Application.Features.Account.Cmd.Cookies.SignIn;
public record CookieSignInCmd(CookieSignInDto Dto) : AIdCommand<CookieSignInResultData>;



