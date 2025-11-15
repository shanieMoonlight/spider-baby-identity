using ID.Application.Models;
using ID.Jobs.Quartz.Persistence.Abs;
using ID.Jobs.Quartz.Persistence.Ef;
using ID.Jobs.Quartz.Persistence.Ef.Postgres;
using ID.Jobs.Quartz.Persistence.Ef.SqlServer;
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
                services.AddScoped<IDbCommandExecutor, SqlDbCommandExecutor>();
                services.TryAddScoped<IEfCoreMigrator, SqlEfCoreMigrator>();
                break;
            case DatabaseType.PostgreSql:
                services.AddScoped<IDbCommandExecutor, PgDbCommandExecutor>();
                services.TryAddScoped<IEfCoreMigrator, PgEfCoreMigrator>();
                break;
            default:
                throw new NotSupportedException($"The database type '{dbType}' is not supported for Quartz job implementations.");
        }

        services.AddSingleton<IMigrationNotifier, InMemoryMigrationNotifier>();

        return services;
    }



}//Cls
