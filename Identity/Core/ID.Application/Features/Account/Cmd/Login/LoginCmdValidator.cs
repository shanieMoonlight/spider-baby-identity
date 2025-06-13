
using FluentValidation;
using ID.Domain.Utility.Messages;
using StringHelpers;

namespace ID.Application.Features.Account.Cmd.Login;

/// <summary>
/// Validates login command requirements including user identifier and password.
/// </summary>
public class LoginCmdValidator : AbstractValidator<LoginCmd>
{
    public LoginCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                    .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto)
                .NotNull()
                .Must(d => !d.Email.IsNullOrWhiteSpace() || !d.Username.IsNullOrWhiteSpace() || d.UserId is not null)
                        .WithMessage(IDMsgs.Error.RequestData.MISSING_USER_IDENTIFIER_DATA);


            RuleFor(p => p.Dto.Password)
              .NotEmpty()
                      .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
        });
    }
}

