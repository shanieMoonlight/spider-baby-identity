using ID.Application.Models;
using ID.Jobs.Quartz.Persistence.Abs;
using ID.Jobs.Quartz.Persistence.DbUp;
using ID.Jobs.Quartz.Persistence.Ef;
using ID.Jobs.Quartz.Persistence.MigrationNotifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ID.Jobs.Quartz.Persistence;
internal static class SetupPersistence
{

    public static IServiceCollection AddQuartzPersistence(this IServiceCollection services, DatabaseType dbType)
    {

        services.AddScoped<IEmbeddedScriptLoader, EmbeddedScriptLoader>();

        switch (dbType)
        {
            case DatabaseType.SqlServer:
                services.TryAddScoped<IEfCoreMigrator, SqlEfCoreMigrator>();
                //services.TryAddScoped<IDbUpMigrator, DbUpSqlServerMigrator>();
                break;
            case DatabaseType.PostgreSql:
                services.TryAddScoped<IEfCoreMigrator, PgEfCoreMigrator>();
                //services.TryAddScoped<IDbUpMigrator, DbUpPostgresServerMigrator>();
                break;
            default:
                throw new NotSupportedException($"The database type '{dbType}' is not supported for Quartz job implementations.");
        }

        services.AddSingleton<IMigrationNotifier, InMemoryMigrationNotifier>();

        return services;
    }



}//Cls
