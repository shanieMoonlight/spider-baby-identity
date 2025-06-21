using ID.Application.AppAbs.SignIn;
using ID.Application.Dtos.Account.Cookies;
using ID.Domain.Models;

namespace ID.Application.Features.Account.Cmd.Cookies.SignIn;
internal static class CookieSignInResultDataExtensions
{
    public static CookieSignInResultData ToCookieSignInResultData(this MyIdSignInResult signInResult) => new()
    {
        Succeeded = signInResult.Succeeded,
        EmailConfirmationRequired = signInResult.EmailConfirmationRequired,
        TwoFactorRequired = signInResult.TwoFactorRequired,
        TwoFactorProvider = signInResult.MfaResultData?.TwoFactorProvider ?? signInResult.User?.TwoFactorProvider ?? TwoFactorProvider.Email,
        Message = signInResult.Message,
        ExtraInfo = signInResult.MfaResultData?.ExtraInfo,
    };


}
