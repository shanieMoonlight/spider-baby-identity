
using FluentValidation;
using ID.Application.Features.Account.Cmd.LoginRefresh;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Account.Cmd.Refresh;
public class LoginRefreshCmdValidator : AbstractValidator<LoginRefreshCmd>
{
    public LoginRefreshCmdValidator()
    {
        RuleFor(p => p.RefreshToken)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }
}

