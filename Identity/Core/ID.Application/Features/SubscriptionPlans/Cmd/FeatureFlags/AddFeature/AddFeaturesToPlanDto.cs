namespace ID.Application.Features.SubscriptionPlans.Cmd.FeatureFlags.AddFeature;

public record AddFeaturesToPlanDto(Guid SubscriptionPlanId, IEnumerable<Guid> FeatureIds);


public record AddFeatureToPlanDto(Guid SubscriptionPlanId, Guid FeatureId)
{
    public AddFeaturesToPlanDto ToMultipleFeaturesDto() =>
        new(SubscriptionPlanId, [FeatureId]);
}



