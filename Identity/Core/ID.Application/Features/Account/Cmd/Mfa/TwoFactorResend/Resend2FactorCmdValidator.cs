
using FluentValidation;
using StringHelpers;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorResend;
public class Resend2FactorCmdValidator : IsAuthenticatedValidator<Resend2FactorCmd>
{
    public Resend2FactorCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto is not null, () =>
        {
            RuleFor(p => p.Dto)
                .NotNull()
                .Must(d => !d.Email.IsNullOrWhiteSpace() || !d.Username.IsNullOrWhiteSpace() || d.UserId is not null)
                        .WithMessage("You must supply at least one of [Username, UserId or Email]");

        });

    }
}

