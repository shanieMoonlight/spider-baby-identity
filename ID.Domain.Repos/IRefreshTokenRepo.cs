using ID.Domain.Entities.Refreshing;
using ID.Domain.Repos.GenRepo;

namespace ID.Domain.Repos;
internal interface IIdentityRefreshTokenRepo : IGenCrudRepo<IdRefreshToken>
{
    //Task UpsertRefreshTokenAsync(IdRefreshToken entity);
}


