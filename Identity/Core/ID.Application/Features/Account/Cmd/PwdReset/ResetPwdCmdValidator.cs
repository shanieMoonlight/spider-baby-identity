
using FluentValidation;
using StringHelpers;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.PwdReset;
public class ResetPwdCmdValidator : AbstractValidator<ResetPwdCmd>
{
    public ResetPwdCmdValidator()
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

            RuleFor(p => p.Dto.ResetToken)
               .NotEmpty()
                       .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));


            RuleFor(p => p.Dto.NewPassword)
              .NotEmpty()
                      .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));


            RuleFor(p => p.Dto.ConfirmPassword)
              .NotEmpty()
                      .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p.Dto.ConfirmPassword)
            .Equal(p => p.Dto.NewPassword);

        });

    }
}

