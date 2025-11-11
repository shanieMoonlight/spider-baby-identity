using ID.Application.AppAbs.Setup;
using ID.Infrastructure.Persistance.EF;
using Microsoft.EntityFrameworkCore;

namespace ID.Infrastructure.Persistance.EF.Services;
internal class MigrateService(IdDbContext _db) : IIdMigrateService
{

    public async Task MigrateAsync()
    {
        //_db.Database.EnsureCreated();
        await _db.Database.MigrateAsync();

    }

}//Cls
