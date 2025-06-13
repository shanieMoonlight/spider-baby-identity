using ID.Domain.Entities.Common;
using ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;

namespace ID.Infrastructure.Persistance.EF.Repos.Abstractions;
internal abstract class AGenCrudRepo<T>(IdDbContext db)
    : AGenReadUpdateDeleteRepo<T>(db), IGenCrudRepo<T> where T : class, IIdBaseDomainEntity
{
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default) =>
        (await Db.Set<T>().AddAsync(entity, cancellationToken))
        .Entity;

    //---------------------------//

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default) =>
        await Db.Set<T>().AddRangeAsync(entities, cancellationToken);



}//Cls
