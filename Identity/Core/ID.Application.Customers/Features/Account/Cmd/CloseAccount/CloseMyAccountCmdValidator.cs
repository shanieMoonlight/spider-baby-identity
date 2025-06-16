using FluentValidation;
using ID.Application.Customers.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Customers.Features.Account.Cmd.CloseAccount;
public class CloseMyAccountCmdValidator : ACustomerLeaderValidator<CloseMyAccountCmd> {
    public CloseMyAccountCmdValidator()
    {

        RuleFor(p => p.TeamId)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

}

