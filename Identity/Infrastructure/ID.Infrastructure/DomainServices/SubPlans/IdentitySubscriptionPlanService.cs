using CollectionHelpers;
using ID.Domain.Abstractions.Services.SubPlans;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.SubPlans;
using Pagination;

namespace ID.Infrastructure.DomainServices.SubPlans;

internal class IdentitySubscriptionPlanService(IIdUnitOfWork _uow)
    : IIdentitySubscriptionPlanService
{
    private readonly IIdentitySubscriptionPlanRepo _repo = _uow.SubscriptionPlanRepo;
    private readonly IIdentityFeatureFlagRepo _featuresRepo = _uow.FeatureFlagRepo;

    //-----------------------//

    /// <summary>
    /// Adds a new SubscriptionPlan along with associated FeatureFlags by their IDs.
    /// </summary>
    /// <param name="plan">The SubscriptionPlan to add.</param>
    /// <param name="featurFlagIds">The IDs of the FeatureFlags to associate with the SubscriptionPlan.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The added SubscriptionPlan with associated FeatureFlags.</returns>
    public async Task<SubscriptionPlan> AddAsync(SubscriptionPlan plan, IEnumerable<Guid>? featurFlagIds, CancellationToken cancellationToken = default)
    {
        var dbPlan = await _repo.AddAsync(plan, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        if (!featurFlagIds.AnyValues())
            return dbPlan;

        var uniqueIds = featurFlagIds!.Distinct();
        var dbFeatures = await _featuresRepo.ListByIdsAsync(uniqueIds);

        return await AddFeaturesToPlanAsync(dbPlan, dbFeatures, cancellationToken);
    }

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Adds a new SubscriptionPlan along with associated FeatureFlags.
    /// </summary>
    /// <param name="plan">The SubscriptionPlan to add.</param>
    /// <param name="featureFlags">The FeatureFlags to associate with the SubscriptionPlan.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The added SubscriptionPlan with associated FeatureFlags.</returns>
    public async Task<SubscriptionPlan> AddAsync(SubscriptionPlan plan, IEnumerable<FeatureFlag>? featureFlags, CancellationToken cancellationToken = default)
    {
        var dbPlan = await _repo.AddAsync(plan, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return await AddFeaturesToPlanAsync(dbPlan, featureFlags, cancellationToken);
    }

    //-----------------------//

    /// <summary>
    /// Adds FeatureFlags to an existing SubscriptionPlan.
    /// </summary>
    /// <param name="plan">The SubscriptionPlan to update.</param>
    /// <param name="featureFlagIds">The Ids of the FeatureFlags to associate with the SubscriptionPlan.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated SubscriptionPlan with associated FeatureFlags.</returns>
    public async Task<SubscriptionPlan> AddFeaturesToPlanAsync(SubscriptionPlan plan, IEnumerable<Guid>? featureFlagIds, CancellationToken cancellationToken = default)
    {
        if (!featureFlagIds.AnyValues())
            return plan;

        var uniqueIds = featureFlagIds!.Distinct();
        var dbFeatures = await _featuresRepo.ListByIdsAsync(uniqueIds);

        return await AddFeaturesToPlanAsync(plan, dbFeatures, cancellationToken);
    }

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Adds FeatureFlags to an existing SubscriptionPlan.
    /// </summary>
    /// <param name="plan">The SubscriptionPlan to update.</param>
    /// <param name="featureFlags">The FeatureFlags to associate with the SubscriptionPlan.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated SubscriptionPlan with associated FeatureFlags.</returns>
    public async Task<SubscriptionPlan> AddFeaturesToPlanAsync(SubscriptionPlan plan, IEnumerable<FeatureFlag>? featureFlags, CancellationToken cancellationToken = default)
    {
        if (!featureFlags.AnyValues())
            return plan;

        var distinctFeatures = featureFlags!.Distinct();

        plan.AddFeatureFlags(distinctFeatures);
        await _repo.UpdateAsync(plan);

        await _uow.SaveChangesAsync(cancellationToken);

        return plan;
    }

    //-----------------------//

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _repo.DeleteAsync(id);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    //-----------------------//

    public Task<SubscriptionPlan?> FirstByNameAsync(string? name) =>
        _repo.FirstOrDefaultAsync(new SubPlanByNameSpec(name));

    //-----------------------//

    public Task<IReadOnlyList<SubscriptionPlan>> GetAllAsync() =>
        _repo.ListAllAsync();

    //- - - - - - - - - - - - - - - - - - //

    public Task<IReadOnlyList<SubscriptionPlan>> GetAllByNameAsync(string? name)
        => _repo.ListAllAsync(new SubPlanByNameSpec(name));

    //- - - - - - - - - - - - - - - - - - //

    public async Task<SubscriptionPlan?> GetByIdWithFeatureFlagsAsync(Guid? id) =>
        await _repo.FirstOrDefaultAsync(new SubPlanByIdWithFlagsSpec(id));

    //-----------------------//

    public Task<Page<SubscriptionPlan>> GetPageAsync(PagedRequest request) => _repo.PageAsync(request);

    //-----------------------//

    public Task<Page<SubscriptionPlan>> GetPageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null)
        => _repo.PageAsync(pageNumber, pageSize, sortList, filterList);

    //-----------------------//

    public async Task<SubscriptionPlan> UpdateAsync(SubscriptionPlan subscriptionPlan, CancellationToken cancellationToken)
    {
        var updatedPlan = await _repo.UpdateAsync(subscriptionPlan);
        await _uow.SaveChangesAsync(cancellationToken);
        return updatedPlan;
    }

    //-----------------------//

    /// <summary>
    /// Removes FeatureFlags from an existing SubscriptionPlan by their IDs.
    /// </summary>
    /// <param name="plan">The SubscriptionPlan to update.</param>
    /// <param name="featureFlagIds">The IDs of the FeatureFlags to remove from the SubscriptionPlan.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated SubscriptionPlan without the specified FeatureFlags.</returns>
    public async Task<SubscriptionPlan> RemoveFeaturesFromPlanAsync(SubscriptionPlan plan, IEnumerable<Guid>? featureFlagIds, CancellationToken cancellationToken = default)
    {
        if (!featureFlagIds.AnyValues())
            return plan;

        var uniqueIds = featureFlagIds!.Distinct();
        var dbFeatures = await _featuresRepo.ListByIdsAsync(uniqueIds);

        return await RemoveFeaturesFromPlanAsync(plan, dbFeatures, cancellationToken);
    }

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Removes FeatureFlags from an existing SubscriptionPlan.
    /// </summary>
    /// <param name="plan">The SubscriptionPlan to update.</param>
    /// <param name="featureFlags">The FeatureFlags to remove from the SubscriptionPlan.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated SubscriptionPlan without the specified FeatureFlags.</returns>
    public async Task<SubscriptionPlan> RemoveFeaturesFromPlanAsync(SubscriptionPlan plan, IEnumerable<FeatureFlag>? featureFlags, CancellationToken cancellationToken = default)
    {
        if (!featureFlags.AnyValues())
            return plan;

        var distinctFeatures = featureFlags!.Distinct();

        plan.RemoveFeatureFlags(distinctFeatures);
        await _repo.UpdateAsync(plan);

        await _uow.SaveChangesAsync(cancellationToken);

        return plan;
    }

    //-----------------------//

}//Cls
