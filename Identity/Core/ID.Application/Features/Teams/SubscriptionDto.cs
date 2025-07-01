using ID.Application.Features.Common.Dtos;
using ID.Application.Features.Teams;
using ID.Application.Features.SubscriptionPlans;
using ID.Application.Features.Teams.Cmd.Dvcs;
using ID.Domain.Entities.Teams;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.Subscriptions;


namespace ID.Application.Features.Teams;
public class SubscriptionDto : AuditableEntityDto
{
    public Guid Id { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public SubscriptionPlanDto? SubscriptionPlan { get; set; }

    public SubscriptionStatus SubscriptionStatus { get; set; }
    public double Discount { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public DateTime TrialStartDate { get; set; }
    public DateTime TrialEndDate { get; set; }

    public DateTime LastPaymentDate { get; set; }
    public double LastPaymenAmount { get; set; }


    //Fks
    public Guid TeamId { get; set; }
    public IEnumerable<DeviceDto> Devices { get; set; } = [];




    //From Plan
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Trial { get; set; }
    public SubscriptionRenewalTypes? RenewalType { get; private set; }




    //------------------------------------//

    #region ModelBinding
#pragma warning disable CS8618
    private SubscriptionDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    public SubscriptionDto(TeamSubscription mdl) : base(mdl)
    {
        Id = mdl.Id;
        SubscriptionPlanId = mdl.SubscriptionPlanId;
        SubscriptionPlan = mdl.SubscriptionPlan?.ToDto();
        SubscriptionStatus = mdl.SubscriptionStatus;
        Discount = mdl.Discount;
        StartDate = mdl.StartDate;
        EndDate = mdl.EndDate;
        TrialStartDate = mdl.TrialStartDate;
        TrialEndDate = mdl.TrialEndDate;
        LastPaymentDate = mdl.LastPaymentDate;
        LastPaymenAmount = mdl.LastPaymenAmount;
        Trial = mdl.Trial;
        Name = mdl.Name;
        Description = mdl.Description;
        RenewalType = mdl.RenewalType;
        TeamId = mdl.TeamId;
        Devices = mdl.Devices?.Select(d => d.ToDto()) ?? [];


    }

    //------------------------------------//
}

