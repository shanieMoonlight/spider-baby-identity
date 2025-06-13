using ClArch.SimpleSpecification;
using ID.Domain.Utility.Exceptions;
using ID.Domain.Entities.Common;
using ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;
using MyResults;

namespace ID.Infrastructure.Persistance.EF.Repos.Abstractions;
internal abstract class AGenReadUpdateDeleteRepo<T>(IdDbContext _db) : AGenReadUpdateRepo<T>(_db), IGenDeleteRepo<T> where T : class, IIdBaseDomainEntity
{
    /// <summary>
    /// Delete <paramref name="entity"/>
    /// </summary>
    /// <exception cref="CantDeleteException"></exception>
    public virtual async Task DeleteAsync(T entity)
    {
        var canDeleteResult = await CanDeleteAsync(entity);
        if (!canDeleteResult.Succeeded)
            throw new CantDeleteException(typeof(T).Name, canDeleteResult.Info);

        Db.Set<T>().Remove(entity);
    }

    //- - - - - - - - - - - - - - - - - - //

    /// <summary>
    /// Delete item with id <paramref name="id"/>
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <returns></returns>
    /// <exception cref="CantDeleteException"></exception>
    public virtual async Task DeleteAsync(Guid? id)
    {
        if (id == null)
            return;

        var entity = await Db.Set<T>()
            .FindAsync(id);

        if (entity == null) //Already Deleted
            return;

        var canDeleteResult = await CanDeleteAsync(entity);
        if (!canDeleteResult.Succeeded)
            throw new CantDeleteException(typeof(T).Name, canDeleteResult.Info);


        await DeleteAsync(entity);
    }

    //- - - - - - - - - - - - - - - - - - //

    protected virtual Task<BasicResult> CanDeleteAsync(T? entity) =>
        Task.FromResult(BasicResult.Success());

    //-----------------------//

    /// <inheritdoc/>
    public Task RemoveRangeAsync(IEnumerable<T> entities)
    {
        Db.Set<T>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    //- - - - - - - - - - - - - - - - - - //

    /// <inheritdoc/>
    public Task RemoveRangeAsync(ASimpleSpecification<T> spec)
    {
        if (spec.ShouldShortCircuit())
            return Task.CompletedTask;

        var entitiesToRemove = Db.Set<T>()
            .BuildQuery(spec);

        Db.Set<T>().RemoveRange(entitiesToRemove);
        return Task.CompletedTask;
    }

}//Cls
