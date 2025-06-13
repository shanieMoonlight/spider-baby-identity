
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Teams.Qry.GetMntcMembersPage;

public class GetMntcMembersPageQryValidator : AMntcMinimumValidator<GetMntcMembersPageQry>
{

    public GetMntcMembersPageQryValidator()
    {
        RuleFor(p => p.PagedRequest)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
    }


}//Cls


