using ID.Domain.Entities.SubscriptionPlans.FeatureFlags;
using Pagination;

namespace ID.Domain.Abstractions.Services.SubPlans;
public interface IIdentityFeatureFlagService
{

    /// <summary>
    /// Adds a new FeatureFlag along with associated FeatureFlags by their IDs.
    /// </summary>
    /// <param name="flag">The FeatureFlag to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The added FeatureFlag.</returns>
    Task<FeatureFlag> AddAsync(FeatureFlag flag, CancellationToken cancellationToken);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Deletes the FeatureFlag with the specified id.
    /// </summary>
    /// <param name="id">The identifier of the FeatureFlag to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(Guid? id);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Deletes the FeatureFlag <paramref name="flag"/>.
    /// </summary>
    /// <param name="flag">The FeatureFlag to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(FeatureFlag flag);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get all FeatureFlags.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of all FeatureFlags.</returns>
    Task<IReadOnlyList<FeatureFlag>> GetAllAsync();
    Task<IReadOnlyList<FeatureFlag>> GetAllByNameAsync(string? name, CancellationToken cancellationToken);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get the FeatureFlag with id, <paramref name="id"/> . Incluse no Foreign Key Properties.
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the FeatureFlag if found, otherwise null.</returns>
    Task<FeatureFlag?> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get the FeatureFlag with id, <paramref name="id"/>  including any SubPlans.
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the FeatureFlag if found, otherwise null.</returns>
    Task<FeatureFlag?> GetByIdWithPlansAsync(Guid? id);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Get the FeatureFlag with name, <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the FeatureFlag.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the FeatureFlag if found, otherwise null.</returns>
    Task<FeatureFlag?> GetByNameAsync(string? name, CancellationToken cancellationToken);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a paginated list of FeatureFlags based on the provided request.
    /// </summary>
    /// <param name="request">The paginated request containing page number, page size, sorting, and filtering criteria.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated list of FeatureFlags.</returns>
    Task<Page<FeatureFlag>> GetPageAsync(PagedRequest request);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a paginated list of FeatureFlags.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="sortList">A collection of sorting criteria.</param>
    /// <param name="filterList">A collection of filtering criteria (optional).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated list of FeatureFlags.</returns>
    Task<Page<FeatureFlag>> GetPageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Update FeatureFlag.
    /// </summary>
    /// <param name="subscriptionPlan">The FeatureFlag to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated FeatureFlag.</returns>
    Task<FeatureFlag> UpdateAsync(FeatureFlag subscriptionPlan);

}//Int
