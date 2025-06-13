
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Qry.GetMyTeamMember;
public class GetMyTeamMemberQryValidator : IsAuthenticatedValidator<GetMyTeamMemberQry>
{
    public GetMyTeamMemberQryValidator()
    {
        RuleFor(p => p.MemberId)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
    }

}//Cls

