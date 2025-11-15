using ID.Application.AppAbs.Setup;
using Microsoft.EntityFrameworkCore;

namespace ID.Persistence.Ef.SQL.Services;

internal class SqlDbMigrator(IdSqlMigrationsContext migrationsDb) : IIdMigrateService
{
    private static readonly SemaphoreSlim _inProcMigrateLock = new(1, 1);

    //----------------------//

    public async Task MigrateAsync()
    {
        await _inProcMigrateLock.WaitAsync();
        try
        {
            var pending = migrationsDb.Database.GetPendingMigrations().ToList();
            Console.WriteLine("[SqlServerMigrateDb] Pending migrations: " + (pending.Count != 0 ? string.Join(", ", pending) : "<none>"));

            await migrationsDb.Database.MigrateAsync();
        }
        finally
        {
            _inProcMigrateLock.Release();
        }
    }

}//Cls