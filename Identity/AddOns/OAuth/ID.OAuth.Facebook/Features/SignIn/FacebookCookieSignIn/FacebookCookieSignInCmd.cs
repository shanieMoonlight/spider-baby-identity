using ID.Application.Dtos.Account.Cookies;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.OAuth.Facebook.Features.SignIn.FacebookCookieSignIn;
public record FacebookCookieSignInCmd(FacebookCookieSignInDto Dto) : AIdCommand<CookieSignInResultData>;



