using ID.Application.Dtos.Account.Cookies;
using ID.Application.Mediatr.Cqrslmps.Commands;

namespace ID.OAuth.Amazon.Features.SignIn.AmazonCookieSignIn;

public record AmazonCookieSignInCmd(AmazonCookieSignInDto Dto) : AIdCommand<CookieSignInResultData>;
