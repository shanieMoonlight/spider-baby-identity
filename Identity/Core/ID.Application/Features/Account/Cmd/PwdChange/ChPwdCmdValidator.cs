
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;
using StringHelpers;
using ID.Application.Utility.Messages;


namespace ID.Application.Features.Account.Cmd.PwdChange;
public class ChPwdCmdValidator : IsAuthenticatedValidator<ChPwdCmd>
{
    public ChPwdCmdValidator()
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


            RuleFor(p => p.Dto.NewPassword)
              .NotEmpty()
                      .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));


            RuleFor(p => p.Dto.ConfirmPassword)
              .NotEmpty()
                      .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p.Dto.NewPassword).Equal(p => p.Dto.ConfirmPassword);


        });

    }

}

