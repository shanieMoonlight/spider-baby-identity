
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Teams.Qry.GetSprMembersPage;
public class GetSprMembersPageQryValidator : ASuperMinimumValidator<GetSprMembersPageQry>
{
    public GetSprMembersPageQryValidator()
    {
        RuleFor(p => p.PagedRequest)
        .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }
}//Cls


