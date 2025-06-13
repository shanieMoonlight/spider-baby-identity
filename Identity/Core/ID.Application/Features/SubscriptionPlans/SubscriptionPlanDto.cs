using ID.Application.Features.Common.Dtos;
using ID.Application.Features.FeatureFlags;
using ID.Application.Features.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans;

namespace ID.Application.Features.SubscriptionPlans;
public class SubscriptionPlanDto : AuditableEntityDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public SubscriptionRenewalTypes RenewalType { get; set; }

    public double Price { get; set; } = 0;

    /// <summary>
    /// Maximum device this user can use - 0 means unlimited
    /// </summary>
    public int DeviceLimit { get; set; } = 0;

    /// <summaryc
    /// How many months will the Sub be free for
    /// </summary>
    public int TrialMonths { get; set; } = 0;


    public IEnumerable<FeatureFlagDto> FeatureFlags { get; set; } = [];
    public IEnumerable<Guid> FeatureFlagIds { get; set; } = [];

    //------------------------------------//

    #region ModelBinding
    public SubscriptionPlanDto() { }
    #endregion

    public SubscriptionPlanDto(SubscriptionPlan mdl) : base(mdl)
    {

        Id = mdl.Id;
        Name = mdl.Name;
        Description = mdl.Description;
        RenewalType = mdl.RenewalType;
        DeviceLimit = mdl.DeviceLimit;
        Price = mdl.Price;
        TrialMonths = mdl.TrialMonths;

        FeatureFlagIds = mdl.FeatureFlags.Select(f => f.Id).ToList();
        FeatureFlags = mdl.FeatureFlags.Select(f => f.ToDto()).ToList();
    }

    //------------------------------------//

}

