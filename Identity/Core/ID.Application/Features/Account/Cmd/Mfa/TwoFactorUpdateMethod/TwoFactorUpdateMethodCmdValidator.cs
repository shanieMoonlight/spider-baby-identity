
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorUpdateMethod;
public class TwoFactorUpdateMethodCmdValidator : IsAuthenticatedValidator<UpdateTwoFactorProviderCmd>
{
    public TwoFactorUpdateMethodCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);


        When(p => p.Dto != null, () =>
        {

            RuleFor(p => p.Dto.Provider)
                .NotEmpty()
                        .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
        });

    }
}

