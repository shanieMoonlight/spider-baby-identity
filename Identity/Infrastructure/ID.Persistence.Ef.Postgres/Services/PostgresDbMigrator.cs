using ID.Application.AppAbs.Setup;
using Microsoft.EntityFrameworkCore;

namespace ID.Persistence.Ef.Postgres.Services;

internal class PostgresDbMigrator(IdPostgresMigrationsContext migrationsDb) : IIdMigrateService
{
    private static readonly SemaphoreSlim _inProcMigrateLock = new(1, 1);

    //----------------------//

    public async Task MigrateAsync()
    {
        await _inProcMigrateLock.WaitAsync();
        try
        {
            // you may keep diagnostics here if helpful
            var pending = migrationsDb.Database.GetPendingMigrations().ToList();
            Console.WriteLine("[PostgresMigrateDb] Pending migrations: " + (pending.Count != 0 ? string.Join(", ", pending) : "<none>"));

            // now run migrations against the target DB
            await migrationsDb.Database.MigrateAsync();
        }
        finally
        {
            _inProcMigrateLock.Release();
        }
    }
}