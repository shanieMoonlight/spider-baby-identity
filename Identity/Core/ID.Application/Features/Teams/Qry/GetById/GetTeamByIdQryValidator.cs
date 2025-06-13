
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.Teams.Qry.GetById;
public class GetTeamByIdQryValidator : AMntcMinimumValidator<GetTeamByIdQry>
{
    public GetTeamByIdQryValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);
    }

}//Cls


