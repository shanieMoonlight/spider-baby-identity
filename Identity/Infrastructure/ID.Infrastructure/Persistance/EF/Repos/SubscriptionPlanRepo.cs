using ID.Domain.Entities.SubscriptionPlans;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Abstractions;
using MyResults;

namespace ID.Infrastructure.Persistance.EF.Repos;
internal class SubscriptionPlanRepo(IdDbContext db) : AGenCrudRepo<SubscriptionPlan>(db), IIdentitySubscriptionPlanRepo
{
    protected override Task<BasicResult> CanDeleteAsync(SubscriptionPlan? dbPlan)
    {
        if (dbPlan is null)
            return Task.FromResult(BasicResult.Success());

        var subCount = dbPlan.Subscriptions
            ?.Count ?? 0;
        var isAre = subCount > 1 ? "are" : "is";
        var mbr = subCount > 1 ? "subscriptions" : "subscription";

        var result = subCount > 0
            ? BasicResult.BadRequestResult($"There {isAre} {subCount} {mbr} connected to Plan, {dbPlan.Name}. You must delete them or move them to a different Team before deleting this Subscription Plan.")
            : BasicResult.Success();

        return Task.FromResult(result);
    }

}//Cls
