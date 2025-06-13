
using FluentValidation;
using ID.Application.Mediatr.Validation;
using ID.Domain.Utility.Messages;

namespace ID.Application.Features.SubscriptionPlans.Cmd.FeatureFlags.RemoveFeature;
public class RemoveFeaturesFromSubscriptionPlanCmdValidator : AMntcMinimumValidator<RemoveFeaturesFromSubscriptionPlanCmd>
{
    public RemoveFeaturesFromSubscriptionPlanCmdValidator()
    {
        RuleFor(p => p.Dto)
            .NotEmpty()
                .WithMessage(IDMsgs.Error.NO_DATA_SUPPLIED);

        When(p => p.Dto != null, () =>
        {
            RuleFor(p => p.Dto.FeatureIds)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));

            RuleFor(p => p.Dto.SubscriptionPlanId)
                .NotEmpty()
                .WithMessage(IDMsgs.Error.IsRequired("{PropertyName}"));
        });

    }
}//Cls

