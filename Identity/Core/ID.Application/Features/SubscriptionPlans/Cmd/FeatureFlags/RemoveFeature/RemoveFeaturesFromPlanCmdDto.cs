namespace ID.Application.Features.SubscriptionPlans.Cmd.FeatureFlags.RemoveFeature;

//------------------------------------//

public record RemoveFeaturesFromSubscriptionPlanDto(Guid SubscriptionPlanId, IEnumerable<Guid> FeatureIds);

//- - - - - - - - - - - - - - - - - - //

public record RemoveFeatureFromSubscriptionPlanDto(Guid SubscriptionPlanId, Guid FeatureId)
{
    public RemoveFeaturesFromSubscriptionPlanDto ToMultipleFeaturesDto() =>
        new(SubscriptionPlanId, [FeatureId]);
}

//------------------------------------//




