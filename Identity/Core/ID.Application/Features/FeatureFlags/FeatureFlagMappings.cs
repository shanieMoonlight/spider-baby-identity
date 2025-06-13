using ClArch.ValueObjects;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;

namespace ID.Application.Features.FeatureFlags;

public static class FeatureFlagMappings
{
    //------------------------------------//

    public static FeatureFlag Update(this FeatureFlag model, FeatureFlagDto dto) =>
        model.Update(
            Name.Create(dto.Name),
            Description.Create(dto.Description)
        );

    //------------------------------------//

    public static FeatureFlag ToModel(this FeatureFlagDto dto) =>
        FeatureFlag.Create(
            Name.Create(dto.Name),
            Description.Create(dto.Description)
        );

    //------------------------------------//

    public static FeatureFlagDto ToDto(this FeatureFlag mdl) =>
        new(mdl);

    //------------------------------------//

}//Cls


