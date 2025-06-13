
using FluentValidation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.ConfirmEmail;
public class ConfirmEmailCmdValidator : AbstractValidator<ConfirmEmailCmd>
{
    public ConfirmEmailCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.UserId)
                .NotEmpty()
                        .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p.Dto.ConfirmationToken)
                .NotEmpty()
                        .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        });

    }
}

