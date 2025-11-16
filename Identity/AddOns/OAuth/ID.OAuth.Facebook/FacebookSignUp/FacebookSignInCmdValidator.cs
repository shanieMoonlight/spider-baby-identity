using FluentValidation;
using ID.Domain.Utility.Messages;

namespace ID.OAuth.Facebook.FacebookSignUp;

/// <summary>
/// Validator for Facebook sign-in commands.
/// Validates only the essential fields - identity data is verified server-side.
/// </summary>
public class FacebookSignInCmdValidator : AbstractValidator<FacebookSignInCmd>
{
    public FacebookSignInCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
            .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.FacebookAccessToken)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        });
    }
}
