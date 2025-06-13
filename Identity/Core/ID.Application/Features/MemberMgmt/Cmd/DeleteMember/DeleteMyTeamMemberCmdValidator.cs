
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Cmd.DeleteMember;
public class DeleteMyTeamMemberCmdValidator : IsAuthenticatedValidator<DeleteMyTeamMemberCmd>
{
    public DeleteMyTeamMemberCmdValidator() : base()
    {
        RuleFor(p => p.UserId)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);


    }

}//Cls

