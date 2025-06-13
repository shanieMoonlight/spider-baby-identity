using ClArch.ValueObjects;
using ID.Domain.Entities.Common;
using ID.Domain.Entities.SubscriptionPlans.Events;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams;
using MassTransit;
using System.ComponentModel.DataAnnotations;

namespace ID.Domain.Entities.SubscriptionPlans;

public class SubscriptionPlan : IdDomainEntity
{
    [MaxLength(100)]
    public string Name { get; private set; }

    [MaxLength(1000)]
    public string Description { get; private set; }

    public SubscriptionRenewalTypes RenewalType { get; private set; }

    /// <summary>
    /// Maximum device this user can use - 0 means unlimited
    /// </summary>
    public int DeviceLimit { get; private set; } = 0;

    /// <summary>
    /// How many months will the Sub be free for
    /// </summary>
    public int TrialMonths { get; private set; } = 0;

    public double Price { get; private set; } = 0;

    //Fks
    private readonly HashSet<FeatureFlag> _featureFlags = [];
    public IReadOnlyCollection<FeatureFlag> FeatureFlags 
        => _featureFlags.ToList().AsReadOnly();

    public ICollection<TeamSubscription> Subscriptions { get; } = [];
    public ICollection<SubscriptionPlanFeature> SubscriptionPlanFeatures { get; private set; } = [];

    //------------------------//

    #region EfCoreCtor
#pragma warning disable CS8618 
    private SubscriptionPlan() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    private SubscriptionPlan(
        Name name,
        Description description,
        Price price,
        SubscriptionRenewalTypes renewalType,
        TrialMonths? trialMonths,
        DeviceLimit? deviceLimit)
        : base(NewId.NextSequentialGuid())
    {
        Name = name.Value;
        Description = description.Value;
        RenewalType = renewalType;
        TrialMonths = trialMonths?.Value ?? 0;
        DeviceLimit = deviceLimit?.Value ?? 0;
        Price = price.Value;
    }

    //------------------------//

    public static SubscriptionPlan Create(
        Name name,
        Description description,
        Price price,
        SubscriptionRenewalTypes renewalType,
        TrialMonths? trialMonths,
        DeviceLimit? deviceLimit) =>
        new(name,
            description,
            price,
            renewalType,
            trialMonths,
            deviceLimit);

    //- - - - - - - - - - - - //  

    public SubscriptionPlan Update(
        Name name,
        Description description,
        Price price,
        SubscriptionRenewalTypes renewalType,
        TrialMonths? trialMonths,
        DeviceLimit? deviceLimit)
    {
        Name = name.Value;
        Description = description.Value;

        Price = price.Value;

        RenewalType = renewalType;

        TrialMonths = trialMonths?.Value ?? 0;
        DeviceLimit = deviceLimit?.Value ?? 0;
        return this;
    }


    //------------------------//

    public void AddFeatureFlags(IEnumerable<FeatureFlag> flags)
    {
        foreach (FeatureFlag flag in flags)
            AddFeatureFlag(flag);
    }

    //- - - - - - - - - - - - //  

    public void AddFeatureFlag(FeatureFlag flag)
    {
        var succeeded = _featureFlags.Add(flag);
        if (succeeded)
            RaiseDomainEvent(new SubscriptionPlanFeatureAddedDomainEvent(Id, this, flag));
        //else Already Added
    }

    ///- - - - - - - - - - - - //  

    public void RemoveFeatureFlags(IEnumerable<FeatureFlag> flags)
    {
        foreach (FeatureFlag flag in flags)
            RemoveFeatureFlag(flag);
    }

    ///- - - - - - - - - - - - //  

    public void RemoveFeatureFlag(FeatureFlag flag)
    {
        var succeeded = _featureFlags.Remove(flag);
        if (succeeded)
            RaiseDomainEvent(new SubscriptionPlanFeatureRemovedDomainEvent(Id, this, flag));
        //else Already Deleted
    }

    //------------------------//

}//Cls