using ID.GlobalSettings.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ID.Persistence.Ef.SQL;

/// <summary>
/// Ef Tooling will look for this class when running commands such as "dotnet ef migrations add"
/// <para/>
/// [DbContext(typeof(PnaSqlMigrationsContext))] will appear in the ModelSnapshot file to indicate which context it is for.
/// </summary>
public class IdSqlDesignTimeFactory : IDesignTimeDbContextFactory<IdSqlMigrationsContext>
{
    public IdSqlMigrationsContext CreateDbContext(string[] args)
    {
        var cfg = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)// Or whatever appsettings file has your SQL Settings.
            .AddEnvironmentVariables()
            .Build();

        var cs = cfg.GetConnectionString("SqlDb")
                 ?? Environment.GetEnvironmentVariable("ConnectionStrings__SqlDb")
                 ?? throw new InvalidOperationException("SQL Server connection string not found (ConnectionStrings:SqlDb or ConnectionStrings__SqlDb).");

        Console.WriteLine($"ConnectionString: {cs}");

        var options = new DbContextOptionsBuilder<IdSqlMigrationsContext>();
        options.UseSqlServer(cs, sql => sql.MigrationsHistoryTable(IdGlobalConstants.Db.MIGRATIONS_HISTORY_TABLE, IdGlobalConstants.Db.SCHEMA))
               .EnableSensitiveDataLogging(false);

        return new IdSqlMigrationsContext(options.Options);
    }
}