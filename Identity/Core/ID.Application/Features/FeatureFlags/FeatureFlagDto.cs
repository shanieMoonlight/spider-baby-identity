using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;

namespace ID.Application.Features.FeatureFlags;
public class FeatureFlagDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string? AdministratorUsername { get; set; } = string.Empty;
    public string? AdministratorId { get; set; } = string.Empty;
    public DateTime? DateCreated { get; set; }

    //------------------------------------//

    #region ModelBindingCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public FeatureFlagDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    public FeatureFlagDto(FeatureFlag mdl)
    {
        Name = mdl.Name;
        Description = mdl.Description;
        Id = mdl.Id;
        AdministratorUsername = mdl.AdministratorUsername;
        AdministratorId = mdl.AdministratorId;
        DateCreated = mdl.DateCreated;
    }

    //------------------------------------//

}//Cls

