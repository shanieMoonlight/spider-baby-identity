using ClArch.SimpleSpecification;
using ID.Domain.Entities.Common;

namespace ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;

/// <summary>
/// Create
/// </summary>
internal interface IGenCreateRepo<T> where T : class, IIdBaseDomainEntity
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity">Database item</param>
    /// <returns>Returns the added entity</returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Enter Multiple Items at once
    /// </summary>
    /// <param name="entities">Database items</param>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);


}//Int
