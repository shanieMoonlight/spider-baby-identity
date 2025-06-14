using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.OAuth.Google.Features.SignIn.GoogleCookieSignIn;
public record GoogleCookieSignInCmd(GoogleCookieSignInDto Dto) : AIdCommand<CookieSignInResultData>;



