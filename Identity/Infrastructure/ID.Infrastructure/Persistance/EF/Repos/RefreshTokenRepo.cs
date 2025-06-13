using ID.Domain.Entities.Refreshing;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Persistance.EF.Repos;
internal class RefreshTokenRepo(IdDbContext db)
    : AGenCrudRepo<IdRefreshToken>(db), IIdentityRefreshTokenRepo
{    public async Task UpsertRefreshTokenAsync(IdRefreshToken entity)
    {
        await Db.Database.ExecuteSqlRawAsync(@"
            MERGE INTO [MyId].[RefreshTokens] AS Target
            USING (VALUES ({0}, {1}, {2}, {3}, {4})) AS Source (Id, UserId, Payload, ExpiresOnUtc, DateCreated)
            ON Target.UserId = Source.UserId
            WHEN MATCHED THEN
                UPDATE SET 
                    Payload = Source.Payload,
                    ExpiresOnUtc = Source.ExpiresOnUtc
            WHEN NOT MATCHED THEN
                INSERT (Id, UserId, Payload, ExpiresOnUtc, DateCreated)
                VALUES (Source.Id, Source.UserId, Source.Payload, Source.ExpiresOnUtc, Source.DateCreated);",
        entity.Id, entity.UserId, entity.Payload, entity.ExpiresOnUtc, entity.DateCreated);
    }

}
