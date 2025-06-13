
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.PhoneConfirmation.Features.Account.ConfirmPhone;
public class ConfirmPhoneCmdValidator : IsAuthenticatedValidator<ConfirmPhoneCmd>
{
    public ConfirmPhoneCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.ConfirmationToken)
                .NotEmpty()
                        .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        });

    }
}

