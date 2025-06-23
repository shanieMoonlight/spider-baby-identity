
using FluentValidation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;
public class Resend2FactorCmdValidator : AbstractValidator<Resend2FactorCmd>
{
    public Resend2FactorCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto is not null, () =>
        {
            RuleFor(p => p.Dto.Token)
                .NotEmpty()
                       .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        });

    }
}

