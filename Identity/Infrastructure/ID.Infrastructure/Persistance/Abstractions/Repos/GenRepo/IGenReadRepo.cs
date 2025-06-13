using ClArch.SimpleSpecification;
using ID.Domain.Entities.Common;
using ID.Infrastructure.Persistance.Abstractions.Repos.Specs;
using Pagination;

namespace ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;

/// <summary>
/// Interface for a generic read repository.
/// </summary>
internal interface IGenReadRepo<T> where T : class, IIdBaseDomainEntity
{
    /// <summary>
    /// Gets the number of Items in the Db
    /// </summary>
    Task<int> CountAsync();

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Checks if an entity exists based on the provided specification.
    /// </summary>
    /// <param name="spec">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the entity exists, otherwise false.</returns>
    Task<bool> ExistsAsync(ASimpleSpecification<T> spec, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Does an Entity with id, <paramref name="id"/> exist?
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <returns>True if entity was found</returns>
    Task<bool> ExistsAsync(Guid? id);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Get the entity with id, <paramref name="id"/>
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <returns>The entity if it exists</returns>
    Task<T?> FirstOrDefaultByIdAsync(Guid? id);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Retrieves the first entity that matches the specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity if it exists</returns>
    Task<T?> FirstOrDefaultAsync(ASimpleSpecification<T> specification, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <returns>List of all entities</returns>
    Task<IReadOnlyList<T>> ListAllAsync();

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a list of all entities that match the specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all entities that match the specification</returns>
    Task<IReadOnlyList<T>> ListAllAsync(ASimpleSpecification<T> specification, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a list of entities by their identifiers.
    /// </summary>
    /// <param name="ids">The identifiers of the entities.</param>
    /// <returns>List of entities</returns>
    Task<IReadOnlyList<T>> ListByIdsAsync(IEnumerable<Guid>? ids);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Gets a paginated list of entities.
    /// </summary>
    /// <param name="request">The pagination request.</param>
    /// <returns>A paginated list of entities.</returns>
    Task<Page<T>> PageAsync(PagedRequest request);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Retrieves a paginated list of entities.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="sortList">A collection of sorting criteria.</param>
    /// <param name="filterList">A collection of filtering criteria (optional).</param>
    /// <returns>A paginated list of Entities.</returns>
    Task<Page<T>> PageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Gets first <paramref name="count"/> Entities, after skipping <paramref name="skip"/> entities.
    /// </summary>
    /// <param name="count">The number of entities to retrieve.</param>
    /// <param name="skip">The number of entities to skip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of <paramref name="count"/> entities</returns>
    Task<IReadOnlyList<T>> TakeAsync(int count, int skip, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Gets first <paramref name="spec"/>.Count Entities, after skipping <paramref name="spec"/>.Skip entities.
    /// </summary>
    /// <param name="spec">The specification to match. Contains count, skip and any "wheres"</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of entities</returns>
    Task<IReadOnlyList<T>> TakeAsync(TakeSkipSpec<T> specification, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - - - - - - - //

}//Int

