
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthAppEmailComplete;
public class TwoFactorAuthAppEmailCompleteCmdValidator : IsAuthenticatedValidator<TwoFactorAuthAppEmailCompleteCmd>
{
    public TwoFactorAuthAppEmailCompleteCmdValidator()
    {
        RuleFor(p => p.TwoFactorCode)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

    }
}

