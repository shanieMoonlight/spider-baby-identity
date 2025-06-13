using ID.Domain.Entities.Refreshing;
using ID.Infrastructure.Persistance.Abstractions.Repos.GenRepo;

namespace ID.Infrastructure.Persistance.Abstractions.Repos;
internal interface IIdentityRefreshTokenRepo : IGenCrudRepo<IdRefreshToken>
{
    //Task UpsertRefreshTokenAsync(IdRefreshToken entity);
}


