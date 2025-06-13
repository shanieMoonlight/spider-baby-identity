using ClArch.ValueObjects;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;

namespace ID.Application.Features.SubscriptionPlans;

public static class SubscriptionPlanMappings
{
    //------------------------------------//

    public static SubscriptionPlan Update(this SubscriptionPlan model, SubscriptionPlanDto dto)
    {

        model.Update(
            Name.Create(dto.Name),
            Description.Create(dto.Description),
            Price.Create(dto.Price),
            dto.RenewalType,
            TrialMonths.Create(dto.TrialMonths),
            DeviceLimit.Create(dto.DeviceLimit)
        );

        return model;
    }

    //------------------------------------//

    public static SubscriptionPlan ToModel(this SubscriptionPlanDto dto) =>
        SubscriptionPlan.Create(
            Name.Create(dto.Name),
            Description.Create(dto.Description),
            Price.Create(dto.Price),
            dto.RenewalType,
            TrialMonths.Create(dto.TrialMonths),
            DeviceLimit.Create(dto.DeviceLimit)
        );

    //------------------------------------//

    public static SubscriptionPlanDto ToDto(this SubscriptionPlan mdl) =>
        new(mdl);

    //------------------------------------//

}//Cls


