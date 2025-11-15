using ID.Application.AppAbs.Setup;
using ID.Persistence.Ef;
using Microsoft.EntityFrameworkCore;

namespace ID.Persistence.Ef.Services;
internal class MigrateService(IdDbContext _db) : IIdMigrateService
{

    public async Task MigrateAsync()
    {
        //_db.Database.EnsureCreated();
        await _db.Database.MigrateAsync();

    }

}//Cls
