using ClArch.SimpleSpecification;
using CollectionHelpers;
using ID.Domain.Entities.Common;
using ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;
using ID.Infrastructure.Persistance.Abstractions.Repos.Specs;
using ID.Infrastructure.Persistance.EF;
using Microsoft.EntityFrameworkCore;
using Pagination;
using Pagination.Extensions;
using StringHelpers;

namespace ID.Infrastructure.Persistance.EF.Repos.Abstractions;

/// <summary>
/// Abstract generic read repository.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
internal abstract class AGenReadRepo<T>(IdDbContext _db) : IGenReadRepo<T> where T : class, IIdBaseDomainEntity
{
    protected readonly IdDbContext Db = _db;

    //-----------------------//

    /// <summary>
    /// Gets the count of all entities.
    /// </summary>
    /// <returns>The count of all entities.</returns>
    public Task<int> CountAsync() =>
        Task.FromResult(Db.Set<T>().Count());

    //-----------------------//

    /// <summary>
    /// Checks if an entity exists based on the provided specification.
    /// </summary>
    /// <param name="spec">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the entity exists, otherwise false.</returns>
    public async Task<bool> ExistsAsync(ASimpleSpecification<T> spec, CancellationToken cancellationToken = default)
    {
        if (spec.ShouldShortCircuit())
            return false;

        return await Db.Set<T>()
            .BuildQuery(spec)
            .AnyAsync(cancellationToken);
    }

    //-----------------------//

    /// <summary>
    /// Checks if an entity with the specified ID exists.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>True if the entity exists, otherwise false.</returns>
    public async Task<bool> ExistsAsync(Guid? id)
    {
        if (id == null)
            return false;

        var entity = await FirstOrDefaultByIdAsync(id);

        return entity is not null;
    }

    //-----------------------//

    /// <summary>
    /// Retrieves the first entity that matches the specification.
    /// </summary>
    /// <param name="spec">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity if it exists.</returns>
    public virtual async Task<T?> FirstOrDefaultAsync(ASimpleSpecification<T> spec, CancellationToken cancellationToken = default)
    {
        if (spec.ShouldShortCircuit())
            return null;

        return await Db.Set<T>()
            .BuildQuery(spec)
            .FirstOrDefaultAsync(cancellationToken);
    }

    //-----------------------//

    /// <summary>
    /// Retrieves the first entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>The entity if it exists.</returns>
    public async Task<T?> FirstOrDefaultByIdAsync(Guid? id) =>
        await Db.Set<T>().FindAsync(id);

    //-----------------------//

    /// <summary>
    /// Gets the property selector lambda for filtering.
    /// </summary>
    /// <param name="field">The field name.</param>
    /// <returns>The property selector lambda.</returns>
    protected virtual string GetFilteringPropertySelectorLambda(string field) =>
        field.CamelToPascal();

    //-----------------------//

    /// <summary>
    /// Gets the custom sort expression builder.
    /// </summary>
    /// <returns>The custom sort expression builder.</returns>
    protected virtual CustomSortExpressionBuilder<T>? GetSortBuider() => null;

    //-----------------------//

    /// <summary>
    /// Gets a list of all entities.
    /// </summary>
    /// <returns>A list of all entities.</returns>
    public async Task<IReadOnlyList<T>> ListAllAsync() =>
        await Db.Set<T>().ToListAsync();

    //-----------------------//

    /// <summary>
    /// Retrieves a list of all entities that match the specification.
    /// </summary>
    /// <param name="spec">The specification to match.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all entities that match the specification.</returns>
    public virtual async Task<IReadOnlyList<T>> ListAllAsync(ASimpleSpecification<T> spec, CancellationToken cancellationToken = default)
    {
        if (spec.ShouldShortCircuit())
            return [];

        return await Db.Set<T>()
            .BuildQuery(spec)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    //-----------------------//

    /// <summary>
    /// Gets a list of entities by their IDs.
    /// </summary>
    /// <param name="ids">The IDs of the entities.</param>
    /// <returns>A list of entities.</returns>
    public async Task<IReadOnlyList<T>> ListByIdsAsync(IEnumerable<Guid>? ids)
    {
        if (!ids.AnyValues())
            return []; //Don't waste time querying the DB

        return await Db.Set<T>()
            .Where(item => ids!.Contains(item.Id))
            .ToListAsync();
    }

    //-----------------------//

    /// <summary>
    /// Gets a paginated list of entities.
    /// </summary>
    /// <param name="request">The pagination request.</param>
    /// <returns>A paginated list of entities.</returns>
    public virtual async Task<Page<T>> PageAsync(PagedRequest request) =>
        await PageAsync(request.PageNumber, request.PageSize, request.SortList, request.FilterList);

    //-----------------------//

    /// <summary>
    /// Gets a paginated list of entities.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="sortList">The sorting criteria.</param>
    /// <param name="filterList">The filtering criteria.</param>
    /// <returns>A paginated list of entities.</returns>
    public virtual Task<Page<T>> PageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null)
    {
        var data = Db.Set<T>()
           .AddFiltering(filterList, GetFilteringPropertySelectorLambda)
           .AddEfSorting(sortList, GetSortBuider())
           .AsNoTracking();

        var page = new Page<T>(data, pageNumber, pageSize);

        return Task.FromResult(page);
    }

    //-----------------------//

    /// <summary>
    /// Gets first <paramref name="count"/> Entities, after skipping <paramref name="skip"/> entities.
    /// Whithout any where criteria.
    /// </summary>
    /// <param name="count">The number of entities to retrieve.</param>
    /// <param name="skip">The number of entities to skip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of <paramref name="count"/> entities.</returns>
    public async Task<IReadOnlyList<T>> TakeAsync(int count, int skip, CancellationToken cancellationToken = default) =>
        await Db.Set<T>()
                .Skip(skip)
                .Take(count)
                .ToListAsync(cancellationToken);

    //-----------------------//

    /// <summary>
    /// Gets first <paramref name="spec"/>.Count Entities, after skipping <paramref name="spec"/>.Skip entities.
    /// </summary>
    /// <param name="spec">The specification to match. Contains count, skip and any "wheres".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of entities.</returns>
    public async Task<IReadOnlyList<T>> TakeAsync(TakeSkipSpec<T> spec, CancellationToken cancellationToken = default)
    {
        if (spec.ShouldShortCircuit())
            return [];

        return await Db.Set<T>()
            .BuildQuery(spec)
            .Skip(spec.Skip)
            .Take(spec.Count)
            .ToListAsync(cancellationToken);
    }

}//Cls
