
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Cmd.DeleteSuperMember;
public class DeleteSprMemberCmdValidator : ASuperMinimumValidator<DeleteSprMemberCmd>
{
    public DeleteSprMemberCmdValidator()
    {
        RuleFor(p => p.UserId)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);


    }

}//Cls

