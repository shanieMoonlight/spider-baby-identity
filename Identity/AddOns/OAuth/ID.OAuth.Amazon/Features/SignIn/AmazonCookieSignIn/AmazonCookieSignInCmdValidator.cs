using FluentValidation;
using ID.Domain.Utility.Messages;

namespace ID.OAuth.Amazon.Features.SignIn.AmazonCookieSignIn;

public class AmazonCookieSignInCmdValidator : AbstractValidator<AmazonCookieSignInCmd>
{
    public AmazonCookieSignInCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
            .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.AuthToken)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
        });
    }
}
