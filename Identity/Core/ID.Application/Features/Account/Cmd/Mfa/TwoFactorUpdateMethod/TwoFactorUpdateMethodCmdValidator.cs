
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Mfa.TwoFactorUpdateMethod;
public class TwoFactorUpdateMethodCmdValidator : IsAuthenticatedValidator<TwoFactorUpdateMethodCmd>
{
    public TwoFactorUpdateMethodCmdValidator()
    {
        RuleFor(p => p.Provider)
            .NotNull() // Value might be zero
                    .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

    }
}

