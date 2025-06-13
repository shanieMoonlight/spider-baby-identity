
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Teams.Cmd.Delete;
public class DeleteCustomerTeamCmdValidator : AMntcMinimumValidator<DeleteCustomerTeamCmd>
{
    public DeleteCustomerTeamCmdValidator()
    {
        RuleFor(p => p.Id)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }
}//Cls


