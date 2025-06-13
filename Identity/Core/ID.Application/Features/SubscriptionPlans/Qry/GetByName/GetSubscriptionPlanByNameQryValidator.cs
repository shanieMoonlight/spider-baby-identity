
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.SubscriptionPlans.Qry.GetByName;
public class GetSubscriptionPlanByNameQryValidator : AMntcMinimumValidator<GetSubscriptionPlanByNameQry>
{
    public GetSubscriptionPlanByNameQryValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }
}//Cls


