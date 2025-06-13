using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;

namespace ID.Domain.Entities.SubscriptionPlans;

/// <summary>
/// Many-To-Many (Joins SubscriptionPlan to FeatureFlag)
/// </summary>
public class SubscriptionPlanFeature
{
    public Guid SubscriptionPlanId { get; private set; }
    public SubscriptionPlan? SubscriptionPlan { get; private set; }

    public Guid FeatureFlagId { get; private set; }
    public FeatureFlag? FeatureFlag { get; private set; }

    //------------------------//

    #region EfCoreCtor
    private SubscriptionPlanFeature() { }
    #endregion

    //- - - - - - - - - - - - - - - - - - - - -//

    private SubscriptionPlanFeature(SubscriptionPlan subscriptionPlan, FeatureFlag featureFlag)
    {
        SubscriptionPlan = subscriptionPlan;
        SubscriptionPlanId = subscriptionPlan.Id;
        FeatureFlag = featureFlag;
        FeatureFlagId = featureFlag.Id;
    }

    //------------------------//

    public static SubscriptionPlanFeature Create(SubscriptionPlan subscriptionPlan, FeatureFlag featureFlag) =>
        new(subscriptionPlan, featureFlag);

    //------------------------//

    #region EquasAndHashCode

    public override bool Equals(object? thatObj) =>
        thatObj is SubscriptionPlanFeature that
            && SubscriptionPlanId == that.SubscriptionPlanId
            && FeatureFlagId == that.FeatureFlagId;

    //- - - - - - - - - - - - - - - - - - - - - //   

    public override int GetHashCode() =>
        HashCode.Combine(SubscriptionPlanId, FeatureFlagId);

    #endregion

    //------------------------// 

}
