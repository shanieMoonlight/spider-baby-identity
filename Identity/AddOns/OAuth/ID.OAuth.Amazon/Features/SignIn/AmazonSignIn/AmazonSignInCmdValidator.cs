using FluentValidation;
using ID.Domain.Utility.Messages;

namespace ID.OAuth.Amazon.Features.SignIn.AmazonSignIn;

public class AmazonSignInCmdValidator : AbstractValidator<AmazonSignInCmd>
{
    public AmazonSignInCmdValidator()
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
