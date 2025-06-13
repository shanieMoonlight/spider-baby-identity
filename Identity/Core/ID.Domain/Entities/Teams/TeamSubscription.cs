using ClArch.ValueObjects;
using ID.Domain.Entities.Common;
using ID.Domain.Entities.Devices;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.Subscriptions;
using ID.Domain.Entities.SubscriptionPlans.Subscriptions.Events;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams.Events;
using ID.Domain.Utility.Exceptions;
using MassTransit;
using System.ComponentModel.DataAnnotations.Schema;

namespace ID.Domain.Entities.Teams;

public class TeamSubscription : IdDomainEntity
{
    public SubscriptionStatus SubscriptionStatus { get; private set; }
    public double Discount { get; set; }

    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public DateTime TrialStartDate { get; private set; }
    public DateTime TrialEndDate { get; private set; }

    public DateTime LastPaymentDate { get; private set; }
    public double LastPaymenAmount { get; private set; }

    /// <summary>
    /// Maximum device this user can use - 0 means unlimited
    /// </summary>
    public int DeviceLimit { get; private set; } = 0;

    private readonly HashSet<TeamDevice> _devices = [];
    /// <summary>
    /// Devices connected to this Subscription.
    /// <para></para>
    /// Will return empty list if no devices are attached.
    /// </summary>
    public IReadOnlyCollection<TeamDevice> Devices => _devices.ToList().AsReadOnly();

    public Guid TeamId { get; private set; }
    public Team? Team { get; set; }


    public Guid SubscriptionPlanId { get; private set; }
    public SubscriptionPlan? SubscriptionPlan { get; private set; }

    public SubscriptionRenewalTypes RenewalType { get; private init; } = SubscriptionRenewalTypes.Monthly;

    [NotMapped]
    public bool Expired { get => EndDate < DateTime.UtcNow; }

    //------------------------//

    #region EfCoreCtor
    private TeamSubscription() { }
    #endregion

    public TeamSubscription(SubscriptionPlan plan, Team team, Discount? discount)
        : base(NewId.NextSequentialGuid())
    {
        SubscriptionPlan = plan;
        SubscriptionPlanId = plan.Id;

        Team = team;
        TeamId = team.Id;

        Discount = discount?.Value ?? 0;

        StartDate = DateTime.Now;

        TrialStartDate = DateTime.Now;
        TrialEndDate = DateTime.Now.AddMonths(plan.TrialMonths);

        EndDate = TrialEndDate; //Will change when payments are received

        SubscriptionStatus = SubscriptionStatus.InActive;
        RenewalType = plan.RenewalType;

        DeviceLimit = plan.DeviceLimit;
    }

    //- - - - - - - - - - - - //

    internal static TeamSubscription Create(
        SubscriptionPlan plan,
        Team team,
        Discount? discount,
        bool alreadyPaid = false)
    {
        var sub = new TeamSubscription(plan, team, discount);
        if (alreadyPaid)
            return sub.ExtendEndDate();

        return sub;
    }

    //------------------------//

    private TeamSubscription ExtendEndDate()
    {
        EndDate = EndDate.Extend(RenewalType);
        return this;
    }

    //------------------------//

    public TeamSubscription RecordPayment()
    {
        LastPaymenAmount = SubscriptionPlan?.Price ?? -1; //-1 Means we didn't have the Plan avaiable. It's probably not necesary anyway
        LastPaymentDate = DateTime.Now;
        SubscriptionStatus = SubscriptionStatus.Active;
        ExtendEndDate();

        return this;
    }

    //------------------------//

    public TeamSubscription Deactivate()
    {
        if (EndDate > DateTime.Now)
            EndDate = DateTime.Now;

        //Make sure we don't raise the event twice
        if (SubscriptionStatus != SubscriptionStatus.InActive)
        {
            SubscriptionStatus = SubscriptionStatus.InActive;
            RaiseDomainEvent(new TeamSubscriptionDeactivatedDomainEvent(this));
        }

        return this;
    }

    //------------------------//

    public TeamDevice AddDevice(Name name, DescriptionNullable description, UniqueId uniqueId)
    {
        if (DeviceLimit > 0 && Devices.Count >= DeviceLimit)
            throw new DeviceLimitExceededException(this);

        TeamDevice dvc = TeamDevice.Create(this, name, description, uniqueId);

        var succeeded = _devices.Add(dvc);
        if (succeeded)
            RaiseDomainEvent(new SubscriptionDeviceAddedDomainEvent(Id, this, dvc));
        //else Already Added

        return dvc;
    }

    //- - - - - - - - - - - - //  

    public bool RemoveDevice(TeamDevice dvc)
    {
        var succeeded = _devices.Remove(dvc);
        if (succeeded)
            RaiseDomainEvent(new SubscriptionDeviceRemovedDomainEvent(Id, this, dvc));
        //else Already Deleted

        return succeeded;
    }

    //------------------------//

    #region NotMapped
    [NotMapped]
    public bool Trial { get => TrialEndDate < DateTime.Now.Date; }

    //- - - - - - - - - - - - //

    /// <summary>
    /// Subscription Name 
    /// </summary>
    [NotMapped]
    public string Name =>
        SubscriptionPlan?.Name ?? string.Empty;

    //- - - - - - - - - - - - //

    /// <summary>
    /// Subscription Description 
    /// </summary>
    [NotMapped]
    public string Description =>
        SubscriptionPlan?.Description ?? string.Empty;
    #endregion

    //------------------------//

    #region EqualsAndHashcode
    public override bool Equals(object? thatObj) =>
        thatObj is TeamSubscription that
           && TeamId == that.TeamId
           && SubscriptionPlanId == that.SubscriptionPlanId;

    //- - - - - - - - - - - - //     

    public override int GetHashCode() =>
        HashCode.Combine(TeamId, SubscriptionPlanId);

    #endregion

    //------------------------// 

}//Cls
