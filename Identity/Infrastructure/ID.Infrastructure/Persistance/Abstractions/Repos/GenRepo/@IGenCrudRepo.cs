using ID.Domain.Entities.Common;

namespace ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;

/// <summary>
/// Create, Read, Update, Delete repo
/// </summary>
internal interface IGenCrudRepo<T> :
    IGenCreateRepo<T>,
    IGenReadRepo<T>,
    IGenUpdateRepo<T>,
    IGenDeleteRepo<T> 
    where T : class, IIdBaseDomainEntity;
