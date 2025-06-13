
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.MemberMgmt.Qry.GetTeamMemberQry;

//Customers can view ther own team with GetMyTeamMemberQry
public class GetTeamMemberQryValidator : AMntcMinimumValidator<GetTeamMemberQry>
{
    public GetTeamMemberQryValidator()
    {
        RuleFor(p => p.MemberId)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

        RuleFor(p => p.TeamId)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
    }

}//Cls

