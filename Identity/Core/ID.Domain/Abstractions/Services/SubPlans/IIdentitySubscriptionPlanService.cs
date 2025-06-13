using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using Pagination;

namespace ID.Domain.Abstractions.Services.SubPlans;
public interface IIdentitySubscriptionPlanService
{

    /// <summary>
    /// Adds a new SubscriptionPlan along with associated FeatureFlags by their IDs.
    /// </summary>
    /// <param name="plan">The SubscriptionPlan to add.</param>
    /// <param name="featurFlagIds">The IDs of the FeatureFlags to associate with the SubscriptionPlan.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The added SubscriptionPlan with associated FeatureFlags.</returns>
    Task<SubscriptionPlan> AddAsync(SubscriptionPlan plan, IEnumerable<Guid>? featurFlagIds, CancellationToken cancellationToken = default);


    //- - - - - - - - - - - - //

    /// <summary>
    /// Adds a new SubscriptionPlan along with associated FeatureFlags.
    /// </summary>
    /// <param name="plan">The SubscriptionPlan to add.</param>
    /// <param name="featureFlags">The FeatureFlags to associate with the SubscriptionPlan.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The added SubscriptionPlan with associated FeatureFlags.</returns>
    Task<SubscriptionPlan> AddAsync(SubscriptionPlan plan, IEnumerable<FeatureFlag>? featureFlags, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Adds FeatureFlags to an existing SubscriptionPlan.
    /// </summary>
    /// <param name="plan">The SubscriptionPlan to update.</param>
    /// <param name="featureFlags">The FeatureFlags to associate with the SubscriptionPlan.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated SubscriptionPlan with associated FeatureFlags.</returns>
    Task<SubscriptionPlan> AddFeaturesToPlanAsync(SubscriptionPlan plan, IEnumerable<FeatureFlag>? featueFlags, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Adds FeatureFlags to an existing SubscriptionPlan.
    /// </summary>
    /// <param name="plan">The SubscriptionPlan to update.</param>
    /// <param name="featureFlagIds">The Ids of the FeatureFlags to associate with the SubscriptionPlan.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated SubscriptionPlan with associated FeatureFlags.</returns>
    Task<SubscriptionPlan> AddFeaturesToPlanAsync(SubscriptionPlan plan, IEnumerable<Guid>? featureFlagIds, CancellationToken cancellationToken = default);

    //------------------------//

    /// <summary>
    /// Deletes the SubscriptionPlan with the specified id.
    /// </summary>
    /// <param name="id">The identifier of the SubscriptionPlan to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get the first SubscriptionPlan that has this name. If it exists.
    /// </summary>
    /// <param name="name">Entity identifier</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the SubscriptionPlan if found, otherwise null.</returns>
    Task<SubscriptionPlan?> FirstByNameAsync(string? name);

    //------------------------//

    /// <summary>
    /// Get all SubscriptionPlans.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of all SubscriptionPlans.</returns>
    Task<IReadOnlyList<SubscriptionPlan>> GetAllAsync();

    //- - - - - - - - - - - - //

    /// <summary>
    /// Retrieves all SubscriptionPlans that match the given name filter.
    /// </summary>
    /// <param name="filter">The name filter to apply.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of SubscriptionPlans.</returns>
    Task<IReadOnlyList<SubscriptionPlan>> GetAllByNameAsync(string? filter);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get the SubscriptionPlan with id, <paramref name="id"/>  including any FeatureFlags.
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the SubscriptionPlan if found, otherwise null.</returns>
    Task<SubscriptionPlan?> GetByIdWithFeatureFlagsAsync(Guid? id);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a paginated list of SubscriptionPlans based on the provided request.
    /// </summary>
    /// <param name="request">The paginated request containing page number, page size, sorting, and filtering criteria.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated list of SubscriptionPlans.</returns>
    Task<Page<SubscriptionPlan>> GetPageAsync(PagedRequest request);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a paginated list of SubscriptionPlans.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="sortList">A collection of sorting criteria.</param>
    /// <param name="filterList">A collection of filtering criteria (optional).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated list of SubscriptionPlans.</returns>
    Task<Page<SubscriptionPlan>> GetPageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null);


    //------------------------//

    /// <summary>
    /// Removes FeatureFlags from an existing SubscriptionPlan by their IDs.
    /// </summary>
    /// <param name="plan">The SubscriptionPlan to update.</param>
    /// <param name="featureFlagIds">The IDs of the FeatureFlags to remove from the SubscriptionPlan.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated SubscriptionPlan without the specified FeatureFlags.</returns>
    Task<SubscriptionPlan> RemoveFeaturesFromPlanAsync(SubscriptionPlan plan, IEnumerable<Guid>? featureFlagIds, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Removes FeatureFlags from an existing SubscriptionPlan.
    /// </summary>
    /// <param name="plan">The SubscriptionPlan to update.</param>
    /// <param name="featureFlags">The FeatureFlags to remove from the SubscriptionPlan.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated SubscriptionPlan without the specified FeatureFlags.</returns>
    Task<SubscriptionPlan> RemoveFeaturesFromPlanAsync(SubscriptionPlan plan, IEnumerable<FeatureFlag>? featureFlags, CancellationToken cancellationToken = default);

    //------------------------//

    /// <summary>
    /// Update SubscriptionPlan.
    /// </summary>
    /// <param name="subscriptionPlan">The SubscriptionPlan to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated SubscriptionPlan.</returns>
    Task<SubscriptionPlan> UpdateAsync(SubscriptionPlan subscriptionPlan, CancellationToken cancellationToken = default);
}


