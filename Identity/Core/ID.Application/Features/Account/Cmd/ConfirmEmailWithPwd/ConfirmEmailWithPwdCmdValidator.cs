
using FluentValidation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.ConfirmEmailWithPwd;
public class ConfirmEmailWithPwdCmdValidator : AbstractValidator<ConfirmEmailWithPwdCmd>
{
    public ConfirmEmailWithPwdCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.UserId)
                .NotEmpty()
                        .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p.Dto.ConfirmationToken)
                .NotEmpty()
                        .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p.Dto.ConfirmPassword)
                .NotEmpty()
                        .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p.Dto.Password)
                .NotEmpty()
                        .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));


            RuleFor(p => p.Dto.ConfirmPassword).Equal(p => p.Dto.Password);


        });

    }
}

