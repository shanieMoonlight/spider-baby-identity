
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Qry.GetMntcTeamMember;

//Customers can view ther own team with GetMyTeamMemberQry
public class GetMntcTeamMemberQryValidator : AMntcMinimumValidator<GetMntcTeamMemberQry>
{
    public GetMntcTeamMemberQryValidator()
    {
        RuleFor(p => p.MemberId)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
    }

}//Cls

