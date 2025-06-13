using ClArch.SimpleSpecification;
using ID.Domain.Entities.Common;
using ID.Infrastructure.Persistance.Abstractions.Repos.Specs;

namespace ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;

/// <summary>
///  Delete repo
/// </summary>
internal interface IGenDeleteRepo<T> where T : class, IIdBaseDomainEntity
{
    /// <summary>
    /// Delete <paramref name="entity"/>
    /// </summary>
    /// <param name="entity">Database item</param>
    /// <returns></returns>
    Task DeleteAsync(T entity);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Delete entity with id, <paramref name="id"/>
    /// </summary>
    /// <param name="id">Entity identifier</param>
    /// <returns></returns>
    Task DeleteAsync(Guid? id);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Remove a range of entities based on the provided specification.
    /// </summary>
    /// <param name="spec">Specification to filter entities to be removed</param>
    /// <returns></returns>
    Task RemoveRangeAsync(ASimpleSpecification<T> spec);

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Remove a range of entities.
    /// </summary>
    /// <param name="entities">Collection of entities to be removed</param>
    /// <returns></returns>
    Task RemoveRangeAsync(IEnumerable<T> entities);

    //- - - - - - - - - - - - - - - - - - //

}
