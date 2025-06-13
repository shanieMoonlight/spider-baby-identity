
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.SubscriptionPlans.Qry.GetById;
public class GetSubscriptionPlanByIdQryValidator : AMntcMinimumValidator<GetSubscriptionPlanByIdQry>
{
    public GetSubscriptionPlanByIdQryValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }
}//Cls


