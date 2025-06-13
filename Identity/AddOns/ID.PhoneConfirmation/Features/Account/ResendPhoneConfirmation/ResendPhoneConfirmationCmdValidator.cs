
using FluentValidation;
using StringHelpers;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.PhoneConfirmation.Features.Account.ResendPhoneConfirmation;
public class ResendPhoneConfirmationCmdValidator : IsAuthenticatedValidator<ResendPhoneConfirmationCmd>
{
    public ResendPhoneConfirmationCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto)
                .NotNull()
                .Must(d => !d.Email.IsNullOrWhiteSpace() || !d.Username.IsNullOrWhiteSpace() || d.UserId is not null)
                        .WithMessage("You must supply at least one of [Username, UserId or Email]");

        });

    }
}

