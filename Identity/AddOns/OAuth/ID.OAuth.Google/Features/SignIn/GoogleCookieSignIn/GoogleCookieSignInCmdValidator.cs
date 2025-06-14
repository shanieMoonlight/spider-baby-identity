
using FluentValidation;
using ID.Domain.Utility.Messages;

namespace ID.OAuth.Google.Features.SignIn.GoogleCookieSignIn;
public class GoogleSignUpCmdValidator : AbstractValidator<GoogleCookieSignInCmd>
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

