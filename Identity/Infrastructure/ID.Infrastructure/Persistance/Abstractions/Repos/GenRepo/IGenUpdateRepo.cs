using ID.Domain.Entities.Common;

namespace ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;

/// <summary>
///  Update
/// </summary>
internal interface IGenUpdateRepo<T> where T : IIdBaseDomainEntity
{
    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="entity">Database item</param>
    /// <returns>The updated entity</returns>
    Task<T> UpdateAsync(T entity);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Starts tracking entities for updates
    /// </summary>
    /// <param name="entity">Database item</param>
    /// <returns>Returns the added entity</returns>
    Task UpdateRangeAsync(IEnumerable<T> entities);

 
}//Int
