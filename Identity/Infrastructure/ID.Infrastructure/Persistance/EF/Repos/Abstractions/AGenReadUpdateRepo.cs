using ID.Domain.Entities.Common;
using ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Persistance.EF.Repos.Abstractions;
internal abstract class AGenReadUpdateRepo<T>(IdDbContext _db) : AGenReadRepo<T>(_db), IGenUpdateRepo<T> where T : class, IIdBaseDomainEntity
{
    /// <summary>
    /// Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The updated entity.</returns>
    public virtual Task<T> UpdateAsync(T entity)
    {
        Db.Entry(entity).State = EntityState.Modified;
        return Task.FromResult(entity);
    }

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Updates the specified range of entities.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        Db.Set<T>().UpdateRange(entities);
        return Task.CompletedTask;
    }
}//Cls
