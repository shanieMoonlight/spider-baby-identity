
using FluentValidation;
using ID.Domain.Utility.Messages;

namespace ID.OAuth.Google.Features.SignIn.GoogleSignIn;
public class GoogleSignUpCmdValidator : AbstractValidator<GoogleSignInCmd>
{
    public GoogleSignUpCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.IdToken)
                .NotEmpty()
                        .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        });

    }
}

