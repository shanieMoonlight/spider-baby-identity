using ClArch.ValueObjects;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags.Events;
using ID.Domain.Entities.Common;
using MassTransit;

namespace ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
public class FeatureFlag : IdDomainEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public ICollection<SubscriptionPlan> SubscriptionPlans { get; private set; } = [];
    public ICollection<SubscriptionPlanFeature> SubscriptionPlanFeatures { get; private set; } = [];

    //------------------------//

    #region EfCoreCtor
#pragma warning disable CS8618
    private FeatureFlag() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    private FeatureFlag(
          Name name,
          Description description)
      : base(NewId.NextSequentialGuid())
    {
        Name = name.Value;
        Description = description.Value;
        RaiseDomainEvent(new FeatureFlagCreatedDomainEvent(Id, this));
    }

    //- - - - - - - - - - - - - - - - - - - - -//

    public static FeatureFlag Create(
        Name name,
        Description description)
    {
        var feature = new FeatureFlag(
                name,
                description);
        return feature;
    }

    //- - - - - - - - - - - - - - - - - - - - -//

    public FeatureFlag Update(
        Name name,
        Description description)
    {
        Name = name.Value;
        Description = description.Value;

        RaiseDomainEvent(new FeatureFlagUpdatedDomainEvent(Id, this));
        return this;
    }

    //------------------------//

    #region EquasAndHashCode
    public override bool Equals(object? obj) =>
        obj is FeatureFlag that
        &&
        (Name.Equals(that.Name, StringComparison.CurrentCultureIgnoreCase) || Id == that.Id);

    //- - - - - - - - - - - - - - - - - - - - - //   

    public override int GetHashCode() =>
        HashCode.Combine(Name.ToLower());

    #endregion

    //------------------------// 

}//Cls
