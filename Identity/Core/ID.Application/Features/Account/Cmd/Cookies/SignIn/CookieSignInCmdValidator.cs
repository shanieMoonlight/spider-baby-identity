
using FluentValidation;
using StringHelpers;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Cookies.SignIn;
public class CookieSignInCmdValidator : AbstractValidator<CookieSignInCmd>
{
    public CookieSignInCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto)
                .NotNull()
                .Must(d => !d.Email.IsNullOrWhiteSpace() || !d.Username.IsNullOrWhiteSpace() || d.UserId is not null)
                        .WithMessage("You must supply at least one of [Username, UserId or Email]");


            RuleFor(p => p.Dto.Password)
              .NotEmpty()
                      .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
        });
    }
}

