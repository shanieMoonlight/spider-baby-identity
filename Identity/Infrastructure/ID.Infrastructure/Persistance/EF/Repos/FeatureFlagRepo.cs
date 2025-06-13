using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Abstractions;
using MyResults;

namespace ID.Infrastructure.Persistance.EF.Repos;
internal class FeatureFlagRepo(IdDbContext db) : AGenCrudRepo<FeatureFlag>(db), IIdentityFeatureFlagRepo
{
    protected override Task<BasicResult> CanDeleteAsync(FeatureFlag? dbFeature)
    {
        if (dbFeature is null)
            return Task.FromResult(BasicResult.Success());

        var subCount = dbFeature.SubscriptionPlans
            ?.Count ?? 0;
        var isAre = subCount > 1 ? "are" : "is";
        var themIt = subCount > 1 ? "them" : "it";
        var mbr = subCount > 1 ? "plans" : "plan";

        var result = subCount > 0
            ? BasicResult.BadRequestResult($"There {isAre} {subCount} {mbr} connected to Feature, {dbFeature.Name}. You must delete {themIt} or remove this Feature from any connected SubscriptionPlans.")
            : BasicResult.Success();

        return Task.FromResult(result);
    }

}//Cls
