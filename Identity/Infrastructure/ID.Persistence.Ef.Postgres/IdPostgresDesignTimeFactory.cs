using ID.GlobalSettings.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace ID.Persistence.Ef.Postgres;


/// <summary>
/// Ef Tooling will look for this class when running commands such as "dotnet ef migrations add"
/// </summary>
public class IdPostgresDesignTimeFactory : IDesignTimeDbContextFactory<IdPostgresMigrationsContext>
{
    public IdPostgresMigrationsContext CreateDbContext(string[] args)
    {
        // Directory.GetCurrentDirectory() will be the project folder when --project points here.
        var cfg = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true) // Or whatever appsettings file has your Postgres Settings.
            .AddEnvironmentVariables()
            .Build();

        var cs = cfg.GetConnectionString("PostgresDb")
                 ?? Environment.GetEnvironmentVariable("ConnectionStrings__PostgresDb")
                 ?? throw new InvalidOperationException("Postgres connection string not found (ConnectionStrings:PostgresDb or ConnectionStrings__PostgresDb).");

        Console.WriteLine($"ConnectionString: {cs}");

        var options = new DbContextOptionsBuilder<IdPostgresMigrationsContext>();
        options.UseNpgsql(cs, npgsql => npgsql.MigrationsHistoryTable(IdGlobalConstants.Db.MIGRATIONS_HISTORY_TABLE, IdGlobalConstants.Db.SCHEMA))
               .UseSnakeCaseNamingConvention()
               .EnableSensitiveDataLogging(false);

        return new IdPostgresMigrationsContext(options.Options);
    }

}