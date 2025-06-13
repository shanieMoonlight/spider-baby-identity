
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Cmd.DeleteMntcMember;
public class DeleteMntcMemberCmdValidator : AMntcOnlyValidator<DeleteMntcMemberCmd>
{
    public DeleteMntcMemberCmdValidator() : base()
    {
        RuleFor(p => p.UserId)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);


    }

}//Cls

