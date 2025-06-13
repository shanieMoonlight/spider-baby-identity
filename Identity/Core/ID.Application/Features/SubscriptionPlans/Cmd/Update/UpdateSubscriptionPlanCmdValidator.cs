
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.SubscriptionPlans.Cmd.Update;
public class UpdateSubscriptionPlanCmdValidator : AMntcMinimumValidator<UpdateSubscriptionPlanCmd>
{
    public UpdateSubscriptionPlanCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }
}//Cls


