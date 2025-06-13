
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Cmd.UpdateLeaderMntc;
public class UpdateTeamLeaderCmdValidator : AMntcMinimumValidator<UpdateTeamLeaderCmd>
{
    public UpdateTeamLeaderCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);


        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.NewLeaderId)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
        });
    }

}//Cls


