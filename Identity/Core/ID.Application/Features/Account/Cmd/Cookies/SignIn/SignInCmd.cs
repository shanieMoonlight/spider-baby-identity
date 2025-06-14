using ID.Application.AppAbs.SignIn;
using MediatR;

namespace ID.Application.Features.Account.Cmd.Cookies.SignIn;
public record SignInCmd(CookieSignInDto Dto) : IRequest<MyIdSignInResult>;



