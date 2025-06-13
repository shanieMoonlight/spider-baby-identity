
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Mfa.Reg.TwoFactorGoogleComplete;
public class MfaGoogleCompleteRegistrationCmdValidator : IsAuthenticatedValidator<MfaGoogleCompleteRegistrationCmd>
{
    public MfaGoogleCompleteRegistrationCmdValidator()
    {
        RuleFor(p => p.TwoFactorCode)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

    }
}

