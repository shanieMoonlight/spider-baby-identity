
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorAuthyComplete;
public class TwoFactorAuthyCompleteCmdValidator : IsAuthenticatedValidator<TwoFactorAuthyCompleteRegCmd>
{
    public TwoFactorAuthyCompleteCmdValidator()
    {
        RuleFor(p => p.TwoFactorCode)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

    }
}

