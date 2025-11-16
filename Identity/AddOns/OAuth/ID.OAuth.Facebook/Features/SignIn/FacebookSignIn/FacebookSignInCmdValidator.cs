
using FluentValidation;
using ID.Domain.Utility.Messages;

namespace ID.OAuth.Facebook.Features.SignIn.FacebookSignIn;
public class FacebookSignUpCmdValidator : AbstractValidator<FacebookSignInCmd>
{
    public FacebookSignUpCmdValidator()
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

