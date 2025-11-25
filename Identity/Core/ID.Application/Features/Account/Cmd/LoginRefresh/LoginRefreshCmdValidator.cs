namespace ID.Application.Features.Account.Cmd.LoginRefresh;
public class LoginRefreshCmdValidator : AbstractValidator<LoginRefreshCmd>
{
    public LoginRefreshCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.RefreshToken)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
        });

    }
}

