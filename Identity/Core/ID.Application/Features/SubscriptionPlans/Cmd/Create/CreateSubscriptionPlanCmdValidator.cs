
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.SubscriptionPlans.Cmd.Create;
public class CreateSubscriptionPlanCmdValidator : AMntcMinimumValidator<CreateSubscriptionPlanCmd>
{
    public CreateSubscriptionPlanCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

    }
}//Cls

