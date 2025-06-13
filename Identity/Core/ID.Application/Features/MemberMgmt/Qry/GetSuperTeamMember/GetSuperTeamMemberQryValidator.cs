
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Qry.GetSuperTeamMember;

//Customers can view ther own team with GetMyTeamMemberQry
public class GetSuperTeamMemberQryValidator : ASuperMinimumValidator<GetSuperTeamMemberQry>
{
    public GetSuperTeamMemberQryValidator()
    {
        RuleFor(p => p.MemberId)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
    }

}//Cls

