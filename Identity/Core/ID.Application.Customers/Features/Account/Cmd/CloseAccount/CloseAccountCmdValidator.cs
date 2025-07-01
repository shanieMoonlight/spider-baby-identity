using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Customers.Features.Account.Cmd.CloseAccount;
public class CloseAccountCmdValidator : AMntcMinimumValidator<CloseAccountCmd> {
    public CloseAccountCmdValidator()
    {

        RuleFor(p => p.TeamId)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

}

