
using FluentValidation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Cookies.TwoFactorResendCookie;
public class Resend2FactorCookieCmdValidator : AbstractValidator<Resend2FactorCookieCmd>
{
    public Resend2FactorCookieCmdValidator()
    {
    }
}

