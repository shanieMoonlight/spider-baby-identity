
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateMyTeamLeader;
public class UpdateMyTeamLeaderCmdValidator : IsAuthenticatedValidator<UpdateMyTeamLeaderCmd>
{
    public UpdateMyTeamLeaderCmdValidator()
    {
        RuleFor(p => p.NewLeaderId)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }

}//Cls

